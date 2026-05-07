namespace MyShop.Application.Common.ResultPattern
{
    public class Result<T>
    {
        public bool Success => Error == null;
        public int Status { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public Error? Error { get; set; }
        public Meta? Meta { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;


        public static Result<T> Ok(T data, string message = "Operation completed successfully", Meta? meta = null)
        {
            return new Result<T>
            {
                Status = 200,
                Message = message,
                Data = data,
                Meta = meta
            };
        }


        public static Result<T> Created(T data, string message = "Created successfully")
        {
            return new Result<T>
            {
                Status = 201,
                Message = message,
                Data = data
            };
        }


        public static Result<T> NoContent(string message = "Deleted successfully")
        {
            return new Result<T>
            {
                Status = 204,
                Message = message,
                Data = default
            };
        }


        public static Result<T> Failure(string message, ErrorCode code, Dictionary<string, string[]>? details = null)
        {
            return new Result<T>
            {
                Status = GetHttpStatus(code),
                Message = message,
                Error = new Error(message, code.ToString(), details)
            };
        }

        private static int GetHttpStatus(ErrorCode code) => code switch
        {
            ErrorCode.VALIDATION_ERROR    => 422,
            ErrorCode.UNAUTHENTICATED     => 401,
            ErrorCode.FORBIDDEN           => 403,
            ErrorCode.NOT_FOUND           => 404,
            ErrorCode.DUPLICATE_ENTRY     => 409,
            ErrorCode.RATE_LIMIT_EXCEEDED => 429,
            ErrorCode.SERVER_ERROR        => 500,
            _                             => 500
        };
    }
}
