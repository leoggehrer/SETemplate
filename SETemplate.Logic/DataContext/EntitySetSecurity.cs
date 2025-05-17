//@BaseCode
#if ACCOUNT_ON
using SETemplate.Logic.Modules.Security;
using System.Reflection;

namespace SETemplate.Logic.DataContext
{
    /// <summary>
    /// Represents a secure entity set with authorization checks.
    /// </summary>
    [Authorize]
    partial class EntitySet<TEntity>
    {
        #region properties
        /// <summary>
        /// Gets or sets the session token used for authorization.
        /// </summary>
        public string SessionToken
        {
            internal get => Context.SessionToken;
            set => Context.SessionToken = value;
        }
        #endregion properties

        #region methods
        /// <summary>
        /// Executes logic before accessing a method, including authorization checks.
        /// </summary>
        /// <param name="methodBase">The method being accessed.</param>
        partial void BeforeAccessing(MethodBase methodBase)
        {
            CheckAccessing(methodBase);
        }
        #endregion methods

        #region customize accessing
        /// <summary>
        /// Checks if the current session has access to the specified method or its declaring type.
        /// Performs authorization checks based on the presence and requirements of the <see cref="AuthorizeAttribute"/>.
        /// If the method has an <see cref="AuthorizeAttribute"/> with <c>Required = true</c>, authorization is checked for the method.
        /// Otherwise, if the declaring type has an <see cref="AuthorizeAttribute"/> with <c>Required = true</c>, authorization is checked for the type.
        /// Writes a debug message indicating the method being accessed.
        /// </summary>
        /// <param name="methodBase">The method for which access is being checked.</param>
        protected virtual void CheckAccessing(MethodBase methodBase)
        {
            var methodAuthorize = Authorization.GetAuthorizeAttribute(methodBase);

            if (methodAuthorize != null && methodAuthorize.Required)
            {
                Authorization.CheckAuthorization(SessionToken, methodBase);
            }
            else
            {
                var typeAuthorize = Authorization.GetAuthorizeAttribute(methodBase.DeclaringType!);

                if (typeAuthorize != null && typeAuthorize.Required)
                {
                    Authorization.CheckAuthorization(SessionToken, methodBase.DeclaringType!);
                }
            }
        }
        #endregion customize accessing
    }
}
#endif
