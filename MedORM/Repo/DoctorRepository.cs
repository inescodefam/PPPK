using MedCore.Entities;
using MedCore.Interfaces;
using MedORM.ORM;
using Npgsql;

namespace MedORM.Repo
{
    public class DoctorRepository : Repository<Doctor>, IDoctorRepository
    {
        public DoctorRepository(MedDbContext context) : base(context) { }

        public List<Doctor> GetBySpecialization(string specialization)
        {
            var sql = "SELECT * FROM doctors WHERE LOWER(specialization) LIKE LOWER(@specialization)";

            var result = new List<Doctor>();

            var conn = _context.GetConnection();
            var cmd = new NpgsqlCommand(sql, conn);
            
                cmd.Parameters.AddWithValue("@specialization", $"%{specialization}%");
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    result.Add(QueryBuilder.MapFromReader<Doctor>(reader));
                }
            

            return result;
        }

        public Doctor? GetDoctorWithDetails(int doctorId)
        {
            var doctor = GetById(doctorId);
            if (doctor == null) return null;

            var examRepo = new Repository<Examination>(_context);
            doctor.Examinations = examRepo.GetAll()
                .FindAll(e => e.DoctorId == doctorId);

            var prescriptionRepo = new Repository<Prescription>(_context);
            doctor.Prescriptions = prescriptionRepo.GetAll()
                .FindAll(p => p.DoctorId == doctorId);

            return doctor;
        }

      
    }
}
