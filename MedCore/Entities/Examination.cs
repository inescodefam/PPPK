using MedCore.Attributes;
using MedCore.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedCore.Entities
{
    public class Examination
    {

        [Key]
        public int Id { get; set; }
        [Required]
        [ForeignKey("Patient")]
        public int PatientId { get; set; }
        public int DoctorId { get; set; }

        [ValidEnum]
        [Required]
        public ExaminationType ExaminationType { get; set; }

        [Required]
        [Column(TypeName = "timestamp without time zone")]
        public DateTime ScheduledDateTime { get; set; }

        [System.ComponentModel.DefaultValue(ExaminationStatus.Scheduled)]
        [ValidEnum]
        public ExaminationStatus Status { get; set; } = ExaminationStatus.Scheduled;

        [ColumnType("text")]
        public string Findings { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;


        public Patient Patient { get; set; } = null!;
        public Doctor Doctor { get; set; } = null!;
    }
}
