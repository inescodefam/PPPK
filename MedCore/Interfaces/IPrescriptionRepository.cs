using MedCore.Entities;

namespace MedCore.Interfaces
{
    public interface IPrescriptionRepository : IRepository<Prescription>
    {
        List<Prescription> GetByTherapyId(int therapyId);
        List<Prescription> GetByMedicationId(int medicationId);
        List<Prescription> GetByDoctorId(int doctorId);
        Prescription? GetPrescriptionWithDetails(int prescriptionId);
    }
}
