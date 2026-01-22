namespace MedORM.Migrations.Models
{
    public class ColumnMetadata
    {
        public string ColumnName { get; set; } = string.Empty;
        public string PropertyName { get; set; } = string.Empty;
        public string DataType { get; set; } = string.Empty;
        public bool IsNullable { get; set; }
        public bool IsPrimaryKey { get; set; }
        public bool IsUnique { get; set; }
        public int? MaxLength { get; set; }
        public string? DefaultValue { get; set; }
    }
}
