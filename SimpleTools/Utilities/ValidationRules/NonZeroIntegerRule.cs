using System.Globalization;
using System.Windows.Controls;

namespace SimpleTools.Utilities.ValidationRules
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
        /// Overrides default validation.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="cultureInfo"></param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
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