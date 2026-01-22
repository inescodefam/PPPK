using MedCore.Entities;

namespace MedCore.Interfaces
{
    public interface ITherapyRepository : IRepository<Therapy>
    {
        List<Therapy> GetByPatientId(int patientId);
        List<Therapy> GetActiveTherapies();
        Therapy? GetTherapyWithPrescriptions(int therapyId);
    }
}
