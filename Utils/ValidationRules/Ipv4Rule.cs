using System.Globalization;
using System.Net;
using System.Windows.Controls;

namespace Utils.ValidationRules
{
    public class Ipv4Rule : ValidationRule
    {
        #region Constructor

        public Ipv4Rule()
        {
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Validates a input string to determine if it is a valid IP address.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="cultureInfo"></param>
        /// <returns>ValidResult if IP address is valid, error content otherwise.</returns>
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            ValidationResult result = ValidationResult.ValidResult;

            string ip = (string)value;

            if (ip.Length == 0)
            {
                result = new ValidationResult(false, "Field is required!");
            }
            else if (ip != "localhost" && ip != "*")
            {
                string[] octets = ip.Split('.');

                if (!(IPAddress.TryParse(ip, out _) && octets.Length == 4))
                {
                    result = new ValidationResult(false, "Invalid IP address!");
                }
            }

            return result;
        }

        #endregion Methods
    }
}