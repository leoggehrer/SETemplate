//@CodeCopy

#if ACCOUNT_ON && ACCESSRULES_ON
namespace SETemplate.Logic.Modules.Exceptions
{
    /// <summary>
    /// Represents errors that occur during application execution.
    /// </summary>
    public partial class AccessRuleException : CommonModules.Exceptions.ModuleException
    {
        /// <summary>
        /// Initializes a new instance of the AccessRuleException class with a specified error message.
        /// </summary>
        /// <param name="errorId">Identification of the error message.</param>
        public AccessRuleException(int errorId)
            : base(errorId)
        {
        }
        
        /// <summary>
        /// Initializes a new instance of the AccessRuleException class with a specified error message.
        /// </summary>
        /// <param name="errorId">Identification of the error message.</param>
        /// <param name="message">The message that describes the error.</param>
        public AccessRuleException(int errorId, string message)
            : base(errorId, message)
        {
        }
    }
}
#endif
