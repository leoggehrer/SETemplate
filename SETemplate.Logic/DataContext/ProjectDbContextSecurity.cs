//@BaseCode
#if ACCOUNT_ON
using SETemplate.Logic.Modules.Security;
using System.Reflection;

namespace SETemplate.Logic.DataContext
{
    [Authorize]
    partial class ProjectDbContext
    {
        #region fields
        private string? _sessionToken;
        #endregion fields

        #region properties
        /// <summary>
        /// Sets the session token.
        /// </summary>
        public string SessionToken
        {
            internal get => _sessionToken ?? string.Empty;
            set
            {
                _sessionToken = value;
            }
        }
        #endregion properties

        #region constructors
        public ProjectDbContext(string sessionToken)
        {
            SessionToken = sessionToken;
            Constructing();

            Constructed();
        }
        #endregion constructors

        #region methods
        /// <summary>
        /// Handles authorization checks before accessing a method.
        /// Invokes a handler for custom logic, then checks for <see cref="AuthorizeAttribute"/> on the method or its declaring type.
        /// If authorization is required, calls <see cref="Authorization.CheckAuthorization(string, MethodBase)"/> or <see cref="Authorization.CheckAuthorization(string, Type)"/>.
        /// </summary>
        /// <param name="methodBase">The method being accessed.</param>
        partial void BeforeAccessing(MethodBase methodBase)
        {
            bool handled = false;

            BeforeAccessingHandler(methodBase, ref handled);
            if (handled == false)
            {
                var methodAuthorize = Authorization.GetAuthorizeAttribute(methodBase);

                if (methodAuthorize != null && methodAuthorize.Required)
                {
                    Authorization.CheckAuthorization(SessionToken, methodBase);
                }
                else
                {
                    var type = GetType();
                    var typeAuthorize = Authorization.GetAuthorizeAttribute(type);

                    if (typeAuthorize != null && typeAuthorize.Required)
                    {
                        Authorization.CheckAuthorization(SessionToken, type);
                    }
                }
                System.Diagnostics.Debug.WriteLine($"Before accessing {methodBase.Name}");
            }
        }
        #endregion methods

        #region customize accessing
        /// <summary>
        /// Partial method for handling custom logic before accessing a method.
        /// Implement this method in a partial class to provide custom authorization or handling logic.
        /// Set <paramref name="handled"/> to true to bypass default authorization checks.
        /// </summary>
        /// <param name="methodBase">The method being accessed.</param>
        /// <param name="handled">Set to true to indicate the access was handled and skip default checks.</param>
        partial void BeforeAccessingHandler(MethodBase methodBase, ref bool handled);
        #endregion customize accessing
    }
}
#endif
