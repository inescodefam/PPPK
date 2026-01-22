using System.ComponentModel.DataAnnotations;

namespace MedCore.Entities
{
    public class Prescription
    {

        [Key]
        public int Id { get; set; }
        public int TherapyId { get; set; }
        public int MedicationId { get; set; }
        public int DoctorId { get; set; }
        public string Dosage { get; set; } = string.Empty;
        public string Frequency { get; set; } = string.Empty;
        public DateTime PrescribedDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Instructions { get; set; } = string.Empty;


        public Therapy Therapy { get; set; } = null!;
        public Medication Medication { get; set; } = null!;
        public Doctor Doctor { get; set; } = null!;
    }
}
