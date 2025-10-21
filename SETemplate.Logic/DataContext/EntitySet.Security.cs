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
        /// Gets a dictionary of authorization parameters, where the key is a string identifier and the value is an <see
        /// cref="AuthorizeAttribute"/> representing the associated authorization settings.
        /// </summary>
        private static Dictionary<string, AuthorizeAttribute> AuthorizationParameters { get; } = new();
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
        /// Adds an authorization parameter for a specific type.
        /// </summary>
        /// <param name="type">The type for which to add authorization parameters.</param>
        /// <param name="authorizeAttribute">The authorization attribute containing the authorization rules.</param>
        /// <remarks>
        /// This method ensures that authorization parameters are added only once per type.
        /// If authorization parameters for the specified type already exist, they will not be overwritten.
        /// </remarks>
        internal static void AddAuthorizationParameter(Type type, AuthorizeAttribute authorizeAttribute)
        {
            if (!AuthorizationParameters.ContainsKey(type.Name))
            {
                AuthorizationParameters.Add(type.Name, authorizeAttribute);
            }
        }
        /// <summary>
        /// Adds an authorization parameter for a specific method.
        /// </summary>
        /// <param name="methodInfo">The method information for which to add authorization parameters.</param>
        /// <param name="authorizeAttribute">The authorization attribute containing the authorization rules.</param>
        /// <remarks>
        /// This method creates a unique key using the format "DeclaringTypeName.MethodName" to identify the method.
        /// Authorization parameters are added only once per method. If parameters for the specified method already exist,
        /// they will not be overwritten.
        /// </remarks>
        internal static void AddAuthorizationParameter(MethodInfo methodInfo, AuthorizeAttribute authorizeAttribute)
        {
            var methodKey = $"{methodInfo.DeclaringType?.Name}.{methodInfo.Name}";

            if (!AuthorizationParameters.ContainsKey(methodKey))
            {
                AuthorizationParameters.Add(methodKey, authorizeAttribute);
            }
        }
        #endregion methods

        #region partial methods
        /// <summary>
        /// Executes logic before accessing a method for read, including authorization checks.
        /// </summary>
        /// <param name="methodBase">The method being accessed for read.</param>
        /// <param name="roles">The roles required for authorization.</param>
        partial void CheckAccessBeforeReading(MethodBase methodBase, params string[] roles)
        {
            CheckReadAccessing(methodBase, roles);
        }
        /// <summary>
        /// Executes logic before accessing a method for create, including authorization checks.
        /// </summary>
        /// <param name="methodBase">The method being accessed for create.</param>
        /// <param name="roles">The roles required for authorization.</param>
        partial void CheckAccessBeforeCreating(MethodBase methodBase, params string[] roles)
        {
            CheckCreateAccessing(methodBase, roles);
        }
        /// <summary>
        /// Executes logic before accessing a method for update, including authorization checks.
        /// </summary>
        /// <param name="methodBase">The method being accessed for update.</param>
        /// <param name="roles">The roles required for authorization.</param>
        partial void CheckAccessBeforeUpdating(MethodBase methodBase, params string[] roles)
        {
            CheckUpdateAccessing(methodBase, roles);
        }
        /// <summary>
        /// Executes logic before accessing a method for delete, including authorization checks.
        /// </summary>
        /// <param name="methodBase">The method being accessed for delete.</param>
        /// <param name="roles">The roles required for authorization.</param>
        partial void CheckAccessBeforeDeleting(MethodBase methodBase, params string[] roles)
        {
            CheckDeleteAccessing(methodBase, roles);
        }
        #endregion partial methods

        #region accessing methods
        /// <summary>
        /// Checks if the current session has access to the specified method or type.
        /// First checks for an <see cref="AuthorizeAttribute"/> on the method. If present and required, 
        /// authorization is enforced for the method. If not present, checks for the attribute on the type.
        /// If the type-level attribute is present and required, authorization is enforced for the type.
        /// </summary>
        /// <param name="methodBase">The method for which access is being checked.</param>
        /// <param name="roles">The roles required for authorization.</param>
        protected virtual void CheckAccessing(MethodBase methodBase, params string[] roles)
        {
            var methodAuthorize = Authorization.GetAuthorizeAttribute(methodBase);

            if (methodAuthorize != null)
            {
                if (methodAuthorize.Required)
                {
                    Authorization.CheckAuthorization(SessionToken, methodBase, roles);
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
                        Authorization.CheckAuthorization(SessionToken, type, roles);
                    }
                }
                else
                {
                    var methodKey = $"{type.Name}.{methodBase.Name}";

                    if (AuthorizationParameters.TryGetValue(methodKey, out var methodAuthorizeAttribute))
                    {
                        Authorization.CheckAuthorization(SessionToken, methodAuthorizeAttribute, roles);
                    }
                    else if (AuthorizationParameters.TryGetValue(type.Name, out var typeAuthorizeAttribute))
                    {
                        Authorization.CheckAuthorization(SessionToken, typeAuthorizeAttribute, roles);
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
        /// <param name="roles">The roles required for authorization.</param>
        protected virtual void CheckReadAccessing(MethodBase methodBase, params string[] roles)
        {
            CheckAccessing(methodBase, roles);
        }
        /// <summary>
        /// Checks if the current session has create access to the specified method.
        /// By default, delegates to <see cref="CheckAccessing(MethodBase)"/> for standard authorization checks.
        /// Can be overridden to implement custom create-access logic.
        /// </summary>
        /// <param name="methodBase">The method for which create access is being checked.</param>
        /// <param name="roles">The roles required for authorization.</param>
        protected virtual void CheckCreateAccessing(MethodBase methodBase, params string[] roles)
        {
            CheckAccessing(methodBase, roles);
        }
        /// <summary>
        /// Checks if the current session has update access to the specified method.
        /// By default, delegates to <see cref="CheckAccessing(MethodBase)"/> for standard authorization checks.
        /// Can be overridden to implement custom update-access logic.
        /// </summary>
        /// <param name="methodBase">The method for which update access is being checked.</param>
        /// <param name="roles">The roles required for authorization.</param>
        protected virtual void CheckUpdateAccessing(MethodBase methodBase, params string[] roles)
        {
            CheckAccessing(methodBase, roles);
        }
        /// <summary>
        /// Checks if the current session has delete access to the specified method.
        /// By default, delegates to <see cref="CheckAccessing(MethodBase)"/> for standard authorization checks.
        /// Can be overridden to implement custom delete-access logic.
        /// </summary>
        /// <param name="methodBase">The method for which delete access is being checked.</param>
        /// <param name="roles">The roles required for authorization.</param>
        protected virtual void CheckDeleteAccessing(MethodBase methodBase, params string[] roles)
        {
            CheckAccessing(methodBase, roles);
        }
        #endregion accessing methods
    }
}
#endif
