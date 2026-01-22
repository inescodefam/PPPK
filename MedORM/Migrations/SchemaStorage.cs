using MedORM.Migrations.Models;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;


namespace MedORM.Migrations
{
    public class SchemaStorage
    {
        private readonly string _snapshotPath;

        public SchemaStorage(string snapshotPath = "schema_snapshot.json")
        {
            _snapshotPath = snapshotPath;
        }


        public SchemaSnapshot? LoadSnapshot()
        {
            if (!File.Exists(_snapshotPath))
                return null;

            try
            {
                var options = new JsonSerializerOptions
                {
                    TypeInfoResolver = new DefaultJsonTypeInfoResolver()
                };

                var json = File.ReadAllText(_snapshotPath);
                return JsonSerializer.Deserialize<SchemaSnapshot>(json, options);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading snapshot: {ex.Message}");
                return null;
            }
        }


        public void SaveSnapshot(SchemaSnapshot snapshot)
        {
            try
            {
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    TypeInfoResolver = new DefaultJsonTypeInfoResolver()
                };

                var json = JsonSerializer.Serialize(snapshot, options);
                File.WriteAllText(_snapshotPath, json);

                Console.WriteLine($"Schema snapshot saved to {_snapshotPath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving snapshot: {ex.Message}");
                throw;
            }
        }

    }
}
