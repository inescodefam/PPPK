namespace MedORM.Migrations.Models
{
    public class SchemaSnapshot
    {
        public string Version { get; set; } = "1.0";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public List<TableMetadata> Tables { get; set; } = new();
    }

}
