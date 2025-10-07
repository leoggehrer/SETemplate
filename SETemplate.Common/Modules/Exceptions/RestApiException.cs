//@BaseCode


namespace SETemplate.Common.Modules.Exceptions
{
    /// <summary>
    /// Represents errors encountered while running the application.
    /// </summary>
    public partial class RestApiException : ModuleException
    {
        /// <summary>
        /// Initializes a new instance of the LogicException class with a specified error message.
        /// </summary>
        /// <param name="errorId">Identification of the error message.</param>
        public RestApiException(int errorId)
        : base(errorId)
        {
        }
        
        /// <summary>
        /// Initializes a new instance of the LogicException class with a specified error message.
        /// </summary>
        /// <param name="errorId">Identification of the error message.</param>
        /// <param name="message">The message that describes the error.</param>
        public RestApiException(int errorId, string message)
        : base(errorId, message)
        {
        }
        
        /// <summary>
        /// Initializes a new instance of the LogicException class with a specified error message.
        /// </summary>
        /// <param name="errorId">Identification der Fehlermeldung.</param>
        /// <param name="ex">Exception die aufgetreten ist.</param>
        public RestApiException(int errorId, Exception ex)
        : base(errorId, ex)
        {
        }
        
        /// <summary>
        /// Initializes a new instance of the LogicException class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public RestApiException(string? message)
        : base(message)
        {
        }
        
        /// <summary>
        /// Initializes a new instance of the LogicException class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="innerException">An instance of inner exception.</param>
        public RestApiException(string? message, Exception? innerException)
        : base(message, innerException)
        {
        }
    }
}


