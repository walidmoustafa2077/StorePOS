namespace StorePOS.Client.Wpf.Services
{
    /// <summary>
    /// Generic result wrapper for service operations providing standardized response handling
    /// </summary>
    /// <typeparam name="T">The type of data returned by the service operation</typeparam>
    public class ServiceResult<T>
    {
        /// <summary>
        /// Indicates whether the operation was successful
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// The data returned by the operation (null if failed)
        /// </summary>
        public T? Data { get; set; }

        /// <summary>
        /// Error message if the operation failed
        /// </summary>
        public string ErrorMessage { get; set; } = string.Empty;

        /// <summary>
        /// List of validation errors
        /// </summary>
        public List<string> ValidationErrors { get; set; } = new();

        /// <summary>
        /// Exception details if applicable
        /// </summary>
        public Exception? Exception { get; set; }

        /// <summary>
        /// Creates a successful result with data
        /// </summary>
        public static ServiceResult<T> Success(T data)
        {
            return new ServiceResult<T>
            {
                IsSuccess = true,
                Data = data
            };
        }

        /// <summary>
        /// Creates a successful result without data
        /// </summary>
        public static ServiceResult<T> Success()
        {
            return new ServiceResult<T>
            {
                IsSuccess = true
            };
        }

        /// <summary>
        /// Creates a failed result with error message
        /// </summary>
        public static ServiceResult<T> Failure(string errorMessage)
        {
            return new ServiceResult<T>
            {
                IsSuccess = false,
                ErrorMessage = errorMessage
            };
        }

        /// <summary>
        /// Creates a failed result with exception
        /// </summary>
        public static ServiceResult<T> Failure(Exception exception)
        {
            return new ServiceResult<T>
            {
                IsSuccess = false,
                ErrorMessage = exception.Message,
                Exception = exception
            };
        }

        /// <summary>
        /// Creates a failed result with validation errors
        /// </summary>
        public static ServiceResult<T> ValidationFailure(List<string> validationErrors)
        {
            return new ServiceResult<T>
            {
                IsSuccess = false,
                ErrorMessage = "Validation failed",
                ValidationErrors = validationErrors
            };
        }

        /// <summary>
        /// Creates a failed result with a single validation error
        /// </summary>
        public static ServiceResult<T> ValidationFailure(string validationError)
        {
            return new ServiceResult<T>
            {
                IsSuccess = false,
                ErrorMessage = "Validation failed",
                ValidationErrors = new List<string> { validationError }
            };
        }

        /// <summary>
        /// Checks if the result has validation errors
        /// </summary>
        public bool HasValidationErrors => ValidationErrors.Any();

        /// <summary>
        /// Gets all error messages combined
        /// </summary>
        public string AllErrorMessages
        {
            get
            {
                var messages = new List<string>();
                
                if (!string.IsNullOrEmpty(ErrorMessage))
                    messages.Add(ErrorMessage);
                
                if (ValidationErrors.Any())
                    messages.AddRange(ValidationErrors);
                
                return string.Join(Environment.NewLine, messages);
            }
        }
    }

    /// <summary>
    /// Non-generic service result for operations that don't return data
    /// </summary>
    public class ServiceResult : ServiceResult<object>
    {
        /// <summary>
        /// Creates a successful result
        /// </summary>
        public new static ServiceResult Success()
        {
            return new ServiceResult
            {
                IsSuccess = true
            };
        }

        /// <summary>
        /// Creates a failed result with error message
        /// </summary>
        public new static ServiceResult Failure(string errorMessage)
        {
            return new ServiceResult
            {
                IsSuccess = false,
                ErrorMessage = errorMessage
            };
        }

        /// <summary>
        /// Creates a failed result with exception
        /// </summary>
        public new static ServiceResult Failure(Exception exception)
        {
            return new ServiceResult
            {
                IsSuccess = false,
                ErrorMessage = exception.Message,
                Exception = exception
            };
        }

        /// <summary>
        /// Creates a failed result with validation errors
        /// </summary>
        public new static ServiceResult ValidationFailure(List<string> validationErrors)
        {
            return new ServiceResult
            {
                IsSuccess = false,
                ErrorMessage = "Validation failed",
                ValidationErrors = validationErrors
            };
        }

        /// <summary>
        /// Creates a failed result with a single validation error
        /// </summary>
        public new static ServiceResult ValidationFailure(string validationError)
        {
            return new ServiceResult
            {
                IsSuccess = false,
                ErrorMessage = "Validation failed",
                ValidationErrors = new List<string> { validationError }
            };
        }
    }
}