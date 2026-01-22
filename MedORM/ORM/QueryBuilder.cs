using Npgsql;
using System.Reflection;

namespace MedORM.ORM
{
    public class QueryBuilder
    {
       public static string? BuildDelete(object entity)
        {
            var entityType = entity.GetType();
            var tableName = GetTableName(entityType);
            var idProp = entityType.GetProperty("Id");
            var id = idProp?.GetValue(entity);

            return $"DELETE FROM {tableName} WHERE id = {id}";
        }


        public static string? BuildInsert(object entity)
        {
            var entityType = entity.GetType();
            var tableName = GetTableName(entityType);
            var properties = entityType.GetProperties()
                .Where(p => p.Name.ToLower() != "id" &&
                        (!p.PropertyType.IsClass || p.PropertyType == typeof(string)))
                .ToList();

            var columns = string.Join(", ", properties.Select(p => p.Name.ToLower()));
            var values = string.Join(", ", properties.Select(p => FormatValue(p.GetValue(entity))));

            return $"INSERT INTO {tableName} ({columns}) VALUES ({values})";
        }

        private static string FormatValue(object? value)
        {
            if (value == null) return "NULL";

            if (value is string str)
                return $"'{str.Replace("'", "''")}'";

            if (value is DateTime dateTime)
                return $"'{dateTime:yyyy-MM-dd HH:mm:ss}'";

            if (value is Enum enumValue)
                return Convert.ToInt32(enumValue).ToString();

            if (value is bool b)
                return b ? "TRUE" : "FALSE";

            return value.ToString() ?? "NULL";
        }

       public static string GetTableName(Type type)
        {
            return type.Name.ToLower() + "s";
        }


        public static string BuildUpdate(object entity)
        {
            var entityType = entity.GetType();
            var tableName = GetTableName(entityType);
            var idProp = entityType.GetProperty("Id");
            var id = idProp?.GetValue(entity);

            var properties = entityType.GetProperties()
                .Where(p => p.Name.ToLower() != "id" &&
                        (!p.PropertyType.IsClass || p.PropertyType == typeof(string)))
                .ToList();

            var setClauses = properties.Select(p =>
                $"{p.Name.ToLower()} = {FormatValue(p.GetValue(entity))}");

            return $"UPDATE {tableName} SET {string.Join(", ", setClauses)} WHERE id = {id}";
        }


        internal static T MapFromReader<T>(NpgsqlDataReader reader) where T : class
        {
            var entity = Activator.CreateInstance<T>();
            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var prop in properties)
            {

                if (prop.PropertyType.IsClass && prop.PropertyType != typeof(string))
                    continue;

                var columnName = prop.Name.ToLower();

                try
                {
                    var ordinal = reader.GetOrdinal(columnName);
                    if (!reader.IsDBNull(ordinal))
                    {
                        var value = reader.GetValue(ordinal);

                        if (prop.PropertyType.IsEnum)
                        {
                            var enumValue = Enum.Parse(prop.PropertyType, value.ToString()!);
                            prop.SetValue(entity, enumValue);
                        }
                        else
                        {
                            prop.SetValue(entity, value);
                        }
                    }
                }
                catch (IndexOutOfRangeException)
                {

                    Console.WriteLine($"Probably error while trying to parse enum. {columnName}");
                    continue;
                }
            }
            return entity;
        }

    }
}