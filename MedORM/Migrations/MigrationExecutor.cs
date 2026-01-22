using MedORM.Migrations.Models;
using Npgsql;
using System.Reflection;

namespace MedORM.Migrations
{
    public class MigrationExecutor
    {
        private readonly string _connectionString;
        private readonly Assembly _entityAssembly;

        public MigrationExecutor(string connectionString, Assembly entityAssembly)
        {
            _connectionString = connectionString;
            _entityAssembly = entityAssembly;
        }

        public void RunMigrations()
        {
            Console.WriteLine("=== Starting ORM Migrations ===");

            using var conn = new NpgsqlConnection(_connectionString);

            try
            {
                conn.Open();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error connecting to database: {e.Message}");
            }

            CreateMigrationsTable(conn);

            var storage = new SchemaStorage();
            var oldSnapshot = storage.LoadSnapshot();

            var scanner = new EntityScanner(_entityAssembly);
            var newSnapshot = scanner.ScanEntities();

            if (oldSnapshot == null && !DatabaseHasTables(conn))
            {
                Console.WriteLine("No previous schema found. Creating initial schema...");
                ApplyInitialSchema(conn, newSnapshot);
                storage.SaveSnapshot(newSnapshot);
                Console.WriteLine("Initial schema created and saved!");
                return;
            }
            else if (oldSnapshot == null && DatabaseHasTables(conn))
            {
                Console.WriteLine("Warning: No previous schema snapshot found, but database has existing tables.");
                storage.SaveSnapshot(newSnapshot);
                return;
            }
            else if (oldSnapshot != null && !DatabaseHasTables(conn))
            {
                Console.WriteLine("Warning: Previous schema snapshot found, but database is empty. Applying initial schema from snapshot...");
                ApplyInitialSchema(conn, newSnapshot);
                storage.SaveSnapshot(newSnapshot);
                return;
            }

            Console.WriteLine("Comparing schemas...");
            var generator = new MigrationGenerator();
            var migrationSql = generator.GenerateMigrationSql(oldSnapshot, newSnapshot);

            Console.WriteLine($"Migracijska file: {migrationSql.ToString().Length}");

            if (string.IsNullOrWhiteSpace(migrationSql) ||
                migrationSql.Trim().StartsWith("-- Auto-generated") && migrationSql.Split('\n').Length <= 3)
            {
                Console.WriteLine("No schema changes detected.");
                return;
            }

            var migrationName = $"{DateTime.UtcNow:yyyyMMddHHmmss}_AutoMigration";
            SaveMigrationFile(migrationName, migrationSql);

            Console.WriteLine($"Applying migration: {migrationName}");
            ApplyMigration(conn, migrationName, migrationSql);

            storage.SaveSnapshot(newSnapshot);
            Console.WriteLine("Migration completed and snapshot updated!");
        }

        bool DatabaseHasTables(NpgsqlConnection conn)
        {
            using var cmd = new NpgsqlCommand(
                "SELECT COUNT(*) FROM information_schema.tables WHERE table_schema = 'public'",
                conn);

            var number = (long)cmd.ExecuteScalar();

            Console.WriteLine($"Exsisting tables in database: {number}");

            return number > 1;
        }


        private void CreateMigrationsTable(NpgsqlConnection conn)
        {
            var sql = @"
                CREATE TABLE IF NOT EXISTS __migrations (
                    id SERIAL PRIMARY KEY,
                    name VARCHAR(255) UNIQUE NOT NULL,
                    applied_at TIMESTAMP DEFAULT NOW()
                )";

            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.ExecuteNonQuery();
        }

        private void ApplyInitialSchema(NpgsqlConnection conn, SchemaSnapshot snapshot)
        {
            var generator = new MigrationGenerator();
            var emptySnapshot = new SchemaSnapshot();
            var sql = generator.GenerateMigrationSql(emptySnapshot, snapshot);

            using var transaction = conn.BeginTransaction();

            try
            {
                using var cmd = new NpgsqlCommand(sql, conn, transaction);
                cmd.ExecuteNonQuery(); /// tu kaze i ima i nema doctors ...

                var insertSql = "INSERT INTO __migrations (name) VALUES (@name)";
                using var insertCmd = new NpgsqlCommand(insertSql, conn, transaction);
                insertCmd.Parameters.AddWithValue("@name", "InitialCreate");
                insertCmd.ExecuteNonQuery();

                var seeder = new DatabaseSeeder(conn, transaction);
                seeder.SeedData();

                transaction.Commit();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                Console.WriteLine($"Initial schema creation failed: {ex.Message}");
                throw;
            }
        }

        private void SaveMigrationFile(string migrationName, string sql)
        {
            var migrationsDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Migrations", "Generated");
            Directory.CreateDirectory(migrationsDir);

            var filePath = Path.Combine(migrationsDir, $"{migrationName}.sql");
            File.WriteAllText(filePath, sql);
            Console.WriteLine($"Migration saved to: {filePath}");
        }

        private void ApplyMigration(NpgsqlConnection conn, string migrationName, string sql)
        {

            var checkSql = "SELECT COUNT(*) FROM __migrations WHERE name = @name";
            using var checkCmd = new NpgsqlCommand(checkSql, conn);
            checkCmd.Parameters.AddWithValue("@name", migrationName);
            var count = Convert.ToInt32(checkCmd.ExecuteScalar());

            if (count > 0)
            {
                Console.WriteLine($"Skipping {migrationName} (already applied)");
                return;
            }

            using var transaction = conn.BeginTransaction();
            try
            {

                using var cmd = new NpgsqlCommand(sql, conn, transaction);
                cmd.ExecuteNonQuery();

                var insertSql = "INSERT INTO __migrations (name) VALUES (@name)";
                using var insertCmd = new NpgsqlCommand(insertSql, conn, transaction);
                insertCmd.Parameters.AddWithValue("@name", migrationName);
                insertCmd.ExecuteNonQuery();

                transaction.Commit();
                Console.WriteLine($"Migration: {migrationName} applied successfully");
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                Console.WriteLine($"Migration {migrationName} failed: {ex.Message}");
                throw;
            }
        }
    }
}