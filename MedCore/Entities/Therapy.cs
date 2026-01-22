using System.ComponentModel.DataAnnotations;

namespace MedCore.Entities
{
    public class Therapy
    {

        [Key]
        public int Id { get; set; }
        public int PatientId { get; set; }
        public string TherapyName { get; set; } = string.Empty;
        public string Diagnosis { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; } = null;
        public bool IsActive { get; set; } = true;


        public Patient Patient { get; set; } = null!;
        public List<Prescription> Prescriptions { get; set; } = new();
    }
}
