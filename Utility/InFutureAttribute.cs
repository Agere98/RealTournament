using System;
using System.ComponentModel.DataAnnotations;

namespace RealTournament.Utility
{
    public class InFutureAttribute : ValidationAttribute {

        protected override ValidationResult IsValid(object objValue, ValidationContext validationContext) {
            var dateValue = objValue as DateTime?;
            if (dateValue == null) {
                if (objValue is DateTimeOffset temp) {
                    dateValue = temp.DateTime;
                }
            }
            if (dateValue.HasValue && dateValue.Value <= DateTime.Now) {
                return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
            }
            return ValidationResult.Success;
        }
    }
}
