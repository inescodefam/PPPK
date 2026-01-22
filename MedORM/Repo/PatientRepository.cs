using MedCore.Entities;
using MedCore.Interfaces;
using MedORM.ORM;
using Npgsql;

namespace MedORM.Repo
{
    public class PatientRepository : Repository<Patient>, IPatientRepository
    {
        public PatientRepository(MedDbContext context) : base(context) { }

        public Patient? GetByOIB(string oib)
        {
            var sql = "SELECT * FROM patients WHERE oib = @oib";

            var conn = _context.GetConnection();
            var cmd = new NpgsqlCommand(sql, conn);
            
                cmd.Parameters.AddWithValue("@oib", oib);

                using var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    return QueryBuilder.MapFromReader<Patient>(reader);
                }
            

            return null;
        }

        public List<Patient> SearchByLastName(string lastName)
        {
            var sql = "SELECT * FROM patients WHERE LOWER(lastname) LIKE LOWER(@lastName)";

            var result = new List<Patient>();

            var conn = _context.GetConnection();
            var cmd = new NpgsqlCommand(sql, conn);
            
                cmd.Parameters.AddWithValue("@lastName", $"%{lastName}%");

                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    result.Add(QueryBuilder.MapFromReader<Patient>(reader));
                }
            

            return result;
        }

        public Patient? GetPatientWithDetails(int patientId)
        {
            var patient = GetById(patientId);
            if (patient == null) return null;

            var historyRepo = new Repository<MedicalHistory>(_context);
            patient.MedicalHistories = historyRepo.GetAll()
                .FindAll(h => h.PatientId == patientId);

            var examRepo = new Repository<Examination>(_context);
            patient.Examinations = examRepo.GetAll()
                .FindAll(e => e.PatientId == patientId);

            var therapyRepo = new Repository<Therapy>(_context);
            var prescriptionRepo = new Repository<Prescription>(_context);

            patient.Therapies = therapyRepo.GetAll()
                .FindAll(t => t.PatientId == patientId);

            foreach (var therapy in patient.Therapies)
            {
                therapy.Prescriptions = prescriptionRepo.GetAll()
                    .FindAll(p => p.TherapyId == therapy.Id);
            }

            return patient;
        }

    }
}
