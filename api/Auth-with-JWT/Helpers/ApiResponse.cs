namespace Auth_with_JWT.Helpers
{
    /// <summary>
    /// Generic class to represent API responses.
    /// </summary>
    /// <typeparam name="T">The type of the data returned in the response.</typeparam>

    public class ApiResponse<T>
    {
        public int Code { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        // Add this constructor
        public ApiResponse(int code, string message, T? data)
        {
            Code = code;
            Message = message;
            Data = data;
        }
    }
}