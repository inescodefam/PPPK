using MedCore.Attributes;
using MedORM.Migrations.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace MedORM.Migrations
{
    public class EntityScanner
    {
        private readonly Assembly _assembly;

        public EntityScanner(Assembly assembly)
        {
            _assembly = assembly;
        }

        public SchemaSnapshot ScanEntities()
        {
            var snapshot = new SchemaSnapshot();

            var entityTypes = _assembly.GetTypes()
                .Where(t => t.Namespace == "MedCore.Entities" &&
                           t.IsClass &&
                           !t.IsAbstract)
                .ToList();

            Console.WriteLine($"Found {entityTypes.Count} entities to scan");

            foreach (var entityType in entityTypes)
            {
                var tableMetadata = ScanEntity(entityType);
                snapshot.Tables.Add(tableMetadata);
            }

            return snapshot;
        }

        private TableMetadata ScanEntity(Type entityType)
        {
            var tableName = GetTableName(entityType);

            var table = new TableMetadata
            {
                TableName = tableName,
                EntityName = entityType.Name
            };

            var properties = entityType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var prop in properties)
            {
                if (IsNavigationProperty(prop))
                {
                    var fkInfo = GetForeignKeyInfo(prop, entityType);
                    if (fkInfo != null)
                    {
                        table.ForeignKeys.Add(fkInfo);
                    }
                    continue;
                }

                var column = ScanProperty(prop, entityType);
                table.Columns.Add(column);
            }

            return table;
        }

        private ColumnMetadata ScanProperty(PropertyInfo prop, Type entityType)
        {
            var column = new ColumnMetadata
            {
                ColumnName = prop.Name.ToLower(),
                PropertyName = prop.Name,
                DataType = GetPostgreSqlType(prop),
                IsNullable = IsNullable(prop),
                IsPrimaryKey = IsPrimaryKey(prop),
                IsUnique = IsUnique(prop),
                MaxLength = GetMaxLength(prop),
                DefaultValue = GetDefaultValue(prop)
            };

            return column;
        }

        private string GetPostgreSqlType(PropertyInfo prop)
        {
            var columnTypeAttr = prop.GetCustomAttribute<ColumnTypeAttribute>();
            var maxLength = GetMaxLength(prop);
            if (columnTypeAttr != null)
            {
                var typeName = columnTypeAttr.TypeName;
                if (typeName == "CHAR" || typeName == "VARCHAR")
                {
                    if (maxLength.HasValue)
                    {
                        return $"{typeName}({maxLength.Value})";
                    }
                }
                return typeName;
            }

            var stringLength = prop.GetCustomAttribute<StringLengthAttribute>();
            var propType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;

            if (propType == typeof(string))
            {
                if (stringLength != null &&
                    stringLength.MinimumLength == stringLength.MaximumLength)
                {
                    return $"CHAR({stringLength.MaximumLength})";
                }

                if (maxLength.HasValue)
                {
                    return $"VARCHAR({maxLength.Value})";
                }
                return "TEXT";
            }

            return propType.Name switch
            {
                "Int32" => "INTEGER",
                "Int64" => "BIGINT",
                "Single" => "FLOAT",
                "Double" => "DOUBLE PRECISION",
                "Decimal" => "DECIMAL(18,2)",
                "Boolean" => "BOOLEAN",
                "DateTime" => "TIMESTAMP WITHOUT TIME ZONE",
                "Guid" => "UUID",
                _ when propType.IsEnum => "VARCHAR(50)",
                _ => "TEXT"
            };
        }

        private bool IsNullable(PropertyInfo prop)
        {
            if (prop.GetCustomAttribute<RequiredAttribute>() != null)
                return false;

            if (Nullable.GetUnderlyingType(prop.PropertyType) != null)
                return true;

            return !prop.PropertyType.IsValueType;
        }

        private bool IsPrimaryKey(PropertyInfo prop)
        {
            if (prop.GetCustomAttribute<KeyAttribute>() != null)
                return true;

            return prop.Name.Equals("Id", StringComparison.OrdinalIgnoreCase);
        }

        private bool IsUnique(PropertyInfo prop)
        {
            return prop.GetCustomAttribute<UniqueAttribute>() != null;
        }

        private int? GetMaxLength(PropertyInfo prop)
        {
            var maxLengthAttr = prop.GetCustomAttribute<MaxLengthAttribute>();
            if (maxLengthAttr != null)
                return maxLengthAttr.Length;

            var stringLengthAttr = prop.GetCustomAttribute<StringLengthAttribute>();
            if (stringLengthAttr != null)
                return stringLengthAttr.MaximumLength;

            return null;
        }

        private string? GetDefaultValue(PropertyInfo prop)
        {
            var defaultAttr = prop.GetCustomAttribute<DefaultValueAttribute>();
            if (defaultAttr != null)
            {
                var value = defaultAttr.Value;

                if (value is string strValue && strValue.Contains("("))
                    return strValue;

                if (value is string str)
                    return $"'{str}'";

                if (value is bool b)
                    return b ? "TRUE" : "FALSE";

                return value.ToString();
            }

            return null;
        }

        private string GetTableName(Type entityType)
        {
            var tableAttr = entityType.GetCustomAttribute<TableAttribute>();
            if (tableAttr != null && !string.IsNullOrEmpty(tableAttr.Name))
                return tableAttr.Name.ToLower();

            return entityType.Name.ToLower() + "s";
        }

        private bool IsNavigationProperty(PropertyInfo prop)
        {
            var propType = prop.PropertyType;

            if (propType.IsGenericType &&
                propType.GetGenericTypeDefinition() == typeof(List<>))
                return true;

            if (propType.IsClass &&
                propType != typeof(string) &&
                propType.Namespace == "MedCore.Entities")
                return true;

            return false;
        }

        private ForeignKeyMetadata? GetForeignKeyInfo(PropertyInfo prop, Type entityType)
        {
            var properties = entityType.GetProperties();

            foreach (var p in properties)
            {
                var fkAttr = p.GetCustomAttribute<ForeignKeyAttribute>();
                if (fkAttr != null && fkAttr.Name == prop.Name)
                {
                    var referencedType = prop.PropertyType;
                    if (referencedType.IsGenericType)
                        referencedType = referencedType.GetGenericArguments()[0];

                    return new ForeignKeyMetadata
                    {
                        ColumnName = p.Name.ToLower(),
                        ReferencedTable = GetTableName(referencedType),
                        ReferencedColumn = "id",
                        OnDelete = "CASCADE"
                    };
                }
            }

            return null;
        }
    }
}
