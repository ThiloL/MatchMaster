using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace MatchMaster
{
    public class IntValidator : ValidationRule
    {
        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            if (string.IsNullOrWhiteSpace((string)value)) return new ValidationResult(false, "a value is required");

            int v = 0;

            if (!Int32.TryParse(value.ToString(), out v)) return new ValidationResult(false, "a numeric value is required");
            
            if ((v < 1) || (v > 999)) return new ValidationResult(false, "a numeric value between 0 and 1000 is required");

            return ValidationResult.ValidResult;
        }
    }
}
