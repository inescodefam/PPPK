using System.ComponentModel.DataAnnotations;

namespace MedApi.DTOs
{
    public class MedicalHistoryDto
    {
        [Key]
        public int Id { get; set; }
        public int PatientId { get; set; }
        public string DiseaseName { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; } = null;
        public string Notes { get; set; } = string.Empty;
    }
}
