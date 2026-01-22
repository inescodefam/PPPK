using MedCore.Entities;
using MedCore.Interfaces;
using MedORM.ORM;
using Npgsql;

namespace MedORM.Repo
{
    public class MedicalHistoryRepository : Repository<MedicalHistory>, IMedicalHistoryRepository
    {
        public MedicalHistoryRepository(MedDbContext context) : base(context) { }

        public List<MedicalHistory> GetByPatientId(int patientId)
        {
            var sql = "SELECT * FROM medicalhistorys WHERE patientid = @patientId";

            var result = new List<MedicalHistory>();

            var conn = _context.GetConnection();
            var cmd = new NpgsqlCommand(sql, conn);

            cmd.Parameters.AddWithValue("@patientId", patientId);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                result.Add(QueryBuilder.MapFromReader<MedicalHistory>(reader));
            }

            return result;
        }

        public List<MedicalHistory> SearchByDiseaseName(string diseaseName)
        {
            var sql = "SELECT * FROM medicalhistorys WHERE LOWER(diseasename) LIKE LOWER(@diseaseName)";

            var result = new List<MedicalHistory>();

            var conn = _context.GetConnection();
            var cmd = new NpgsqlCommand(sql, conn);

            cmd.Parameters.AddWithValue("@diseaseName", $"%{diseaseName}%");

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                result.Add(QueryBuilder.MapFromReader<MedicalHistory>(reader));
            }

            return result;
        }
    }
}