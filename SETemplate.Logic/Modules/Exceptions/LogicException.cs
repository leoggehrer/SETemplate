//@BaseCode
namespace SETemplate.Logic.Modules.Exceptions
{
    /// <summary>
    /// Represents errors encountered while running the application.
    /// </summary>
    public partial class LogicException : CommonModules.Exceptions.ModuleException
    {
        /// <summary>
        /// Initializes a new instance of the LogicException class with a specified error message.
        /// </summary>
        /// <param name="errorId">Identification of the error message.</param>
        public LogicException(int errorId)
        : base(errorId)
        {
        }
        
        /// <summary>
        /// Initializes a new instance of the LogicException class with a specified error message.
        /// </summary>
        /// <param name="errorId">Identification of the error message.</param>
        /// <param name="message">The message that describes the error.</param>
        public LogicException(int errorId, string message)
        : base(errorId, message)
        {
        }
        
        /// <summary>
        /// Initializes a new instance of the LogicException class with a specified error message.
        /// </summary>
        /// <param name="errorId">Identification der Fehlermeldung.</param>
        /// <param name="ex">Exception die aufgetreten ist.</param>
        public LogicException(int errorId, Exception ex)
        : base(errorId, ex)
        {
        }
        
        /// <summary>
        /// Initializes a new instance of the LogicException class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public LogicException(string? message)
        : base(message)
        {
        }
        
        /// <summary>
        /// Initializes a new instance of the LogicException class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="innerException">An instance of inner exception.</param>
        public LogicException(string? message, Exception? innerException)
        : base(message, innerException)
        {
        }
    }
}

