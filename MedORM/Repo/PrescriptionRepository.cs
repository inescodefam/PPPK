using MedCore.Entities;
using MedCore.Interfaces;
using MedORM.ORM;
using Npgsql;

namespace MedORM.Repo
{
    public class PrescriptionRepository : Repository<Prescription>, IPrescriptionRepository
    {
        public PrescriptionRepository(MedDbContext context) : base(context) { }

        public List<Prescription> GetByTherapyId(int therapyId)
        {
            var sql = "SELECT * FROM prescriptions WHERE therapyid = @therapyId";

            var result = new List<Prescription>();

            var conn = _context.GetConnection();
            var cmd = new NpgsqlCommand(sql, conn);

            cmd.Parameters.AddWithValue("@therapyId", therapyId);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                result.Add(QueryBuilder.MapFromReader<Prescription>(reader));
            }

            return result;
        }

        public List<Prescription> GetByMedicationId(int medicationId)
        {
            var sql = "SELECT * FROM prescriptions WHERE medicationid = @medicationId";

            var result = new List<Prescription>();

            var conn = _context.GetConnection();
            var cmd = new NpgsqlCommand(sql, conn);

            cmd.Parameters.AddWithValue("@medicationId", medicationId);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                result.Add(QueryBuilder.MapFromReader<Prescription>(reader));
            }

            return result;
        }

        public List<Prescription> GetByDoctorId(int doctorId)
        {
            var sql = "SELECT * FROM prescriptions WHERE doctorid = @doctorId";

            var result = new List<Prescription>();

            var conn = _context.GetConnection();
            var cmd = new NpgsqlCommand(sql, conn);

            cmd.Parameters.AddWithValue("@doctorId", doctorId);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                result.Add(QueryBuilder.MapFromReader<Prescription>(reader));
            }

            return result;
        }

        public Prescription? GetPrescriptionWithDetails(int prescriptionId)
        {
            var prescription = GetById(prescriptionId);
            if (prescription == null) return null;

            var therapyRepo = new Repository<Therapy>(_context);
            prescription.Therapy = therapyRepo.GetById(prescription.TherapyId)!;

            var medicationRepo = new Repository<Medication>(_context);
            prescription.Medication = medicationRepo.GetById(prescription.MedicationId)!;

            var doctorRepo = new Repository<Doctor>(_context);
            prescription.Doctor = doctorRepo.GetById(prescription.DoctorId)!;

            return prescription;
        }
    }
}
