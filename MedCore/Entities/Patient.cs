using MedCore.Attributes;
using MedCore.Enums;
using System.ComponentModel.DataAnnotations;

namespace MedCore.Entities
{
    public class Patient
    {

        private string _oib = string.Empty;

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
        [Unique]
        [StringLength(11, MinimumLength = 11)]
        [ColumnType(SqlTypes.Char)]
        public string OIB
        {
            get => _oib;
            set
            {
                if (value.Length != 11)
                    throw new ArgumentException("OIB must be exactly 11 characters");

                _oib = value;
            }
        }

        [Required]
        [ColumnType(SqlTypes.TimestampWithoutTimeZone)]
        public DateTime DateOfBirth { get; set; }

        [Required]
        [ValidEnum]
        public Gender Gender { get; set; }

        [Required]
        [MaxLength(300)]
        public string ResidenceAddress { get; set; } = string.Empty;

        [MaxLength(300)]
        public string PermanentAddress { get; set; } = string.Empty;

        public List<MedicalHistory> MedicalHistories { get; set; } = new();
        public List<Prescription> Prescriptions { get; set; } = new();
        public List<Examination> Examinations { get; set; } = new();

        public List<Therapy> Therapies { get; set; } = new();
    }
}
