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

        #region partial methods
        partial void BeforeAccessingHandler(MethodBase methodBase, ref bool handled);
        #endregion partial methods
    }
}
#endif
