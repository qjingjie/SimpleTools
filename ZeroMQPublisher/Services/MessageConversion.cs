using Newtonsoft.Json;
using Utils.Interfaces;
using ZeroMQPublisher.Enums;

namespace ZeroMQPublisher.Services
{
    public class MessageConversion : IValidation<MessageFormat>
    {
        #region Methods

        /// <summary>
        /// Validate a string input against a desired message format.
        /// </summary>
        /// <param name="messageFormat"></param>
        /// <param name="message"></param>
        /// <returns>Boolean result and string validated output.</returns>
        public (bool isValid, string validatedOutput) Validate(MessageFormat messageFormat, string message)
        {
            bool isValid = true;
            string result;

            switch (messageFormat)
            {
                case MessageFormat.Json:
                    if (string.IsNullOrEmpty(message))
                    {
                        result = message;
                    }
                    else if (IsValidJson(message))
                    {
                        // Format message nicely
                        var parsedMessage = JsonConvert.DeserializeObject(message);
                        result = JsonConvert.SerializeObject(parsedMessage, Formatting.Indented);
                    }
                    else
                    {
                        isValid = false;
                        result = "Invalid JSON format. Please check the message syntax.";
                    }
                    break;

                default:
                    result = message;
                    break;
            }

            return (isValid, result);
        }

        /// <summary>
        /// Check if a string message can be parsed into a valid JSON object.
        /// </summary>
        /// <param name="message"></param>
        /// <returns>True is valid, false otherwise.</returns>
        private static bool IsValidJson(string message)
        {
            bool isValid = true;

            try
            {
                _ = JsonConvert.DeserializeObject(message);
            }
            catch
            {
                isValid = false;
            }

            return isValid;
        }

        #endregion Methods
    }
}