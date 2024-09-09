using System.Globalization;
using System.Windows.Controls;

namespace SimpleTools.Utilities.ValidationRules
{
    public class NoWhiteSpaceStringRule : ValidationRule
    {
        #region Constructor

        public NoWhiteSpaceStringRule()
        {
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Overrides default validation.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="cultureInfo"></param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
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