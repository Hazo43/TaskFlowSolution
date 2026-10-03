namespace Shared.Errors
{
    public class ValidationErrorToReturn
    {
        public int StatusCode { get; set; }
        public string Message { get; set; } = null!;
        public IEnumerable<ValidationError> ValidationErrors { get; set; } = [];
    }
}
