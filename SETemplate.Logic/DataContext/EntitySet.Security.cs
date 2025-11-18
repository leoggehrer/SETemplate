//@BaseCode
#if ACCOUNT_ON
using SETemplate.Logic.Modules.Exceptions;
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
        private static Dictionary<string, AuthorizeAttribute> Authorizations { get; } = new();
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
        /// Determines whether the current user has permission to perform the specified action.
        /// </summary>
        /// <param name="methodName">The name of the method to check permissions for.</param>
        /// <returns>True if the user has permission; otherwise, false.</returns>
        public bool HasCurrentUserPermission(string methodName)
        {
            var result = true;
            var methodBase = GetType().GetMethod(methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);

            if (methodBase != null)
            {
                try
                {
                    CheckAccessing(methodBase);
                }
                catch (AuthorizationException)
                {
                    result = false;
                }
            }
            return result;
        }
        #endregion methods

        #region methods authorization
        /// <summary>
        /// Generates a unique key identifier for the specified type.
        /// </summary>
        /// <param name="type">The type for which to generate a key.</param>
        /// <returns>A string representing the type's name used as a dictionary key for authorization lookups.</returns>
        internal static string GetTypeKey(Type type) => type.GetTypeInfo().Name;
        /// <summary>
        /// Generates a unique key identifier for the specified method.
        /// </summary>
        /// <param name="methodBase">The method for which to generate a key.</param>
        /// <returns>
        /// A string in the format "DeclaringTypeName.MethodName" used as a dictionary key for authorization lookups.
        /// If the declaring type is null, returns ".MethodName".
        /// </returns>
        internal static string GetMethodKey(MethodBase methodBase) => $"{methodBase.DeclaringType?.Name}.{methodBase.Name}";
        /// <summary>
        /// Sets authorization for Get operations.
        /// </summary>
        /// <param name="type">The type for which to set authorization.</param>
        /// <param name="authorize">The authorization settings to apply.</param>
        internal static void SetAuthorization4Get(Type type, Modules.Security.AuthorizeAttribute authorize)
        {
            SetAuthorization(type, nameof(EntitySet<TEntity>.GetAsync), authorize);
            SetAuthorization(type, nameof(EntitySet<TEntity>.GetByIdAsync), authorize);
            SetAuthorization(type, nameof(EntitySet<TEntity>.QueryByIdAsync), authorize);
            SetAuthorization(type, nameof(EntitySet<TEntity>.QueryAsync), authorize);
        }
        /// <summary>
        /// Sets authorization for Create operations.
        /// </summary>
        /// <param name="type">The type for which to set authorization.</param>
        /// <param name="authorize">The authorization settings to apply.</param>
        internal static void SetAuthorization4Create(Type type, Modules.Security.AuthorizeAttribute authorize)
        {
            SetAuthorization(type, nameof(EntitySet<TEntity>.Create), authorize);
            SetAuthorization(type, nameof(EntitySet<TEntity>.AddAsync), authorize);
            SetAuthorization(type, nameof(EntitySet<TEntity>.AddRangeAsync), authorize);
        }
        /// <summary>
        /// Sets authorization for Update operations.
        /// </summary>
        /// <param name="type">The type for which to set authorization.</param>
        /// <param name="authorize">The authorization settings to apply.</param>
        internal static void SetAuthorization4Update(Type type, Modules.Security.AuthorizeAttribute authorize)
        {
            SetAuthorization(type, nameof(EntitySet<TEntity>.UpdateAsync), authorize);
        }
        /// <summary>
        /// Sets authorization for Delete operations.
        /// </summary>
        /// <param name="type">The type for which to set authorization.</param>
        /// <param name="authorize">The authorization settings to apply.</param>
        internal static void SetAuthorization4Delete(Type type, Modules.Security.AuthorizeAttribute authorize)
        {
            SetAuthorization(type, nameof(EntitySet<TEntity>.Create), authorize);
            SetAuthorization(type, nameof(EntitySet<TEntity>.AddAsync), authorize);
            SetAuthorization(type, nameof(EntitySet<TEntity>.UpdateAsync), authorize);
            SetAuthorization(type, nameof(EntitySet<TEntity>.RemoveAsync), authorize);
        }
        /// <summary>
        /// Generates a unique key identifier for the specified method within the context of a specific type.
        /// </summary>
        /// <param name="type">The type context for the method.</param>
        /// <param name="methodBase">The method for which to generate a key.</param>
        /// <returns>
        /// A string in the format "TypeName.MethodName" used as a dictionary key for authorization lookups.
        /// </returns>
        internal static string GetMethodKey(Type type, MethodBase methodBase) => $"{GetTypeKey(type)}.{methodBase.Name}";
        /// <summary>
        /// Sets the authorization attribute for the specified type.
        /// </summary>
        /// <param name="type">The type for which to set authorization.</param>
        /// <param name="authorizeAttribute">The authorization attribute to associate with the type.</param>
        internal static void SetAuthorization(Type type, AuthorizeAttribute authorizeAttribute)
        {
            SetAuthorization(GetTypeKey(type), authorizeAttribute);
        }
        /// <summary>
        /// Sets the authorization attribute for a specific method on the specified type.
        /// </summary>
        /// <param name="type">The type containing the method for which to set authorization.</param>
        /// <param name="methodName">The name of the method for which to set authorization.</param>
        /// <param name="authorizeAttribute">The authorization attribute to associate with the method.</param>
        /// <exception cref="ArgumentException">
        /// Thrown when the specified method is not found on the type.
        /// </exception>
        internal static void SetAuthorization(Type type, string methodName, AuthorizeAttribute authorizeAttribute)
        {
            var methodInfo = type.GetMethod(methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);

            // Resolve to the original method (for async state-machine generated methods) and fall back to the found MethodInfo.
            MethodBase? methodBase = methodInfo?.GetAsyncOriginal() ?? (MethodBase?)methodInfo;

            if (methodBase == null)
            {
                throw new ArgumentException($"Method '{methodName}' not found on type '{type.FullName}'.", nameof(methodName));
            }
            SetAuthorization(type, methodBase, authorizeAttribute);
        }
        /// <summary>
        /// Sets the authorization attribute for the specified type and method.
        /// </summary>
        /// <param name="type">The type for which to set authorization.</param>
        /// <param name="methodBase">The method for which to set authorization.</param>
        /// <param name="authorizeAttribute">The authorization attribute to associate with the method.</param>
        internal static void SetAuthorization(Type type, MethodBase methodBase, AuthorizeAttribute authorizeAttribute)
        {
            SetAuthorization(GetMethodKey(type, methodBase), authorizeAttribute);
        }
        /// <summary>
        /// Sets the authorization attribute for the specified key in the authorization dictionary.
        /// </summary>
        /// <param name="key">The unique key identifying the type or method for which to set authorization.</param>
        /// <param name="authorizeAttribute">The authorization attribute to associate with the key.</param>
        internal static void SetAuthorization(string key, AuthorizeAttribute authorizeAttribute)
        {
            if (!Authorizations.ContainsKey(key))
            {
                Authorizations.Add(key, authorizeAttribute);
            }
            else
            {
                Authorizations[key] = authorizeAttribute;
            }
        }
        /// <summary>
        /// Gets the authorization parameter for a specific type.
        /// </summary>
        /// <param name="type">The type for which to retrieve the authorization parameter.</param>
        /// <returns>The authorization parameter for the specified type, or null if not found.</returns>
        internal static AuthorizeAttribute? GetAuthorization(Type type)
        {
            var runType = type;
            AuthorizeAttribute? result = null;

            while (runType != null && result == null)
            {
                Authorizations.TryGetValue(GetTypeKey(runType), out result);
                runType = runType.BaseType;
            }
            return result;
        }
        /// <summary>
        /// Gets the authorization parameter for a specific type and method.
        /// </summary>
        /// <param name="type">The type for which to retrieve the authorization parameter.</param>
        /// <param name="methodBase">The method for which to retrieve the authorization parameter.</param>
        /// <returns>The authorization parameter for the specified type and method, or null if not found.</returns>
        internal static AuthorizeAttribute? GetAuthorization(Type type, MethodBase methodBase)
        {
            var runType = type;
            AuthorizeAttribute? result = null;

            while (runType != null && result == null)
            {
                Authorizations.TryGetValue(GetMethodKey(runType, methodBase), out result);
                runType = runType.BaseType;
            }
            return result;
        }
        /// <summary>
        /// Removes an authorization parameter for a specific key.
        /// </summary>
        /// <param name="key">The key identifying the authorization parameter.</param>
        internal static void RemoveAuthorization(string key)
        {
            if (Authorizations.ContainsKey(key))
            {
                Authorizations.Remove(key);
            }
        }
        #endregion methods authorization

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
            var type = GetType();
            AuthorizeAttribute? authorizeAttribute = GetAuthorization(type, methodBase);

            if (authorizeAttribute != null)
            {
                if (authorizeAttribute.Required)
                {
                    Authorization.CheckAuthorization(SessionToken, authorizeAttribute, roles);
                }
            }
            else
            {
                authorizeAttribute = GetAuthorization(type);

                if (authorizeAttribute != null)
                {
                    if (authorizeAttribute.Required)
                    {
                        Authorization.CheckAuthorization(SessionToken, authorizeAttribute, roles);
                    }
                }
                else
                {
                    authorizeAttribute = Authorization.GetAuthorizeAttribute(methodBase);

                    if (authorizeAttribute != null)
                    {
                        if (authorizeAttribute.Required)
                        {
                            Authorization.CheckAuthorization(SessionToken, authorizeAttribute, roles);
                        }
                    }
                    else
                    {
                        authorizeAttribute = Authorization.GetAuthorizeAttribute(type);

                        if (authorizeAttribute != null)
                        {
                            if (authorizeAttribute.Required)
                            {
                                Authorization.CheckAuthorization(SessionToken, authorizeAttribute, roles);
                            }
                        }
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
