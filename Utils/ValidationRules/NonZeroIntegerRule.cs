using System.Globalization;
using System.Windows.Controls;

namespace Utils.ValidationRules
{
    public class NonZeroIntegerRule : ValidationRule
    {
        #region Constructor

        public NonZeroIntegerRule()
        {
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Validates a input string to determine if it can be parsed into a non-zero interger.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="cultureInfo"></param>
        /// <returns>ValidResult if input can be parsed into a non-zero integer, error content otherwise.</returns>
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            ValidationResult result = ValidationResult.ValidResult;

            string intString = (string)value;

            if (intString.Length == 0)
            {
                result = new ValidationResult(false, "Field is required!");
            }
            else
            {
                if (int.TryParse(intString, out int output))
                {
                    if (output == 0)
                    {
                        result = new ValidationResult(false, "Value must be more than 0!");
                    }
                }
                else
                {
                    result = new ValidationResult(false, "Only integer values are allowed!");
                }
            }

            return result;
        }

        #endregion Methods
    }
}