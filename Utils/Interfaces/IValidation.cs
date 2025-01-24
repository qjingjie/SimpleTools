namespace Utils.Interfaces
{
    public interface IValidation<T> where T : IConvertible
    {
        /// <summary>
        /// Validate a target input against a target reference.
        /// </summary>
        /// <param name="validationReference"></param>
        /// <param name="validationTarget"></param>
        /// <returns>Boolean result and string validated output.</returns>
        public (bool isValid, string validatedOutput) Validate(T validationReference, string validationTarget);
    }
}