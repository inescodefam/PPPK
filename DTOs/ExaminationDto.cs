using MedCore.Attributes;
using MedCore.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedApi.DTOs
{
    public class ExaminationDto
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [ForeignKey("Patient")]
        public int PatientId { get; set; }
        public int DoctorId { get; set; }
        [Required]
        public ExaminationType ExaminationType { get; set; }

        [Required]
        [Column(TypeName = "timestamp without time zone")]
        public DateTime ScheduledDateTime { get; set; }

        [System.ComponentModel.DefaultValue(ExaminationStatus.Scheduled)]
        public ExaminationStatus Status { get; set; } = ExaminationStatus.Scheduled;

        [ColumnType("text")]
        public string Findings { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
    }
}
