using Npgsql;

namespace MedORM.Migrations
{
    public class DatabaseSeeder
    {
        private readonly NpgsqlConnection _connection;
        private readonly NpgsqlTransaction _transaction;

        public DatabaseSeeder(NpgsqlConnection connection, NpgsqlTransaction transaction)
        {
            _connection = connection;
            _transaction = transaction;
        }

        public void SeedData()
        {
            Console.WriteLine("Seeding initial data...");

            SeedDoctors();
            SeedPatients();
            SeedMedications();
            SeedMedicalHistories();
            SeedTherapies();
            SeedExaminations();
            SeedPrescriptions();

            Console.WriteLine("Initial data seeded successfully!");
        }

        private void SeedDoctors()
        {
            var sql = @"
                INSERT INTO doctors (firstname, lastname, specialization) VALUES
                ('Ivan', 'Horvat', 'Surgery'),
                ('Ana', 'Kovačević', 'Internal_Medicine'),
                ('Marko', 'Babić', 'Pediatrics')
                ON CONFLICT DO NOTHING;
            ";
            ExecuteSql(sql);
        }

        private void SeedPatients()
        {
            var sql = @"
                INSERT INTO patients (firstname, lastname, oib, dateofbirth, gender, residenceaddress, permanentaddress) VALUES
                ('Petra', 'Novak', '12345678901', '1985-03-15', 0, 'Ilica 42, Zagreb', 'Ilica 42, Zagreb'),
                ('Tomislav', 'Jurić', '23456789012', '1990-07-22', 0, 'Vukovarska 15, Split', 'Vukovarska 15, Split'),
                ('Maja', 'Knežević', '34567890123', '1978-11-08', 1, 'Radnička 8, Rijeka', 'Radnička 8, Rijeka')
                ON CONFLICT DO NOTHING;
            ";
            ExecuteSql(sql);
        }

        private void SeedMedications()
        {
            var sql = @"
                INSERT INTO medications (name, activeingredient, manufacturer, form) VALUES
                ('Ibuprofen 400mg', 'Ibuprofen', 'Belupo', 'Tablet'),
                ('Amoxicillin 500mg', 'Amoxicillin', 'Pliva', 'Capsule'),
                ('Metformin 850mg', 'Metformin hydrochloride', 'Genera', 'Tablet')
                ON CONFLICT DO NOTHING;
            ";
            ExecuteSql(sql);
        }

        private void SeedMedicalHistories()
        {
            var sql = @"
                INSERT INTO medicalhistorys (patientid, diseasename, startdate, enddate, notes) VALUES
                (1, 'Hypertension', '2020-01-10', NULL, 'Controlled with medication'),
                (1, 'Appendicitis', '2018-05-20', '2018-06-15', 'Appendectomy performed successfully'),
                (2, 'Type 2 Diabetes', '2019-08-05', NULL, 'Diet controlled, regular monitoring'),
                (2, 'Fractured wrist', '2022-03-12', '2022-05-01', 'Healed completely'),
                (3, 'Asthma', '2015-02-28', NULL, 'Mild, uses inhaler as needed'),
                (3, 'Seasonal allergies', '2010-04-01', NULL, 'Spring pollen allergy')
                ON CONFLICT DO NOTHING;
            ";
            ExecuteSql(sql);
        }

        private void SeedTherapies()
        {
            var sql = @"
                INSERT INTO therapys (patientid, therapyname, diagnosis, startdate, enddate, isactive) VALUES
                (1, 'Blood Pressure Management', 'Essential Hypertension', '2020-01-15', NULL, true),
                (2, 'Diabetes Management', 'Type 2 Diabetes Mellitus', '2019-08-10', NULL, true),
                (3, 'Asthma Control Therapy', 'Mild Persistent Asthma', '2015-03-01', NULL, true)
                ON CONFLICT DO NOTHING;
            ";
            ExecuteSql(sql);
        }

        private void SeedExaminations()
        {
            var sql = @"
                INSERT INTO examinations (patientid, doctorid, examinationtype, scheduleddatetime, status, findings, notes) VALUES
                (1, 2, 'GP', '2024-01-15 09:00:00', 'Completed', 'Blood pressure 140/90, recommended lifestyle changes', 'Follow-up in 3 months'),
                (1, 1, 'XRAY', '2024-02-10 14:30:00', 'Completed', 'Chest X-ray clear, no abnormalities', 'Routine checkup'),
                (2, 2, 'KRV', '2024-01-20 08:00:00', 'Completed', 'HbA1c 6.8%, glucose levels stable', 'Continue current medication'),
                (2, 3, 'GP', '2024-03-05 10:00:00', 'Scheduled', '', 'Annual checkup'),
                (3, 2, 'GP', '2024-02-28 11:00:00', 'Completed', 'Lungs clear, asthma well controlled', 'Continue inhaler as needed'),
                (3, 1, 'XRAY', '2024-03-15 15:00:00', 'Scheduled', '', 'Follow-up chest imaging')
                ON CONFLICT DO NOTHING;
            ";
            ExecuteSql(sql);
        }

        private void SeedPrescriptions()
        {
            var sql = @"
                INSERT INTO prescriptions (therapyid, medicationid, doctorid, dosage, frequency, prescribeddate, enddate, instructions) VALUES
                (1, 1, 2, '400mg', 'Twice daily', '2020-01-15', NULL, 'Take with food, monitor blood pressure weekly'),
                (2, 3, 2, '850mg', 'Once daily', '2019-08-10', NULL, 'Take with breakfast, check glucose levels regularly'),
                (3, 2, 2, '500mg', 'As needed', '2015-03-01', NULL, 'Use during asthma flare-ups, max 3 times per day')
                ON CONFLICT DO NOTHING;
            ";
            ExecuteSql(sql);
        }

        private void ExecuteSql(string sql)
        {
            using var cmd = new NpgsqlCommand(sql, _connection, _transaction);
            cmd.ExecuteNonQuery();
        }
    }
}
