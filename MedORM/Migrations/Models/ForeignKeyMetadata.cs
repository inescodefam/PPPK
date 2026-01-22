namespace MedORM.Migrations.Models
{
    public class ForeignKeyMetadata
    {
        public string ColumnName { get; set; } = string.Empty;
        public string ReferencedTable { get; set; } = string.Empty;
        public string ReferencedColumn { get; set; } = "id";
        public string OnDelete { get; set; } = "CASCADE";
    }
}
