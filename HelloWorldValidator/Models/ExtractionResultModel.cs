namespace HelloWorldValidator.Models
{
    public class ExtractionResultModel
    {
        public bool ContainsHelloWorld { get; set; }
        public string ErrorMessage { get; set; }

        public bool IsError => !string.IsNullOrEmpty(ErrorMessage);
    }
}
