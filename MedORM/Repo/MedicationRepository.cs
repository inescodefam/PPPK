using MedCore.Entities;
using MedCore.Interfaces;
using MedORM.ORM;
using Npgsql;

namespace MedORM.Repo
{
    public class MedicationRepository : Repository<Medication>, IMedicationRepository
    {
        public MedicationRepository(MedDbContext context) : base(context) { }

        public Medication? GetByName(string name)
        {
            var sql = "SELECT * FROM medications WHERE LOWER(name) = LOWER(@name)";

            var conn = _context.GetConnection();
            var cmd = new NpgsqlCommand(sql, conn);

            cmd.Parameters.AddWithValue("@name", name);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return QueryBuilder.MapFromReader<Medication>(reader);
            }

            return null;
        }

        public List<Medication> SearchByName(string name)
        {
            var sql = "SELECT * FROM medications WHERE LOWER(name) LIKE LOWER(@name)";

            var result = new List<Medication>();

            var conn = _context.GetConnection();
            var cmd = new NpgsqlCommand(sql, conn);

            cmd.Parameters.AddWithValue("@name", $"%{name}%");

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                result.Add(QueryBuilder.MapFromReader<Medication>(reader));
            }

            return result;
        }

        public Medication? GetMedicationWithPrescriptions(int medicationId)
        {
            var medication = GetById(medicationId);
            if (medication == null) return null;

            var prescriptionRepo = new Repository<Prescription>(_context);
            medication.Prescriptions = prescriptionRepo.GetAll()
                .FindAll(p => p.MedicationId == medicationId);

            return medication;
        }
    }
}
