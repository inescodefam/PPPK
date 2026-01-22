using MedORM.Migrations.Models;
using System.Text;

namespace MedORM.Migrations
{
    public class MigrationGenerator
    {
        public string GenerateMigrationSql(
            SchemaSnapshot oldSchema,
            SchemaSnapshot newSchema)
        {
            var sql = new StringBuilder();
            sql.AppendLine("-- Auto-generated migration");
            sql.AppendLine($"-- Generated at: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}");
            sql.AppendLine();

            var newTables = newSchema.Tables
                .Where(t => !oldSchema.Tables.Any(o => o.TableName == t.TableName))
                .ToList();

            foreach (var table in newTables)
            {
                sql.AppendLine(GenerateCreateTableSql(table));
                sql.AppendLine();
            }

            foreach (var table in newTables)
            {
                foreach (var fk in table.ForeignKeys)
                {
                    sql.AppendLine(GenerateAddForeignKeySql(table.TableName, fk));
                }
            }

            var deletedTables = oldSchema.Tables
                .Where(t => !newSchema.Tables.Any(n => n.TableName == t.TableName))
                .ToList();

            foreach (var table in deletedTables)
            {
                sql.AppendLine($"DROP TABLE IF EXISTS {table.TableName} CASCADE;");
                sql.AppendLine();
            }

            var existingTables = newSchema.Tables
                .Where(t => oldSchema.Tables.Any(o => o.TableName == t.TableName))
                .ToList();

            foreach (var newTable in existingTables)
            {
                var oldTable = oldSchema.Tables.First(o => o.TableName == newTable.TableName);
                var alterSql = GenerateAlterTableSql(oldTable, newTable);

                if (!string.IsNullOrWhiteSpace(alterSql))
                {
                    sql.AppendLine(alterSql);
                    sql.AppendLine();
                }
            }

            return sql.ToString();
        }

        private string GenerateCreateTableSql(TableMetadata table)
        {
            var sql = new StringBuilder();
            sql.AppendLine($"CREATE TABLE IF NOT EXISTS {table.TableName} (");

            var columns = table.Columns
                         .Select(c => $"    {GenerateColumnDefinition(c)}");

            sql.AppendLine(string.Join(",\n", columns));
            sql.AppendLine(");");

            return sql.ToString();
        }

        private string GenerateAlterTableSql(TableMetadata oldTable, TableMetadata newTable)
        {
            var sql = new StringBuilder();


            var newColumns = newTable.Columns
                .Where(c => !oldTable.Columns.Any(o => o.ColumnName == c.ColumnName))
                .ToList();

            foreach (var col in newColumns)
            {
                sql.AppendLine($"ALTER TABLE {newTable.TableName} " +
                              $"ADD COLUMN {GenerateColumnDefinition(col)};");
            }

            var deletedColumns = oldTable.Columns
                .Where(c => !newTable.Columns.Any(n => n.ColumnName == c.ColumnName))
                .ToList();

            foreach (var col in deletedColumns)
            {
                sql.AppendLine($"ALTER TABLE {newTable.TableName} " +
                              $"DROP COLUMN IF EXISTS {col.ColumnName};");
            }

            var modifiedColumns = newTable.Columns
                .Where(n => oldTable.Columns.Any(o =>
                    o.ColumnName == n.ColumnName &&
                    HasColumnChanged(o, n)))
                .ToList();

            foreach (var newCol in modifiedColumns)
            {
                var oldCol = oldTable.Columns.First(o => o.ColumnName == newCol.ColumnName);
                sql.AppendLine(GenerateAlterColumnSql(newTable.TableName, oldCol, newCol));
            }


            var newFKs = newTable.ForeignKeys
                .Where(f => !oldTable.ForeignKeys.Any(o =>
                    o.ColumnName == f.ColumnName &&
                    o.ReferencedTable == f.ReferencedTable))
                .ToList();

            foreach (var fk in newFKs)
            {
                sql.AppendLine(GenerateAddForeignKeySql(newTable.TableName, fk));
            }

            var deletedFKs = oldTable.ForeignKeys
                .Where(f => !newTable.ForeignKeys.Any(n =>
                    n.ColumnName == f.ColumnName &&
                    n.ReferencedTable == f.ReferencedTable))
                .ToList();

            foreach (var fk in deletedFKs)
            {
                var fkName = $"fk_{oldTable.TableName}_{fk.ColumnName}";
                sql.AppendLine($"ALTER TABLE {oldTable.TableName} " +
                              $"DROP CONSTRAINT IF EXISTS {fkName};");
            }

            return sql.ToString();
        }

        private bool HasColumnChanged(ColumnMetadata oldCol, ColumnMetadata newCol)
        {
            return oldCol.DataType != newCol.DataType ||
                   oldCol.IsNullable != newCol.IsNullable ||
                   oldCol.MaxLength != newCol.MaxLength ||
                   oldCol.IsUnique != newCol.IsUnique;
        }

        private string GenerateAlterColumnSql(
            string tableName,
            ColumnMetadata oldCol,
            ColumnMetadata newCol)
        {
            var sql = new StringBuilder();

            if (oldCol.DataType != newCol.DataType || oldCol.MaxLength != newCol.MaxLength)
            {
                var newType = newCol.MaxLength.HasValue && newCol.DataType == "TEXT"
                    ? $"VARCHAR({newCol.MaxLength})"
                    : newCol.DataType;

                sql.AppendLine($"ALTER TABLE {tableName} " +
                              $"ALTER COLUMN {newCol.ColumnName} TYPE {newType} " +
                              $"USING {newCol.ColumnName}::{newType};");
            }

            if (oldCol.IsNullable != newCol.IsNullable)
            {
                var constraint = newCol.IsNullable ? "DROP NOT NULL" : "SET NOT NULL";
                sql.AppendLine($"ALTER TABLE {tableName} " +
                              $"ALTER COLUMN {newCol.ColumnName} {constraint};");
            }

            return sql.ToString();
        }

        private string GenerateAddForeignKeySql(string tableName, ForeignKeyMetadata fk)
        {
            var fkName = $"fk_{tableName}_{fk.ColumnName}";
            return $"ALTER TABLE {tableName} " +
                   $"ADD CONSTRAINT {fkName} " +
                   $"FOREIGN KEY ({fk.ColumnName}) " +
                   $"REFERENCES {fk.ReferencedTable}({fk.ReferencedColumn}) " +
                   $"ON DELETE {fk.OnDelete};";
        }

        private string GenerateColumnDefinition(ColumnMetadata col)
        {
            var def = new StringBuilder();
            def.Append(col.ColumnName);
            def.Append(" ");


            if (col.MaxLength.HasValue && col.DataType == "TEXT")
            {
                def.Append($"VARCHAR({col.MaxLength})");
            }
            else
            {
                def.Append(col.DataType);
            }

            if (col.IsPrimaryKey)
            {
                if (col.DataType == "INTEGER")
                {
                    def.Clear();
                    def.Append($"{col.ColumnName} SERIAL PRIMARY KEY");
                }
                else
                {
                    def.Append(" PRIMARY KEY");
                }
            }

            else if (!col.IsNullable)
            {
                def.Append(" NOT NULL");
            }


            if (col.IsUnique && !col.IsPrimaryKey)
            {
                def.Append(" UNIQUE");
            }

            if (!string.IsNullOrEmpty(col.DefaultValue))
            {
                def.Append($" DEFAULT {col.DefaultValue}");
            }

            return def.ToString();
        }
    }
}
