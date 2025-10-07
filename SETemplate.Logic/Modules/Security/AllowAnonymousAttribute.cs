//@BaseCode

#if ACCOUNT_ON

namespace SETemplate.Logic.Modules.Security
{
    /// <summary>
    /// Specifies that the decorated class or method allows anonymous access.
    /// Inherits from the <see cref="System.Web.Mvc.AuthorizeAttribute"/> class.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    internal partial class AllowAnonymousAttribute : AuthorizeAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AllowAnonymousAttribute"/> class.
        /// </summary>
        public AllowAnonymousAttribute()
            : base(false)
        {
            
        }
    }
}
#endif

