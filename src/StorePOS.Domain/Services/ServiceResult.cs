namespace StorePOS.Domain.Services
{
    public class ServiceResult<T>
    {
        public bool IsSuccess { get; init; }
        public int StatusCode { get; init; }
        public T? Data { get; init; }
        public Dictionary<string, string[]>? Errors { get; init; }
        public string? Message { get; init; }

        public static ServiceResult<T> Ok(T data) => new() { IsSuccess = true, StatusCode = 200, Data = data };
        public static ServiceResult<T> Created(T data) => new() { IsSuccess = true, StatusCode = 201, Data = data };
        public static ServiceResult<T> NoContent() => new() { IsSuccess = true, StatusCode = 204 };
        public static ServiceResult<T> NotFound(string message = "Not found") => new() { IsSuccess = false, StatusCode = 404, Message = message };
        public static ServiceResult<T> Conflict(string message) => new() { IsSuccess = false, StatusCode = 409, Message = message };
        public static ServiceResult<T> Validation(Dictionary<string, string[]> errors) => new() { IsSuccess = false, StatusCode = 400, Errors = errors };
        public static ServiceResult<T> BadRequest(string message) => new() { IsSuccess = false, StatusCode = 400, Message = message };
    }
}
