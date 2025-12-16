//@BaseCode

using System.Net;
using System.Text.Json;
using SETemplate.Logic.Modules.Exceptions;

namespace SETemplate.WebApi.Middleware
{
    /// <summary>
    /// Middleware for handling exceptions and mapping them to appropriate HTTP status codes.
    /// </summary>
    public partial class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExceptionHandlingMiddleware"/> class.
        /// </summary>
        /// <param name="next">The next middleware in the pipeline.</param>
        /// <param name="logger">The logger instance.</param>
        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        /// <summary>
        /// Invokes the middleware.
        /// </summary>
        /// <param name="context">The HTTP context.</param>
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred: {Message}", ex.Message);
                await HandleExceptionAsync(context, ex);
            }
        }

        /// <summary>
        /// Handles the exception and writes an appropriate response.
        /// </summary>
        /// <param name="context">The HTTP context.</param>
        /// <param name="exception">The exception to handle.</param>
        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var (statusCode, errorResponse) = MapExceptionToResponse(exception);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(errorResponse, options));
        }

        /// <summary>
        /// Maps an exception to an HTTP status code and error response.
        /// </summary>
        /// <param name="exception">The exception to map.</param>
        /// <returns>A tuple containing the HTTP status code and error response.</returns>
        private static (HttpStatusCode statusCode, ErrorResponse errorResponse) MapExceptionToResponse(Exception exception)
        {
            var errorResponse = new ErrorResponse
            {
                Message = exception.Message,
                Details = GetFullExceptionMessage(exception)
            };

            // Handle ModuleException and its derived types
            if (exception is CommonModules.Exceptions.ModuleException moduleException)
            {
                errorResponse.ErrorId = moduleException.ErrorId;

                // Map ErrorId to HTTP Status Code
                var statusCode = MapErrorIdToStatusCode(moduleException.ErrorId);

#if ACCOUNT_ON
                // Special handling for AuthorizationException
                if (exception is AuthorizationException)
                {
                    return (HttpStatusCode.Unauthorized, errorResponse);
                }
#endif

                // Special handling for BusinessRuleException
                if (exception is BusinessRuleException)
                {
                    return (HttpStatusCode.UnprocessableEntity, errorResponse);
                }

                return (statusCode, errorResponse);
            }

            // Handle other common exceptions
            return exception switch
            {
                ArgumentNullException => (HttpStatusCode.BadRequest, errorResponse),
                ArgumentException => (HttpStatusCode.BadRequest, errorResponse),
                InvalidOperationException => (HttpStatusCode.Conflict, errorResponse),
                UnauthorizedAccessException => (HttpStatusCode.Unauthorized, errorResponse),
                KeyNotFoundException => (HttpStatusCode.NotFound, errorResponse),
                _ => (HttpStatusCode.InternalServerError, errorResponse)
            };
        }

        /// <summary>
        /// Maps an ErrorId to an HTTP status code.
        /// </summary>
        /// <param name="errorId">The error ID.</param>
        /// <returns>The corresponding HTTP status code.</returns>
        private static HttpStatusCode MapErrorIdToStatusCode(int errorId)
        {
            // Use the ErrorType constants from Common.Modules.Exceptions
            return errorId switch
            {
                // Validation errors -> 400 Bad Request
                CommonModules.Exceptions.ErrorType.InvalidPropertyName => HttpStatusCode.BadRequest,
                CommonModules.Exceptions.ErrorType.InvalidEntitySet => HttpStatusCode.BadRequest,
                CommonModules.Exceptions.ErrorType.InvalidId => HttpStatusCode.BadRequest,
                CommonModules.Exceptions.ErrorType.InvalidPageSize => HttpStatusCode.BadRequest,

#if ACCOUNT_ON
                // Authentication errors -> 401 Unauthorized
                CommonModules.Exceptions.ErrorType.InvalidToken => HttpStatusCode.Unauthorized,
                CommonModules.Exceptions.ErrorType.InvalidSessionToken => HttpStatusCode.Unauthorized,
                CommonModules.Exceptions.ErrorType.InvalidJsonWebToken => HttpStatusCode.Unauthorized,
                CommonModules.Exceptions.ErrorType.InvalidEmail => HttpStatusCode.Unauthorized,
                CommonModules.Exceptions.ErrorType.InvalidPassword => HttpStatusCode.Unauthorized,
                CommonModules.Exceptions.ErrorType.NotLogedIn => HttpStatusCode.Unauthorized,
                CommonModules.Exceptions.ErrorType.AuthorizationTimeOut => HttpStatusCode.Unauthorized,

                // Authorization errors -> 403 Forbidden
                CommonModules.Exceptions.ErrorType.NotAuthorized => HttpStatusCode.Forbidden,
                CommonModules.Exceptions.ErrorType.MissingAuthorizeAttribute => HttpStatusCode.Forbidden,

                // Account errors -> 400 Bad Request
                CommonModules.Exceptions.ErrorType.InvalidAccount => HttpStatusCode.BadRequest,
                CommonModules.Exceptions.ErrorType.InvalidIdentityName => HttpStatusCode.BadRequest,
                CommonModules.Exceptions.ErrorType.InvalidPasswordSyntax => HttpStatusCode.BadRequest,
                CommonModules.Exceptions.ErrorType.InvalidEmailSyntax => HttpStatusCode.BadRequest,
                CommonModules.Exceptions.ErrorType.InitAppAccess => HttpStatusCode.BadRequest,
                CommonModules.Exceptions.ErrorType.AddAppAccess => HttpStatusCode.BadRequest,

#if ACCESSRULES_ON
                // Access rule violations -> 403 Forbidden
                CommonModules.Exceptions.ErrorType.InvalidAccessRuleEntityValue => HttpStatusCode.Forbidden,
                CommonModules.Exceptions.ErrorType.InvalidAccessRuleAccessValue => HttpStatusCode.Forbidden,
                CommonModules.Exceptions.ErrorType.InvalidAccessRuleAlreadyExits => HttpStatusCode.Conflict,
                CommonModules.Exceptions.ErrorType.AccessRuleViolationCanNotCreated => HttpStatusCode.Forbidden,
                CommonModules.Exceptions.ErrorType.AccessRuleViolationCanNotRead => HttpStatusCode.Forbidden,
                CommonModules.Exceptions.ErrorType.AccessRuleViolationCanNotChanged => HttpStatusCode.Forbidden,
                CommonModules.Exceptions.ErrorType.AccessRuleViolationCanNotDeleted => HttpStatusCode.Forbidden,
#endif
#endif

                // Entity errors -> 422 Unprocessable Entity
                CommonModules.Exceptions.ErrorType.InvalidEntityInsert => HttpStatusCode.UnprocessableEntity,
                CommonModules.Exceptions.ErrorType.InvalidEntityUpdate => HttpStatusCode.UnprocessableEntity,
                CommonModules.Exceptions.ErrorType.InvalidEntityContent => HttpStatusCode.UnprocessableEntity,

                // Operation errors -> 400 Bad Request
                CommonModules.Exceptions.ErrorType.InvalidOperation => HttpStatusCode.BadRequest,

                // Default -> 500 Internal Server Error
                _ => HttpStatusCode.InternalServerError
            };
        }

        /// <summary>
        /// Gets the full exception message including inner exceptions.
        /// </summary>
        /// <param name="ex">The exception.</param>
        /// <returns>The full error message.</returns>
        private static string GetFullExceptionMessage(Exception? ex)
        {
            var messages = new List<string>();
            while (ex != null)
            {
                messages.Add($"{ex.GetType().Name}: {ex.Message}");
                ex = ex.InnerException;
            }
            return string.Join(" -> ", messages);
        }
    }

    /// <summary>
    /// Represents an error response returned by the API.
    /// </summary>
    public class ErrorResponse
    {
        /// <summary>
        /// Gets or sets the error ID.
        /// </summary>
        public int? ErrorId { get; set; }

        /// <summary>
        /// Gets or sets the error message.
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the detailed error information.
        /// </summary>
        public string? Details { get; set; }

        /// <summary>
        /// Gets or sets the timestamp of the error.
        /// </summary>
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }

    /// <summary>
    /// Extension methods for registering the exception handling middleware.
    /// </summary>
    public static class ExceptionHandlingMiddlewareExtensions
    {
        /// <summary>
        /// Adds the exception handling middleware to the application pipeline.
        /// </summary>
        /// <param name="app">The application builder.</param>
        /// <returns>The application builder.</returns>
        public static IApplicationBuilder UseExceptionHandling(this IApplicationBuilder app)
        {
            return app.UseMiddleware<ExceptionHandlingMiddleware>();
        }
    }
}
