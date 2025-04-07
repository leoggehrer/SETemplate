//@BaseCode
using System.Text;

namespace SETemplate.Common.Modules.Exceptions
{
    /// <summary>
    /// Represents a collection of error messages identified by integer keys.
    /// </summary>
    public static partial class ErrorMessage
    {
        /// <summary>
        /// Contains a collection of messages identified by their integer keys.
        /// </summary>
        private static Dictionary<int, string> Messages { get; set; }
        /// <summary>
        /// Initializes the ErrorMessage class.
        /// </summary>
        static ErrorMessage()
        {
            ClassConstructing();
            Messages = new Dictionary<int, string>
            {
#if ACCOUNT_ON
                { ErrorType.InitAppAccess, "The initialization of the app access is not permitted because an app access has already been initialized." },
                { ErrorType.InvalidAccount, "Invalid identity or password." }
#endif
            };
            
            InitMessages();
            ClassConstructed();
        }
        /// <summary>
        /// This is a partial method that is called before the construction of the class.
        /// </summary>
        static partial void ClassConstructing();
        /// <summary>
        /// This method is called when the class is constructed.
        /// </summary>
        /// <remarks>
        /// This method is declared as "static partial" meaning it is only a part of the class and not meant to be used externally.
        /// </remarks>
        /// <returns>Void</returns>
        static partial void ClassConstructed();
        /// <summary>
        /// Initializes the error messages for the ErrorType enumeration.
        /// </summary>
        private static void InitMessages()
        {
            foreach (var item in typeof(ErrorType).GetFields())
            {
                var value = Convert.ToInt16(item.GetValue(null));
                
                if (Messages.ContainsKey(value) == false)
                {
                    var sb = new StringBuilder();
                    var error = item.Name ?? string.Empty;
                    
                    foreach (var chr in error)
                    {
                        if (char.IsUpper(chr))
                        {
                            if (sb.Length > 0)
                            {
                                sb.Append(' ');
                            }
                            sb.Append(chr);
                        }
                        else
                        sb.Append(chr);
                    }
                    Messages.Add(value, sb.ToString());
                }
            }
        }
        
        /// <summary>
        /// Retrieves a message based on the given error ID.
        /// </summary>
        /// <param name="errorId">The ID of the error.</param>
        /// <returns>The message associated with the error ID. If the error ID is not found, an empty string is returned.</returns>
        public static string GetById(int errorId)
        {
            string result = string.Empty;
            
            if (Messages.ContainsKey(errorId))
            {
                result = Messages[errorId];
            }
            return result;
        }
    }
}
