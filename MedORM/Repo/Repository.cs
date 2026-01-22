using MedCore.Interfaces;
using MedORM.ORM;

namespace MedORM.Repo
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly MedDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public Repository(MedDbContext context)
        {
            _context = context;

            var entityName = typeof(T).Name;
            var pluralName = GetPluralName(entityName);
            var dbSetProperty = context.GetType().GetProperty(pluralName);
            _dbSet = (DbSet<T>)dbSetProperty!.GetValue(context)!;
        }

        private static string GetPluralName(string entityName)
        {
            if (entityName.EndsWith("y") && !entityName.EndsWith("ey") && !entityName.EndsWith("ay") && !entityName.EndsWith("oy"))
            {
                // Therapy -> Therapies, MedicalHistory -> MedicalHistories
                return entityName[..^1] + "ies";
            }
            return entityName + "s";// znam nije ovo dobro ali za sada ce morati
        }

        public virtual List<T> GetAll() => _dbSet.ToList();
        public virtual T? GetById(int id) => _dbSet.Find(id);

        public virtual void Add(T entity)
        {
            _dbSet.Add(entity);
        }

        public virtual void Update(T entity)
        {
            _dbSet.Update(entity);
        }

        public virtual void Delete(T entity)
        {
            _dbSet.Remove(entity);
        }

        public virtual void Save()
        {
            _context.SaveChanges();
        }
    }
}
