using MedCore.Attributes;
using MedCore.Enums;
using System.ComponentModel.DataAnnotations;

namespace MedApi.DTOs
{

    public class DoctorBaseDto
    {
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
        public Specialization Specialization { get; set; }
    }

    public class DoctorDto : DoctorBaseDto
    {
        [Key]
        public int Id { get; set; }
    }
}
