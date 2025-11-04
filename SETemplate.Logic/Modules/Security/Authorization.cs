//@BaseCode
#if ACCOUNT_ON
using SETemplate.Logic.Entities.Account;
using SETemplate.Logic.Modules.Exceptions;
using System.Reflection;

namespace SETemplate.Logic.Modules.Security
{
    using CommonModules.Exceptions;
    using SETemplate.Logic.Modules.Account;

    /// <summary>
    /// Provides authorization functionality for checking permissions and roles.
    /// </summary>
    internal static partial class Authorization
    {
        #region properties
        /// <summary>
        /// Gets or sets the default time-out value in minutes.
        /// </summary>
        internal static int DefaultTimeOutInMinutes { get; private set; } = 90;
        /// <summary>
        /// Gets the default timeout value in seconds.
        /// </summary>
        internal static int DefaultTimeOutInSeconds => DefaultTimeOutInMinutes * 60;
        #endregion properties

        #region class constructor
        /// <summary>
        /// Represents a class that handles authorization logic.
        /// </summary>
        static Authorization()
        {
            ClassConstructing();
            ClassConstructed();
        }
        /// <summary>
        /// This method is called during the construction of the class.
        /// It is a partial method, which means it can be implemented in a separate file.
        /// </summary>
        static partial void ClassConstructing();
        /// <summary>
        /// This method is called when the class is constructed.
        /// It is a partial method, which means it can be implemented in a partial class or a partial struct.
        /// </summary>
        static partial void ClassConstructed();
        #endregion class constructor

        #region Implemented check authorization for attribute
        /// <summary>
        /// Checks the authorization for a given session token, method, action, and roles.
        /// </summary>
        /// <param name="sessionToken">The session token.</param>
        /// <param name="authorizeAttribute">The authorization attribute to evaluate.</param>
        /// <param name="roles">The roles.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        internal static void CheckAuthorization(string? sessionToken, AuthorizeAttribute authorizeAttribute, params string[] roles)
        {
            if (string.IsNullOrEmpty(sessionToken))
            {
                if (authorizeAttribute.Required)
                {
                    throw new AuthorizationException(ErrorType.NotLogedIn);
                }
            }
            else
            {
                var curSession = AccountManager.QueryLoginSession(sessionToken);

                if (curSession == default)
                    throw new AuthorizationException(ErrorType.InvalidSessionToken);

                if (curSession.IsTimeout)
                    throw new AuthorizationException(ErrorType.AuthorizationTimeOut);

                if (IsAuthorized(authorizeAttribute, curSession, roles) == false)
                    throw new AuthorizationException(ErrorType.NotAuthorized);

                curSession.LastAccess = DateTime.UtcNow;
            }
        }
        /// <summary>
        /// Determines whether the specified authorize attribute permits access for the given login session and roles.
        /// </summary>
        /// <param name="authorizeAttribute">The authorization attribute to evaluate.</param>
        /// <param name="loginSession">The login session of the user requesting access.</param>
        /// <param name="roles">Additional roles to check for authorization.</param>
        /// <returns>
        /// <c>true</c> if authorization is not required, or if no specific roles are required, 
        /// or if the login session contains at least one of the required roles; otherwise, <c>false</c>.
        /// </returns>
        private static bool IsAuthorized(AuthorizeAttribute authorizeAttribute, LoginSession loginSession, params string[] roles)
        {
            var result = true;

            if (authorizeAttribute.Required)
            {
                var allRoles = authorizeAttribute.Roles.Union(roles);

                result = allRoles.Any() == false
                       || loginSession.Roles.Any(lr => allRoles.Contains(lr.Designation));
            }
            return result;
        }
        #endregion Implemented check authorization for attribute

        #region Implemented check authorization for type
        /// <summary>
        /// Checks the authorization for a given session token, subject type, action, and roles.
        /// </summary>
        /// <param name="sessionToken">The session token.</param>
        /// <param name="type">The type of the subject.</param>
        /// <param name="roles">The roles required for authorization.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        internal static void CheckAuthorization(string? sessionToken, Type type, params string[] roles)
        {
            if (string.IsNullOrEmpty(sessionToken))
            {
                if (IsAuthorizedRequired(type))
                {
                    throw new AuthorizationException(ErrorType.NotLogedIn);
                }
            }
            else
            {
                var curSession = AccountManager.QueryLoginSession(sessionToken);

                if (curSession == default)
                    throw new AuthorizationException(ErrorType.InvalidSessionToken);

                if (curSession.IsTimeout)
                    throw new AuthorizationException(ErrorType.AuthorizationTimeOut);

                if (IsAuthorized(type, curSession, roles) == false)
                    throw new AuthorizationException(ErrorType.NotAuthorized);

                curSession.LastAccess = DateTime.UtcNow;
            }
        }
        /// <summary>
        /// Retrieves the <see cref="AuthorizeAttribute"/> from the specified <see cref="Type"/>.
        /// </summary>
        /// <param name="type">The type from which to retrieve the attribute.</param>
        /// <returns>The <see cref="AuthorizeAttribute"/> if found; otherwise, <c>null</c>.</returns>
        internal static AuthorizeAttribute? GetAuthorizeAttribute(Type type)
        {
            static AuthorizeAttribute? GetClassAuthorization(Type classType)
            {
                var runType = classType;
                AuthorizeAttribute? result;

                do
                {
                    result = runType.GetCustomAttributes<AuthorizeAttribute>().FirstOrDefault();
                    runType = runType.BaseType;
                } while (result == null && runType != null);
                return result;
            }
            return GetClassAuthorization(type);
        }
        /// <summary>
        /// Determines whether authorization is required for the specified subject type.
        /// </summary>
        /// <param name="type">The type of the subject.</param>
        /// <returns><c>true</c> if authorization is required; otherwise, <c>false</c>.</returns>
        internal static bool IsAuthorizedRequired(Type type)
        {
            var authorization = GetAuthorizeAttribute(type);

            return authorization != null && authorization.Required;
        }
        /// <summary>
        /// Checks if the specified subject type is authorized based on the provided login session and roles.
        /// </summary>
        /// <param name="type">The type of the subject to be authorized.</param>
        /// <param name="loginSession">The login session of the user.</param>
        /// <param name="roles">The roles required for authorization.</param>
        /// <returns><c>true</c> if the subject is authorized; otherwise, <c>false</c>.</returns>
        private static bool IsAuthorized(Type type, LoginSession loginSession, params string[] roles)
        {
            var result = true;
            var authorization = GetAuthorizeAttribute(type)
                              ?? throw new AuthorizationException(ErrorType.MissingAuthorizeAttribute);

            if (authorization.Required)
            {
                var allRoles = authorization.Roles.Union(roles);

                result = allRoles.Any() == false
                       || loginSession.Roles.Any(lr => allRoles.Contains(lr.Designation));
            }
            return result;
        }
        #endregion Implemented check authorization for type

        #region Implemented check authorization for methodBase
        /// <summary>
        /// Checks the authorization for a given session token, method, action, and roles.
        /// </summary>
        /// <param name="sessionToken">The session token.</param>
        /// <param name="methodBase">The method base.</param>
        /// <param name="roles">The roles.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        internal static void CheckAuthorization(string? sessionToken, MethodBase methodBase, params string[] roles)
        {
            if (string.IsNullOrEmpty(sessionToken))
            {
                if (IsAuthorizedRequired(methodBase))
                {
                    throw new AuthorizationException(ErrorType.NotLogedIn);
                }
            }
            else
            {
                var curSession = AccountManager.QueryLoginSession(sessionToken);

                if (curSession == default)
                    throw new AuthorizationException(ErrorType.InvalidSessionToken);

                if (curSession.IsTimeout)
                    throw new AuthorizationException(ErrorType.AuthorizationTimeOut);

                if (IsAuthorized(methodBase, curSession, roles) == false)
                    throw new AuthorizationException(ErrorType.NotAuthorized);

                curSession.LastAccess = DateTime.UtcNow;
            }
        }
        /// <summary>
        /// Checks the authorization for a given session token, method, action, and roles.
        /// </summary>
        /// <param name="sessionToken">The session token.</param>
        /// <param name="methodBase">The method base.</param>
        /// <param name="roles">The roles.</param>
        internal static void CheckAuthorizationInternal(string? sessionToken, MethodBase methodBase, params string[] roles)
        {
            if (string.IsNullOrEmpty(sessionToken))
            {
                if (IsAuthorizedRequired(methodBase))
                {
                    throw new AuthorizationException(ErrorType.NotLogedIn);
                }
            }
            else
            {
                var curSession = AccountManager.QueryLoginSession(sessionToken);

                if (curSession == default)
                    throw new AuthorizationException(ErrorType.InvalidSessionToken);

                if (curSession.IsTimeout)
                    throw new AuthorizationException(ErrorType.AuthorizationTimeOut);

                if (IsAuthorized(methodBase, curSession, roles) == false)
                    throw new AuthorizationException(ErrorType.NotAuthorized);

                curSession.LastAccess = DateTime.UtcNow;
            }
        }

        /// <summary>
        /// Retrieves the <see cref="AuthorizeAttribute"/> from the specified <see cref="MethodBase"/>.
        /// </summary>
        /// <param name="methodBase">The method base from which to retrieve the attribute.</param>
        /// <returns>The <see cref="AuthorizeAttribute"/> if found; otherwise, <c>null</c>.</returns>
        internal static AuthorizeAttribute? GetAuthorizeAttribute(MethodBase methodBase)
        {
            return methodBase.GetCustomAttributes<AuthorizeAttribute>().FirstOrDefault();
        }
        /// <summary>
        /// Determines whether authorization is required for the specified method.
        /// </summary>
        /// <param name="methodBase">The method to check for authorization.</param>
        /// <returns><c>true</c> if authorization is required; otherwise, <c>false</c>.</returns>
        internal static bool IsAuthorizedRequired(MethodBase methodBase)
        {
            var authorization = methodBase.GetCustomAttributes<AuthorizeAttribute>().FirstOrDefault()
                              ?? throw new AuthorizationException(ErrorType.MissingAuthorizeAttribute);

            return authorization?.Required ?? false;
        }

        /// <summary>
        /// Determines whether the specified method is authorized for the given login session and roles.
        /// </summary>
        /// <param name="methodBase">The method base representing the method being checked for authorization.</param>
        /// <param name="loginSession">The login session of the user.</param>
        /// <param name="roles">The roles required for authorization.</param>
        /// <returns><c>true</c> if the method is authorized; otherwise, <c>false</c>.</returns>
        private static bool IsAuthorized(MethodBase methodBase, LoginSession loginSession, params string[] roles)
        {
            var result = true;
            var authorization = methodBase.GetCustomAttributes<AuthorizeAttribute>().FirstOrDefault() 
                              ?? throw new AuthorizationException(ErrorType.MissingAuthorizeAttribute);

            if (authorization.Required)
            {
                var allRoles = authorization.Roles.Union(roles);

                result = allRoles.Any() == false
                    || loginSession.Roles.Any(lr => allRoles.Contains(lr.Designation));
            }
            return result;
        }
        #endregion Implemented check authorization for methodBase
    }
}
#endif


