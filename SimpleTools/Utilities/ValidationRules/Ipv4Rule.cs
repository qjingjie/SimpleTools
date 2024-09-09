using System.Globalization;
using System.Net;
using System.Windows.Controls;

namespace SimpleTools.Utilities.ValidationRules
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
        /// Overrides default validation.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="cultureInfo"></param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
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