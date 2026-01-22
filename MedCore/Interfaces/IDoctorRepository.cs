using MedCore.Entities;

namespace MedCore.Interfaces
{
    public interface IDoctorRepository : IRepository<Doctor>
    {
        List<Doctor> GetBySpecialization(string specialization);
        Doctor? GetDoctorWithDetails(int doctorId);
    }
}
