using System.ComponentModel.DataAnnotations;

namespace MedApi.DTOs
{
    public class TherapyDto
    {
        [Key]
        public int Id { get; set; }
        public int PatientId { get; set; }
        public string TherapyName { get; set; } = string.Empty;
        public string Diagnosis { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; } = null;
        public bool IsActive { get; set; } = true;
    }
}
