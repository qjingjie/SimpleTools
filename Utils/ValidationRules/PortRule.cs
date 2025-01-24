using System.Globalization;
using System.Windows.Controls;

namespace Utils.ValidationRules
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
        /// Validates a input string to determine if it is a valid port.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="cultureInfo"></param>
        /// <returns>ValidResult if port is valid, error content otherwise.</returns>
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
                    if (port < 1 || port > 65535)
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