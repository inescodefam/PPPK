namespace MedCore.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ColumnTypeAttribute : Attribute
    {
        public string TypeName { get; }

        public ColumnTypeAttribute(string typeName)
        {
            TypeName = typeName;
        }
    }

    public static class SqlTypes
    {
        public const string Int = "INTEGER";
        public const string BigInt = "BIGINT";
        public const string Float = "FLOAT";
        public const string Decimal = "DECIMAL";
        public const string Varchar = "VARCHAR";
        public const string Char = "CHAR";
        public const string Text = "TEXT";
        public const string TimestampWithTimeZone = "TIMESTAMP WITH TIME ZONE";
        public const string TimestampWithoutTimeZone = "TIMESTAMP WITHOUT TIME ZONE";
        public const string Boolean = "BOOLEAN";
    }
}
