using MedCore.Entities;

namespace MedCore.Interfaces
{
    public interface IPatientRepository : IRepository<Patient>
    {

        Patient? GetByOIB(string oib);
        List<Patient> SearchByLastName(string lastName);
        Patient? GetPatientWithDetails(int patientId);
    }
}
