using System.ComponentModel.DataAnnotations;

namespace MedCore.Attributes
{

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class ValidEnumAttribute : ValidationAttribute
    {
        public ValidEnumAttribute() : base("The {0} field must have a valid value.")
        {
        }

        public override bool IsValid(object? value)
        {
            if (value == null)
            {
                return true;
            }

            var enumType = value.GetType();

            if (!enumType.IsEnum)
            {
                return false;
            }

            return Enum.IsDefined(enumType, value);
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (!IsValid(value))
            {
                var enumType = value?.GetType();
                var validValues = enumType != null && enumType.IsEnum
                    ? string.Join(", ", Enum.GetNames(enumType))
                    : "N/A";

                var errorMessage = string.Format(
                    ErrorMessageString,
                    validationContext.DisplayName
                ) + $" Valid values are: {validValues}";

                return new ValidationResult(errorMessage, new[] { validationContext.MemberName ?? string.Empty });
            }

            return ValidationResult.Success;
        }
    }
}
