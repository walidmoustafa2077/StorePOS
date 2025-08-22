namespace StorePOS.Domain.Services
{
    /// <summary>
    /// Generic result wrapper for service layer operations providing standardized response handling.
    /// Encapsulates operation success/failure status, HTTP status codes, data payload, and error information.
    /// </summary>
    /// <typeparam name="T">The type of data returned by the service operation</typeparam>
    /// <remarks>
    /// This class standardizes service layer responses across the application, providing:
    /// - Consistent error handling and status reporting
    /// - HTTP status code mapping for API responses
    /// - Validation error collection with field-specific messages
    /// - Null-safe data access patterns
    /// 
    /// Usage patterns:
    /// - Success operations: ServiceResult&lt;T&gt;.Ok(data) or Created(data)
    /// - Client errors: BadRequest, Validation, NotFound, Conflict
    /// - Empty success: NoContent for delete operations
    /// </remarks>
    public class ServiceResult<T>
    {
        /// <summary>Gets a value indicating whether the operation completed successfully.</summary>
        public bool IsSuccess { get; init; }
        
        /// <summary>Gets the HTTP status code representing the operation result.</summary>
        public int StatusCode { get; init; }
        
        /// <summary>Gets the data payload returned by the operation, if any.</summary>
        public T? Data { get; init; }
        
        /// <summary>Gets validation errors organized by field name, if any.</summary>
        public Dictionary<string, string[]>? Errors { get; init; }
        
        /// <summary>Gets a descriptive message about the operation result.</summary>
        public string? Message { get; init; }

        /// <summary>Creates a successful result with 200 OK status and data payload.</summary>
        public static ServiceResult<T> Ok(T data) => new() { IsSuccess = true, StatusCode = 200, Data = data };
        
        /// <summary>Creates a successful result with 201 Created status and data payload.</summary>
        public static ServiceResult<T> Created(T data) => new() { IsSuccess = true, StatusCode = 201, Data = data };
        
        /// <summary>Creates a successful result with 204 No Content status.</summary>
        public static ServiceResult<T> NoContent() => new() { IsSuccess = true, StatusCode = 204 };
        
        /// <summary>Creates a failed result with 404 Not Found status and optional message.</summary>
        public static ServiceResult<T> NotFound(string message = "Not found") => new() { IsSuccess = false, StatusCode = 404, Message = message };
        
        /// <summary>Creates a failed result with 409 Conflict status and descriptive message.</summary>
        public static ServiceResult<T> Conflict(string message) => new() { IsSuccess = false, StatusCode = 409, Message = message };
        
        /// <summary>Creates a failed result with 400 Bad Request status and validation errors.</summary>
        public static ServiceResult<T> Validation(Dictionary<string, string[]> errors) => new() { IsSuccess = false, StatusCode = 400, Errors = errors };
        
        /// <summary>Creates a failed result with 400 Bad Request status and error message.</summary>
        public static ServiceResult<T> BadRequest(string message) => new() { IsSuccess = false, StatusCode = 400, Message = message };
    }
}
