using MedCore.Entities;

namespace MedCore.Interfaces
{
    public interface IMedicalHistoryRepository : IRepository<MedicalHistory>
    {
        List<MedicalHistory> GetByPatientId(int patientId);
        List<MedicalHistory> SearchByDiseaseName(string diseaseName);
    }
}
