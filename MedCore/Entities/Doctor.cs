using MedCore.Attributes;
using MedCore.Enums;
using System.ComponentModel.DataAnnotations;

namespace MedCore.Entities
{
    public class Doctor
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        [MinLength(2)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        [MinLength(2)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [ColumnType(SqlTypes.Varchar)]
        [ValidEnum]
        public Specialization Specialization { get; set; }

        public List<Prescription> Prescriptions { get; set; } = new();
        public List<Examination> Examinations { get; set; } = new();
    }
}
