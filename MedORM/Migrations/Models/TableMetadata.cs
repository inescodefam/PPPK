namespace MedORM.Migrations.Models
{
    public class TableMetadata
    {
        public string TableName { get; set; } = string.Empty;
        public string EntityName { get; set; } = string.Empty;
        public List<ColumnMetadata> Columns { get; set; } = new();
        public List<ForeignKeyMetadata> ForeignKeys { get; set; } = new();
    }
}
