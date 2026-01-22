using MedCore.Entities;

namespace MedORM.ORM
{
    public interface IUnitOfWork : IDisposable
    {
        int SaveChanges();

        DbSet<Patient> Patients { get; }
        DbSet<Doctor> Doctors { get; }
        DbSet<Examination> Examinations { get; }
        DbSet<Therapy> Therapies { get; }
        DbSet<Prescription> Prescriptions { get; }
        DbSet<Medication> Medications { get; }
        DbSet<MedicalHistory> MedicalHistories { get; }
    }
}
