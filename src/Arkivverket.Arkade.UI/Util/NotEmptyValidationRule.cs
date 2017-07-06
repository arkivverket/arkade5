using System.Globalization;
using System.Windows.Controls;

namespace Arkivverket.Arkade.UI.Util
{
    class NotEmptyValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            return string.IsNullOrWhiteSpace((value ?? "").ToString())
                ? new ValidationResult(false, "Påkrevet felt.")
                : ValidationResult.ValidResult;
        }
    }
}
