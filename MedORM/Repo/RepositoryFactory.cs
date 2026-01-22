using MedCore.Interfaces;
using MedORM.ORM;
using MedORM.Repo;

namespace MedCore
{
    public class RepositoryFactory : IRepositoryFactory
    {
        private readonly MedDbContext _context;

        public RepositoryFactory(MedDbContext context)
        {
            _context = context;
        }

        public Repository<T> CreateRepository<T>() where T : class
        {
            return new Repository<T>(_context);
        }

        public IPatientRepository CreatePatientRepository()
        {
            return new PatientRepository(_context);
        }

        public IDoctorRepository CreateDoctorRepository()
        {
            return new DoctorRepository(_context);
        }

        Interfaces.IRepository<T> IRepositoryFactory.CreateRepository<T>()
        {
            throw new NotImplementedException();
        }
    }
}
