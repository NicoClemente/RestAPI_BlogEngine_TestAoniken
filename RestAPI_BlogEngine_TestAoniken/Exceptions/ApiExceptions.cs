namespace RestAPi_BlogEngine_TestAoniken.Exceptions
{
    // Custom exception class for API-related errors
    public class ApiException : Exception
    {
        // Constructor to initialize the ApiException with a message and status code
        public ApiException(string message, int statusCode) : base(message)
        {
            StatusCode = statusCode;
        }


        // Property to hold the status code of the exception
        public int StatusCode { get; set; }
    }
}