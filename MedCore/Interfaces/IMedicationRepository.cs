using MedCore.Entities;

namespace MedCore.Interfaces
{
    public interface IMedicationRepository : IRepository<Medication>
    {
        Medication? GetByName(string name);
        List<Medication> SearchByName(string name);
        Medication? GetMedicationWithPrescriptions(int medicationId);
    }
}
