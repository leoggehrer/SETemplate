//@BaseCode
#if ACCOUNT_ON
using SETemplate.Logic.Modules.Security;
using System.Reflection;

namespace SETemplate.Logic.DataContext
{
    /// <summary>
    /// Represents a view set with security features.
    /// </summary>
    [Authorize]
    partial class ViewSet<TView>
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
            bool handled;

            handled = BeforeAccessingHandler(methodBase);
            if (handled == false)
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
                System.Diagnostics.Debug.WriteLine($"Before accessing {methodBase.Name}");
            }
        }
        #endregion methods

        #region customize accessing
        /// <summary>
        /// Customizable handler for logic before accessing a method.
        /// </summary>
        /// <param name="methodBase">The method being accessed.</param>
        /// <returns>True if the access is handled; otherwise, false.</returns>
        protected virtual bool BeforeAccessingHandler(MethodBase methodBase)
        {
            return false;
        }
        #endregion customize accessing
    }
}
#endif
