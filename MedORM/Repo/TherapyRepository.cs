using MedCore.Entities;
using MedCore.Interfaces;
using MedORM.ORM;
using Npgsql;

namespace MedORM.Repo
{
    public class TherapyRepository : Repository<Therapy>, ITherapyRepository
    {
        public TherapyRepository(MedDbContext context) : base(context) { }

        public List<Therapy> GetByPatientId(int patientId)
        {
            var sql = "SELECT * FROM therapys WHERE patientid = @patientId";

            var result = new List<Therapy>();

            var conn = _context.GetConnection();
            var cmd = new NpgsqlCommand(sql, conn);

            cmd.Parameters.AddWithValue("@patientId", patientId);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                result.Add(QueryBuilder.MapFromReader<Therapy>(reader));
            }

            return result;
        }

        public List<Therapy> GetActiveTherapies()
        {
            var sql = "SELECT * FROM therapys WHERE isactive = true";

            var result = new List<Therapy>();

            var conn = _context.GetConnection();
            var cmd = new NpgsqlCommand(sql, conn);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                result.Add(QueryBuilder.MapFromReader<Therapy>(reader));
            }

            return result;
        }

        public Therapy? GetTherapyWithPrescriptions(int therapyId)
        {
            var therapy = GetById(therapyId);
            if (therapy == null) return null;

            var prescriptionRepo = new Repository<Prescription>(_context);
            therapy.Prescriptions = prescriptionRepo.GetAll()
                .FindAll(p => p.TherapyId == therapyId);

            var patientRepo = new Repository<Patient>(_context);
            therapy.Patient = patientRepo.GetById(therapy.PatientId)!;

            return therapy;
        }
    }
}
