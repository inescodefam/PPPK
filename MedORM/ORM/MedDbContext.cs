using MedCore.Entities;
using Npgsql;

namespace MedORM.ORM
{
    public class MedDbContext : IUnitOfWork
    {
        private readonly string _connectionString;
        private readonly ChangeTracker _changeTracker;

        private NpgsqlConnection? _connection;
        private bool _disposed = false;

        public MedDbContext(string connectionString)
        {
            _connectionString = connectionString;
            _changeTracker = new ChangeTracker();
            InitializeDbSets();
        }

        public DbSet<Patient> Patients { get; private set; }
        public DbSet<Doctor> Doctors { get; private set; }
        public DbSet<Examination> Examinations { get; private set; }
        public DbSet<Therapy> Therapies { get; private set; }
        public DbSet<Prescription> Prescriptions { get; private set; }
        public DbSet<Medication> Medications { get; private set; }
        public DbSet<MedicalHistory> MedicalHistories { get; private set; }

        private void InitializeDbSets()
        {
            Patients = new DbSet<Patient>(this);
            Doctors = new DbSet<Doctor>(this);
            Examinations = new DbSet<Examination>(this);
            Therapies = new DbSet<Therapy>(this);
            Prescriptions = new DbSet<Prescription>(this);
            Medications = new DbSet<Medication>(this);
            MedicalHistories = new DbSet<MedicalHistory>(this);
        }

       public NpgsqlConnection GetConnection()
        {

            if (_connection == null)
            {
                _connection = new NpgsqlConnection(_connectionString);
                _connection.Open();
            }

            if (_connection.State == System.Data.ConnectionState.Closed)
            {
                _connection.Open();
            }

            return _connection;
        }

        public void Add<T>(T entity) where T : class
        {
            _changeTracker.TrackAdded(entity);
        }

        public void Update<T>(T entity) where T : class
        {
            _changeTracker.TrackModified(entity);
        }

        public void Remove<T>(T entity) where T : class
        {
            _changeTracker.TrackDeleted(entity);
        }

        public int SaveChanges()
        {
            int affectedRows = 0;

            var connection = GetConnection();
            using var transaction = connection.BeginTransaction();

            try
            {
                foreach (var entity in _changeTracker.GetAdded())
                {
                    using var cmd = new NpgsqlCommand(QueryBuilder.BuildInsert(entity), connection, transaction);
                    affectedRows += cmd.ExecuteNonQuery();
                }

                foreach (var entity in _changeTracker.GetModified())
                {
                    var sql = QueryBuilder.BuildUpdate(entity);
                    using var cmd = new NpgsqlCommand(sql, connection, transaction);
                    affectedRows += cmd.ExecuteNonQuery();
                }

                foreach (var entity in _changeTracker.GetDeleted())
                {
                    var sql = QueryBuilder.BuildDelete(entity);
                    using var cmd = new NpgsqlCommand(sql, connection, transaction);
                    affectedRows += cmd.ExecuteNonQuery();
                }

                transaction.Commit();
                _changeTracker.Clear();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }

            return affectedRows;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _connection?.Close();
                    _connection?.Dispose();
                    _connection = null;
                }
                _disposed = true;
            }
        }
    }
}
