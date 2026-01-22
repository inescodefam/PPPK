using MedCore.Entities;

namespace MedCore.Interfaces
{
    public interface IExaminationRepository : IRepository<Examination>
    {
        List<Examination> GetByPatientId(int patientId);
        List<Examination> GetByDoctorId(int doctorId);
        List<Examination> GetByStatus(string status);
        Examination? GetExaminationWithDetails(int examinationId);
    }
}
