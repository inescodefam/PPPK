using Npgsql;
using System.Collections;
using System.Reflection;

namespace MedORM.ORM
{
    public class DbSet<T> : IEnumerable<T> where T : class
    {
        private readonly MedDbContext _context;
        private readonly List<T> _localCache = new();

        public DbSet(MedDbContext context)
        {
            _context = context;
        }

        public List<T> ToList()
        {
            string tableName = QueryBuilder.GetTableName(typeof(T));
            Console.WriteLine($"Table name in ToList: {tableName} type T: {typeof(T)}  ");
            var sql = $"SELECT * FROM {tableName}";

            var result = new List<T>();
            var conn = _context.GetConnection();

            try
            {
                Console.WriteLine($"Executing SQL: {sql}");
                Console.WriteLine($"Using connection: {conn.ConnectionString}");
                Console.WriteLine($"Connection State: {conn.State}");
                Console.WriteLine($"Table name: {tableName}");
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    // cmd.CommandTimeout = 60;
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var entity = QueryBuilder.MapFromReader<T>(reader);
                            result.Add(entity);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error retrieving data from table {tableName}\n Exception: {e.Message} {e.StackTrace}");
            }

            return result;
        }

        public T? Find(int id)
        {
            var tableName = QueryBuilder.GetTableName(typeof(T));
            var sql = $"SELECT * FROM {tableName} WHERE id = @id";

            var conn = _context.GetConnection();

            using (var cmd = new NpgsqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@id", id);
                // cmd.CommandTimeout = 60;
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return QueryBuilder.MapFromReader<T>(reader);
                    }
                }
            }

            return null;
        }



        public List<T> Where(string condition)
        {
            var tableName = QueryBuilder.GetTableName(typeof(T));
            var sql = $"SELECT * FROM {tableName} WHERE {condition}";

            var result = new List<T>();
            var conn = _context.GetConnection();

            using (var cmd = new NpgsqlCommand(sql, conn))
            {
                // cmd.CommandTimeout = 60;
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var entity = QueryBuilder.MapFromReader<T>(reader);
                        result.Add(entity);
                    }
                }
            }

            return result;
        }

        public List<T> Include<TProperty>(string navigationProperty) where TProperty : class
        {
            var mainEntities = ToList();

            if (mainEntities.Count == 0)
                return mainEntities;

            var navProp = typeof(T).GetProperty(navigationProperty);
            if (navProp == null)
            {
                Console.WriteLine($"Navigation property '{navigationProperty}' not found on {typeof(T).Name}");
                return mainEntities;
            }

            var propType = navProp.PropertyType;

            if (propType.IsGenericType && propType.GetGenericTypeDefinition() == typeof(List<>))
            {
                LoadOneToManyRelation<TProperty>(mainEntities, navProp);
            }

            else if (propType == typeof(TProperty))
            {
                LoadManyToOneRelation<TProperty>(mainEntities, navProp);
            }

            return mainEntities;
        }


        private void LoadOneToManyRelation<TProperty>(List<T> mainEntities, PropertyInfo navProp) where TProperty : class
        {

            var relatedTableName = QueryBuilder.GetTableName(typeof(TProperty));
            var foreignKeyColumn = typeof(T).Name.ToLower() + "id";

            var idProp = typeof(T).GetProperty("Id");
            var ids = mainEntities.Select(e => idProp!.GetValue(e)).ToList();

            if (ids.Count == 0) return;

            var idList = string.Join(", ", ids);
            var sql = $"SELECT * FROM {relatedTableName} WHERE {foreignKeyColumn} IN ({idList})";

            Console.WriteLine($"SQL: {sql}");

            var conn = _context.GetConnection();
            var relatedEntities = new List<TProperty>();

            using (var cmd = new NpgsqlCommand(sql, conn))
            {
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var entity = QueryBuilder.MapFromReader<TProperty>(reader);
                        relatedEntities.Add(entity);
                    }
                }
            }

            var fkProp = typeof(TProperty).GetProperty(typeof(T).Name + "Id");

            foreach (var mainEntity in mainEntities)
            {
                var mainId = idProp!.GetValue(mainEntity);
                var related = relatedEntities
                    .Where(r => fkProp!.GetValue(r)!.Equals(mainId))
                    .ToList();

                var listType = typeof(List<>).MakeGenericType(typeof(TProperty));
                var list = Activator.CreateInstance(listType);
                var addMethod = listType.GetMethod("Add");

                foreach (var item in related)
                {
                    addMethod!.Invoke(list, new object[] { item });
                }

                navProp.SetValue(mainEntity, list);
            }

            Console.WriteLine($"Loaded {relatedEntities.Count} related {typeof(TProperty).Name} entities");
        }

        private void LoadManyToOneRelation<TProperty>(List<T> mainEntities, PropertyInfo navProp) where TProperty : class
        {

            var fkPropName = typeof(TProperty).Name + "Id";
            var fkProp = typeof(T).GetProperty(fkPropName);

            if (fkProp == null)
            {
                Console.WriteLine($"Foreign key property '{fkPropName}' not found on {typeof(T).Name}");
                return;
            }


            var fkValues = mainEntities
                .Select(e => fkProp.GetValue(e))
                .Where(v => v != null)
                .Distinct()
                .ToList();

            if (fkValues.Count == 0) return;

            var relatedTableName = QueryBuilder.GetTableName(typeof(TProperty));
            var idList = string.Join(", ", fkValues);
            var sql = $"SELECT * FROM {relatedTableName} WHERE id IN ({idList})";

            var conn = _context.GetConnection();
            var relatedEntities = new Dictionary<object, TProperty>();

            using (var cmd = new NpgsqlCommand(sql, conn))
            {
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var entity = QueryBuilder.MapFromReader<TProperty>(reader);
                        var idProp = typeof(TProperty).GetProperty("Id");
                        var id = idProp!.GetValue(entity)!;
                        relatedEntities[id] = entity;
                    }
                }
            }


            foreach (var mainEntity in mainEntities)
            {
                var fkValue = fkProp.GetValue(mainEntity);
                if (fkValue != null && relatedEntities.TryGetValue(fkValue, out var related))
                {
                    navProp.SetValue(mainEntity, related);
                }
            }

            Console.WriteLine($"Loaded {relatedEntities.Count} related {typeof(TProperty).Name} entities");
        }

        public void Add(T entity)
        {
            _context.Add(entity);
            _localCache.Add(entity);
        }

        public void Update(T entity)
        {
            _context.Update(entity);
        }

        public void Remove(T entity)
        {
            _context.Remove(entity);
            _localCache.Remove(entity);
        }

        public IEnumerator<T> GetEnumerator() => _localCache.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

}