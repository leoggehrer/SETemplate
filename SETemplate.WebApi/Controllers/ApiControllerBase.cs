//@BaseCode
using Microsoft.AspNetCore.Mvc;

namespace SETemplate.WebApi.Controllers
{
    /// <summary>
    /// A base class for an MVC controller without view support.
    /// </summary>
    public abstract partial class ApiControllerBase : ControllerBase
    {
        /// <summary>
        /// Initializes the static ApiControllerBase class.
        /// </summary>
        static ApiControllerBase()
        {
            ClassConstructing();
            ClassConstructed();
        }
        /// <summary>
        /// Represents a partial method called before the constructor of the class is executed.
        /// </summary>
        /// <remarks>
        /// This method is automatically generated and can be implemented in partial classes.
        /// </remarks>
        static partial void ClassConstructing();
        /// <summary>
        /// This method is called after the class is constructed.
        /// </summary>
        static partial void ClassConstructed();

        /// <summary>
        /// Initializes a new instance of the ApiControllerBase class.
        /// </summary>
        protected ApiControllerBase()
        {
            Constructing();
            Constructed();
        }
        /// <summary>
        /// This method is called during the construction of the object.
        /// </summary>
        /// <remarks>
        /// This method can be overridden by a partial class to include additional custom logic.
        /// It is defined as "partial" so that multiple partial classes can provide their own implementation of this method.
        /// </remarks>
        partial void Constructing();
        /// <summary>
        /// This method is called after the object has been initialized.
        /// It represents a partial method without an implementation.
        /// </summary>
        partial void Constructed();

        /// <summary>
        /// Builds a detailed error message string from an exception and its inner exceptions.
        /// </summary>
        /// <param name="ex">The exception to extract messages from.</param>
        /// <returns>
        /// A string containing the type and message of the exception and all inner exceptions,
        /// each prefixed with their depth in the exception chain.
        /// </returns>
        public static string GetErrorMessage(Exception? ex)
        {
            int depth = 0;
            var messages = new List<string>();

            while (ex != null)
            {
                messages.Add($"[{depth}] {ex.GetType().Name}: {ex.Message}");
                ex = ex.InnerException;
                depth++;
            }

            return string.Join(Environment.NewLine, messages);
        }
    }
}
