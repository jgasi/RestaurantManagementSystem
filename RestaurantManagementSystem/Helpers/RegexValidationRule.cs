using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace RestaurantManagementSystem.Helpers
{
    public class RegexValidationRule : ValidationRule
    {
        public string Pattern { get; set; }
        public string ErrorMessage { get; set; }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                return new ValidationResult(false, ErrorMessage);
            }

            var input = value.ToString();
            if (Regex.IsMatch(input, Pattern))
            {
                return ValidationResult.ValidResult;
            }
            else
            {
                return new ValidationResult(false, ErrorMessage);
            }
        }
    }
}
