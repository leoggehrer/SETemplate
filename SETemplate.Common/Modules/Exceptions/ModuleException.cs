//@BaseCode


namespace SETemplate.Common.Modules.Exceptions
{
    /// <summary>
    /// Represents errors encountered while running the application.
    /// </summary>
    public abstract partial class ModuleException : ApplicationException
    {
        /// Gets the error ID.
        /// @property ErrorId
        /// @return The error ID.
        public int ErrorId { get; } = -1;
        
        /// <summary>
        /// Initializes a new instance of the LogicException class with a specified error message.
        /// </summary>
        /// <param name="errorId">Identification of the error message.</param>
        public ModuleException(int errorId)
        : base(ErrorMessage.GetById(errorId))
        {
            ErrorId = errorId;
        }
        
        /// <summary>
        /// Initializes a new instance of the LogicException class with a specified error message.
        /// </summary>
        /// <param name="errorId">Identification of the error message.</param>
        /// <param name="message">The message that describes the error.</param>
        public ModuleException(int errorId, string message)
        : base($"{ErrorMessage.GetById(errorId)}: {message}")
        {
            ErrorId = errorId;
        }
        
        /// <summary>
        /// Initializes a new instance of the LogicException class with a specified error message.
        /// </summary>
        /// <param name="errorId">Identification of the error message.</param>
        /// <param name="ex">Exception die aufgetreten ist.</param>
        public ModuleException(int errorId, Exception ex)
        : base(ex.Message, ex.InnerException)
        {
            ErrorId = errorId;
        }
        
        /// <summary>
        /// Initializes a new instance of the LogicException class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public ModuleException(string? message)
        : base(message)
        {
        }
        
        /// <summary>
        /// Initializes a new instance of the LogicException class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="innerException">An instance of inner exception.</param>
        public ModuleException(string? message, Exception? innerException)
        : base(message, innerException)
        {
        }
    }
}


