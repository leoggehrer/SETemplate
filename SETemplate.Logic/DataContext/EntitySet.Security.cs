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
        /// Executes logic before accessing a method for read, including authorization checks.
        /// </summary>
        /// <param name="methodBase">The method being accessed for read.</param>
        partial void BeforeReadAccessing(MethodBase methodBase)
        {
            CheckReadAccessing(methodBase);
        }
        /// <summary>
        /// Executes logic before accessing a method for create, including authorization checks.
        /// </summary>
        /// <param name="methodBase">The method being accessed for create.</param>
        partial void BeforeCreateAccessing(MethodBase methodBase)
        {
            CheckCreateAccessing(methodBase);
        }
        /// <summary>
        /// Executes logic before accessing a method for update, including authorization checks.
        /// </summary>
        /// <param name="methodBase">The method being accessed for update.</param>
        partial void BeforeUpdateAccessing(MethodBase methodBase)
        {
            CheckUpdateAccessing(methodBase);
        }
        /// <summary>
        /// Executes logic before accessing a method for delete, including authorization checks.
        /// </summary>
        /// <param name="methodBase">The method being accessed for delete.</param>
        partial void BeforeDeleteAccessing(MethodBase methodBase)
        {
            CheckDeleteAccessing(methodBase);
        }
        /// <summary>
        /// Checks if the current session has access to the specified method or type.
        /// First checks for an <see cref="AuthorizeAttribute"/> on the method. If present and required, 
        /// authorization is enforced for the method. If not present, checks for the attribute on the type.
        /// If the type-level attribute is present and required, authorization is enforced for the type.
        /// </summary>
        /// <param name="methodBase">The method for which access is being checked.</param>
        protected virtual void CheckAccessing(MethodBase methodBase)
        {
            var methodAuthorize = Authorization.GetAuthorizeAttribute(methodBase);

            if (methodAuthorize != null)
            {
                if (methodAuthorize.Required)
                {
                    Authorization.CheckAuthorization(SessionToken, methodBase);
                }
            }
            else
            {
                var type = GetType();
                var typeAuthorize = Authorization.GetAuthorizeAttribute(type);

                if (typeAuthorize != null)
                {
                    if (typeAuthorize.Required)
                    {
                        Authorization.CheckAuthorization(SessionToken, type);
                    }
                }
            }
        }
        /// <summary>
        /// Checks if the current session has read access to the specified method.
        /// By default, delegates to <see cref="CheckAccessing(MethodBase)"/> for standard authorization checks.
        /// Can be overridden to implement custom read-access logic.
        /// </summary>
        /// <param name="methodBase">The method for which read access is being checked.</param>
        protected virtual void CheckReadAccessing(MethodBase methodBase)
        {
            CheckAccessing(methodBase);
        }
        /// <summary>
        /// Checks if the current session has create access to the specified method.
        /// By default, delegates to <see cref="CheckAccessing(MethodBase)"/> for standard authorization checks.
        /// Can be overridden to implement custom create-access logic.
        /// </summary>
        /// <param name="methodBase">The method for which create access is being checked.</param>
        protected virtual void CheckCreateAccessing(MethodBase methodBase)
        {
            CheckAccessing(methodBase);
        }
        /// <summary>
        /// Checks if the current session has update access to the specified method.
        /// By default, delegates to <see cref="CheckAccessing(MethodBase)"/> for standard authorization checks.
        /// Can be overridden to implement custom update-access logic.
        /// </summary>
        /// <param name="methodBase">The method for which update access is being checked.</param>
        protected virtual void CheckUpdateAccessing(MethodBase methodBase)
        {
            CheckAccessing(methodBase);
        }
        /// <summary>
        /// Checks if the current session has delete access to the specified method.
        /// By default, delegates to <see cref="CheckAccessing(MethodBase)"/> for standard authorization checks.
        /// Can be overridden to implement custom delete-access logic.
        /// </summary>
        /// <param name="methodBase">The method for which delete access is being checked.</param>
        protected virtual void CheckDeleteAccessing(MethodBase methodBase)
        {
            CheckAccessing(methodBase);
        }
        #endregion customize accessing
    }
}
#endif
