using MedCore.Entities;
using MedCore.Enums;
using MedCore.Interfaces;
using MedORM.ORM;
using Npgsql;

namespace MedORM.Repo
{
    public class ExaminationRepository : Repository<Examination>, IExaminationRepository
    {
        public ExaminationRepository(MedDbContext context) : base(context) { }

        public List<Examination> GetByPatientId(int patientId)
        {
            var sql = "SELECT * FROM examinations WHERE patientid = @patientId";

            var result = new List<Examination>();

            var conn = _context.GetConnection();
            var cmd = new NpgsqlCommand(sql, conn);

            cmd.Parameters.AddWithValue("@patientId", patientId);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                result.Add(QueryBuilder.MapFromReader<Examination>(reader));
            }

            return result;
        }

        public List<Examination> GetByDoctorId(int doctorId)
        {
            var sql = "SELECT * FROM examinations WHERE doctorid = @doctorId";

            var result = new List<Examination>();

            var conn = _context.GetConnection();
            var cmd = new NpgsqlCommand(sql, conn);

            cmd.Parameters.AddWithValue("@doctorId", doctorId);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                result.Add(QueryBuilder.MapFromReader<Examination>(reader));
            }

            return result;
        }

        public List<Examination> GetByStatus(string status)
        {
            var sql = "SELECT * FROM examinations WHERE LOWER(status) = LOWER(@status)";

            var result = new List<Examination>();

            var conn = _context.GetConnection();
            var cmd = new NpgsqlCommand(sql, conn);

            cmd.Parameters.AddWithValue("@status", status);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                result.Add(QueryBuilder.MapFromReader<Examination>(reader));
            }

            return result;
        }

        public Examination? GetExaminationWithDetails(int examinationId)
        {
            var examination = GetById(examinationId);
            if (examination == null) return null;

            var patientRepo = new Repository<Patient>(_context);
            examination.Patient = patientRepo.GetById(examination.PatientId)!;

            var doctorRepo = new Repository<Doctor>(_context);
            examination.Doctor = doctorRepo.GetById(examination.DoctorId)!;

            return examination;
        }
    }
}
