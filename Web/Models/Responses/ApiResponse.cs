namespace Web.Models.Responses
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public bool IsSuccess => Success;
        public T? Data { get; set; }
        public string Message { get; set; } = string.Empty;
        public int StatusCode { get; set; }
        public Dictionary<string, string[]>? ValidationErrors { get; set; }

        public static ApiResponse<T> SuccessResponse(T data, string message = "")
        {
            return new ApiResponse<T>
            {
                Success = true,
                Data = data,
                Message = message,
                StatusCode = 200
            };
        }

        public static ApiResponse<T> ErrorResponse(string message, int statusCode = 400)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Message = message,
                StatusCode = statusCode
            };
        }

        public static ApiResponse<T> ValidationErrorResponse(Dictionary<string, string[]> errors)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Message = "Error de validación",
                StatusCode = 400,
                ValidationErrors = errors
            };
        }
    }
}