using MedCore.Attributes;
using System.ComponentModel.DataAnnotations;

namespace MedApi.DTOs
{
    public class MedicationDto
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Unique]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(200)]
        public string ActiveIngredient { get; set; } = string.Empty;

        [MaxLength(200)]
        public string Manufacturer { get; set; } = string.Empty;

        [MaxLength(100)]
        public string Form { get; set; } = string.Empty;
    }
}
