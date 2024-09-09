using Newtonsoft.Json;
using SimpleTools.Enums;
using SimpleTools.Interfaces;
using System.Text.Json;

namespace SimpleTools.Services
{
    public class MessageConversionService : IValidation<MessageConversionType>
    {
        #region Methods

        /// <summary>
        /// Validate if a message can be converted into a requested format.
        /// </summary>
        /// <param name="validationReference"></param>
        /// <param name="validationTarget"></param>
        /// <returns>
        /// <br>Item 1: True if conversion is successful, False otherwise.</br>
        /// <br>Item 2: Output message / error message.</br>
        /// </returns>
        public Tuple<bool, string> Validate(MessageConversionType validationReference, string validationTarget)
        {
            bool isError = false;
            string outputMessage;

            switch (validationReference)
            {
                case MessageConversionType.JSON:
                    if (string.IsNullOrEmpty(validationTarget))
                    {
                        outputMessage = validationTarget;
                    }
                    else if (IsJsonValid(validationTarget))
                    {
                        isError = false;
                        var parsedJson = JsonConvert.DeserializeObject(validationTarget);
                        outputMessage = JsonConvert.SerializeObject(parsedJson, Formatting.Indented);
                    }
                    else
                    {
                        isError = true;
                        outputMessage = "Invalid JSON!";
                    }
                    break;

                default:
                    outputMessage = validationTarget;
                    break;
            }

            return new Tuple<bool, string>(isError, outputMessage);
        }

        /// <summary>
        /// Check if a string is a valid JSON.
        /// </summary>
        /// <param name="jsonString"></param>
        /// <returns>True is valid, False otherwise.</returns>
        private bool IsJsonValid(string jsonString)
        {
            try
            {
                JsonDocument.Parse(jsonString);
                return true;
            }
            catch (System.Text.Json.JsonException)
            {
                return false;
            }
        }

        #endregion Methods
    }
}