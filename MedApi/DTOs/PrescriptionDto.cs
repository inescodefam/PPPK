using System.ComponentModel.DataAnnotations;

namespace MedApi.DTOs
{
    public class PrescriptionDto
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
    }
}
