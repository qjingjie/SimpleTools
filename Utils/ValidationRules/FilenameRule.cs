using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Utils.ValidationRules
{
    public class FilenameRule : ValidationRule
    {
        #region Fields
        private readonly string[] _reservedNames;

        #endregion
        #region Constructor

        public FilenameRule()
        {
            _reservedNames = [
            "CON", "PRN", "AUX", "NUL",
            "COM1", "COM2", "COM3", "COM4", "COM5", "COM6", "COM7", "COM8", "COM9",
            "LPT1", "LPT2", "LPT3", "LPT4", "LPT5", "LPT6", "LPT7", "LPT8", "LPT9"
            ];

        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Validates a input string to determine if it is a valid filename.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="cultureInfo"></param>
        /// <returns>ValidResult if filename is valid, error content otherwise.</returns>
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            ValidationResult result = ValidationResult.ValidResult;

            string filename = (string)value;

            if (string.IsNullOrWhiteSpace(filename))
            {
                result = new ValidationResult(false, "Filename cannot be empty!");
            }

            if (filename.Length > 255)
            {
                result = new ValidationResult(false, "Filename is too long!");
            }

            char[] invalidChars = Path.GetInvalidFileNameChars();
            foreach (char c in filename)
            {
                if (Array.Exists(invalidChars, invalidChar => invalidChar == c))
                {
                    result = new ValidationResult(false, "Invalid characters detected!");
                    break;
                }
            }

            if (Array.Exists(_reservedNames, reserved => reserved.Equals(filename, StringComparison.InvariantCultureIgnoreCase)))
            {
                result = new ValidationResult(false, "Filename is reserved!");
            }


            return result;
        }

        #endregion Methods
    }
}

