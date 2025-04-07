//@BaseCode
#if ACCOUNT_ON
using SETemplate.Logic.Modules.Security;
using System.Reflection;

namespace SETemplate.Logic.DataContext
{
    [Authorize]
    partial class EntitySet<TEntity>
    {
        #region properties
        /// <summary>
        /// Sets the session token.
        /// </summary>
        public string SessionToken
        {
            internal get => Context.SessionToken;
            set => Context.SessionToken = value;
        }
        #endregion properties

        #region methods
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
        protected virtual bool BeforeAccessingHandler(MethodBase methodBase)
        {
            return false;
        }
        #endregion customize accessing
    }
}
#endif
