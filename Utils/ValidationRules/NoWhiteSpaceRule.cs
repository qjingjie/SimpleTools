using System.Globalization;
using System.Windows.Controls;

namespace Utils.ValidationRules
{
    public class NoWhiteSpaceRule : ValidationRule
    {
        #region Constructor

        public NoWhiteSpaceRule()
        {
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Validates a input string to determine if a white space exist.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="cultureInfo"></param>
        /// <returns>ValidResult if no white space is found, error content otherwise.</returns>
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            ValidationResult result = ValidationResult.ValidResult;

            string text = (string)value;

            if (text.Length == 0)
            {
                result = new ValidationResult(false, "Field is required!");
            }

            if (text.Any(char.IsWhiteSpace))
            {
                result = new ValidationResult(false, "No spacing allowed!");
            }

            return result;
        }

        #endregion Methods
    }
}