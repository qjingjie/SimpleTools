namespace SimpleTools.Interfaces
{
    public interface IValidation<T> where T : IConvertible
    {
        Tuple<bool, string> Validate(T validationReference, string validationTarget);
    }
}