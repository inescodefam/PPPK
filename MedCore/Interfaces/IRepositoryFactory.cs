namespace MedCore.Interfaces
{
    public interface IRepositoryFactory
    {
        IRepository<T> CreateRepository<T>() where T : class;
        IPatientRepository CreatePatientRepository();
        IDoctorRepository CreateDoctorRepository();

    }
}