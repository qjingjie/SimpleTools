using System.Globalization;
using System.Windows.Controls;

namespace SimpleTools.Utilities.ValidationRules
{
    public class PortRule : ValidationRule
    {
        #region Constructor

        public PortRule()
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

            string portString = (string)value;

            if (portString.Length == 0)
            {
                result = new ValidationResult(false, "Field is required!");
            }
            else
            {
                if (int.TryParse(portString, out int port))
                {
                    if (port < 0 || port > 65535)
                    {
                        result = new ValidationResult(false, "Invalid port!");
                    }
                }
                else
                {
                    result = new ValidationResult(false, "Invalid port!");
                }
            }

            return result;
        }

        #endregion Methods
    }
}