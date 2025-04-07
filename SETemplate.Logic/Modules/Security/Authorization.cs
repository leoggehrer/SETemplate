//@CodeCopy

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
        /// <summary>
        /// Represents a class that handles authorization logic.
        /// </summary>
        static Authorization()
        {
            ClassConstructing();
            if (string.IsNullOrEmpty(SystemAuthorizationToken))
            {
                SystemAuthorizationToken = Guid.NewGuid().ToString();
            }
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

        /// <summary>
        /// Gets or sets the default time-out value in minutes.
        /// </summary>
        internal static int DefaultTimeOutInMinutes { get; private set; } = 90;
        /// <summary>
        /// Gets the default timeout value in seconds.
        /// </summary>
        internal static int DefaultTimeOutInSeconds => DefaultTimeOutInMinutes * 60;
        /// <summary>
        /// Gets or sets the system authorization token.
        /// </summary>
        internal static string SystemAuthorizationToken { get; set; }

        #region Check authorization for type
        /// <summary>
        /// Checks the authorization for a given session token, subject type, and action.
        /// </summary>
        /// <param name="sessionToken">The session token.</param>
        /// <param name="subjectType">The type of the subject.</param>
        /// <param name="action">The action to be authorized.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        internal static Task CheckAuthorizationAsync(string? sessionToken, Type subjectType, string action)
        {
            return CheckAuthorizationAsync(sessionToken, subjectType, action, string.Empty);
        }
        /// <summary>
        /// Checks the authorization for a given session token, subject type, action, and info data.
        /// </summary>
        /// <param name="sessionToken">The session token.</param>
        /// <param name="subjectType">The type of the subject.</param>
        /// <param name="action">The action to be performed.</param>
        /// <param name="infoData">The additional information data.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        internal static async Task CheckAuthorizationAsync(string? sessionToken, Type subjectType, string action, string infoData)
        {
            bool handled = false;

            BeforeCheckAuthorization(sessionToken, subjectType, action, ref handled);
            if (handled == false)
            {
                await CheckAuthorizationInternalAsync(sessionToken, subjectType, action, infoData).ConfigureAwait(false);
            }
            AfterCheckAuthorization(sessionToken, subjectType, action);
        }
        /// <summary>
        /// This method is called before checking the authorization for a specific action.
        /// </summary>
        /// <param name="sessionToken">The session token of the user.</param>
        /// <param name="subjectType">The type of the subject.</param>
        /// <param name="action">The action to be authorized.</param>
        /// <param name="handled">A reference to a boolean indicating whether the authorization has been handled.</param>
        static partial void BeforeCheckAuthorization(string? sessionToken, Type subjectType, string action, ref bool handled);
        /// <summary>
        /// This method is called after checking the authorization for a specific action.
        /// </summary>
        /// <param name="sessionToken">The session token of the user.</param>
        /// <param name="subjectType">The type of the subject being authorized.</param>
        /// <param name="action">The action being authorized.</param>
        static partial void AfterCheckAuthorization(string? sessionToken, Type subjectType, string action);

        /// <summary>
        /// Checks the authorization asynchronously.
        /// </summary>
        /// <param name="sessionToken">The session token.</param>
        /// <param name="subjectType">The type of the subject.</param>
        /// <param name="action">The action to be authorized.</param>
        /// <param name="roles">The roles required for authorization.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        internal static Task CheckAuthorizationAsync(string? sessionToken, Type subjectType, string action, params string[] roles)
        {
            return CheckAuthorizationAsync(sessionToken, subjectType, action, string.Empty, roles);
        }
        /// <summary>
        /// Checks the authorization for a given session token, subject type, action, info data, and roles.
        /// </summary>
        /// <param name="sessionToken">The session token.</param>
        /// <param name="subjectType">The type of the subject.</param>
        /// <param name="action">The action to be performed.</param>
        /// <param name="infoData">The info data.</param>
        /// <param name="roles">The roles to be checked.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        internal static async Task CheckAuthorizationAsync(string? sessionToken, Type subjectType, string action, string infoData, params string[] roles)
        {
            bool handled = false;

            BeforeCheckAuthorization(sessionToken, subjectType, action, roles, ref handled);
            if (handled == false)
            {
                await CheckAuthorizationInternalAsync(sessionToken, subjectType, action, infoData, roles).ConfigureAwait(false);
            }
            AfterCheckAuthorization(sessionToken, subjectType, action, roles);
        }
        /// <summary>
        /// This method is called before checking the authorization for a specific action.
        /// </summary>
        /// <param name="sessionToken">The session token of the user.</param>
        /// <param name="subjectType">The type of the subject being authorized.</param>
        /// <param name="action">The action being authorized.</param>
        /// <param name="roles">The roles required for the action.</param>
        /// <param name="handled">A reference to a boolean indicating whether the authorization has been handled.</param>
        static partial void BeforeCheckAuthorization(string? sessionToken, Type subjectType, string action, string[] roles, ref bool handled);
        /// <summary>
        /// This method is called after checking the authorization for a specific action.
        /// </summary>
        /// <param name="sessionToken">The session token of the user.</param>
        /// <param name="subjectType">The type of the subject being authorized.</param>
        /// <param name="action">The action being authorized.</param>
        /// <param name="roles">The roles required for the authorization.</param>
        static partial void AfterCheckAuthorization(string? sessionToken, Type subjectType, string action, string[] roles);
        #endregion Check authorization for type

        #region Check authorization for methodBase
        /// <summary>
        /// Checks the authorization asynchronously.
        /// </summary>
        /// <param name="sessionToken">The session token.</param>
        /// <param name="methodBase">The method base.</param>
        /// <param name="action">The action.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        internal static Task CheckAuthorizationAsync(string? sessionToken, MethodBase methodBase, string action)
        {
            return CheckAuthorizationAsync(sessionToken, methodBase, action, string.Empty);
        }
        /// <summary>
        /// Asynchronously checks the authorization for a given session token, method base, action, and info data.
        /// </summary>
        /// <param name="sessionToken">The session token.</param>
        /// <param name="methodBase">The method base.</param>
        /// <param name="action">The action.</param>
        /// <param name="infoData">The info data.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        internal static async Task CheckAuthorizationAsync(string? sessionToken, MethodBase methodBase, string action, string infoData)
        {
            bool handled = false;

            BeforeCheckAuthorization(sessionToken, methodBase, action, ref handled);
            if (handled == false)
            {
                await CheckAuthorizationInternalAsync(sessionToken, methodBase, action, infoData).ConfigureAwait(false);
            }
            AfterCheckAuthorization(sessionToken, methodBase, action);
        }
        /// <summary>
        /// This method is called before checking the authorization for a specific action.
        /// </summary>
        /// <param name="sessionToken">The session token of the user.</param>
        /// <param name="methodBase">The method base representing the current method.</param>
        /// <param name="action">The action being performed.</param>
        /// <param name="handled">A reference to a boolean value indicating whether the authorization has been handled.</param>
        /// <remarks>
        /// This method allows you to perform any necessary checks or modifications before the authorization process takes place.
        /// By setting the <paramref name="handled"/> parameter to <c>true</c>, you can indicate that the authorization has been handled and no further processing is required.
        /// </remarks>
        static partial void BeforeCheckAuthorization(string? sessionToken, MethodBase methodBase, string action, ref bool handled);
        /// <summary>
        /// This method is called after checking the authorization for a specific action.
        /// </summary>
        /// <param name="sessionToken">The session token of the user.</param>
        /// <param name="methodBase">The method base representing the action being authorized.</param>
        /// <param name="action">The name of the action being authorized.</param>
        static partial void AfterCheckAuthorization(string? sessionToken, MethodBase methodBase, string action);

        /// <summary>
        /// Checks the authorization for the specified session token, method base, action, and roles.
        /// </summary>
        /// <param name="sessionToken">The session token.</param>
        /// <param name="methodBase">The method base.</param>
        /// <param name="action">The action.</param>
        /// <param name="roles">The roles.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        internal static Task CheckAuthorizationAsync(string? sessionToken, MethodBase methodBase, string action, params string[] roles)
        {
            return CheckAuthorizationAsync(sessionToken, methodBase, action, string.Empty, roles);
        }
        /// <summary>
        /// Checks the authorization for a given session token, method base, action, info data, and roles.
        /// </summary>
        /// <param name="sessionToken">The session token.</param>
        /// <param name="methodBase">The method base.</param>
        /// <param name="action">The action.</param>
        /// <param name="infoData">The info data.</param>
        /// <param name="roles">The roles.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        internal static async Task CheckAuthorizationAsync(string? sessionToken, MethodBase methodBase, string action, string infoData, params string[] roles)
        {
            bool handled = false;

            BeforeCheckAuthorization(sessionToken, methodBase, action, roles, ref handled);
            if (handled == false)
            {
                await CheckAuthorizationInternalAsync(sessionToken, methodBase, action, infoData, roles).ConfigureAwait(false);
            }
            AfterCheckAuthorization(sessionToken, methodBase, action, roles);
        }
        /// <summary>
        /// This method is called before checking the authorization for a specific action.
        /// </summary>
        /// <param name="sessionToken">The session token of the user.</param>
        /// <param name="methodBase">The method base representing the action being authorized.</param>
        /// <param name="action">The name of the action being authorized.</param>
        /// <param name="roles">An array of roles required to access the action.</param>
        /// <param name="handled">A reference to a boolean value indicating whether the authorization has been handled.</param>
        /// <remarks>
        /// This method allows custom logic to be executed before the authorization check is performed.
        /// By setting the <paramref name="handled"/> parameter to <c>true</c>, the authorization check will be skipped.
        /// </remarks>
        static partial void BeforeCheckAuthorization(string? sessionToken, MethodBase methodBase, string action, string[] roles, ref bool handled);
        /// <summary>
        /// This method is called after checking the authorization for a specific action.
        /// </summary>
        /// <param name="sessionToken">The session token of the user.</param>
        /// <param name="methodBase">The method base representing the method being authorized.</param>
        /// <param name="action">The action being authorized.</param>
        /// <param name="roles">The roles required for the action.</param>
        static partial void AfterCheckAuthorization(string? sessionToken, MethodBase methodBase, string action, string[] roles);
        #endregion Check authorization for methodBase

        #region Implemented check authorization for type
        /// <summary>
        /// Checks the authorization for a given session token, subject type, action, and roles.
        /// </summary>
        /// <param name="sessionToken">The session token.</param>
        /// <param name="subjectType">The type of the subject.</param>
        /// <param name="action">The action to be authorized.</param>
        /// <param name="infoData">Additional information data.</param>
        /// <param name="roles">The roles required for authorization.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        private static async Task CheckAuthorizationInternalAsync(string? sessionToken, Type subjectType, string action, string infoData, params string[] roles)
        {
            if (string.IsNullOrEmpty(sessionToken))
            {
                if (IsAuthorizedRequired(subjectType))
                {
                    throw new AuthorizationException(ErrorType.NotLogedIn);
                }
            }
            else if (sessionToken.Equals(SystemAuthorizationToken) == false)
            {
                var curSession = await AccountManager.QueryAliveSessionAsync(sessionToken).ConfigureAwait(false);

                if (curSession == default)
                    throw new AuthorizationException(ErrorType.InvalidSessionToken);

                if (curSession.IsTimeout)
                    throw new AuthorizationException(ErrorType.AuthorizationTimeOut);

                if (IsAuthorized(subjectType, curSession, roles) == false)
                    throw new AuthorizationException(ErrorType.NotAuthorized);

                curSession.LastAccess = DateTime.UtcNow;
            }
        }
        /// <summary>
        /// Determines whether authorization is required for the specified subject type.
        /// </summary>
        /// <param name="subjectType">The type of the subject.</param>
        /// <returns><c>true</c> if authorization is required; otherwise, <c>false</c>.</returns>
        private static bool IsAuthorizedRequired(Type subjectType)
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
            var authorization = GetClassAuthorization(subjectType)
                              ?? throw new AuthorizationException(ErrorType.MissingAuthorizeAttribute);

            return authorization != null && authorization.Required;
        }
        /// <summary>
        /// Checks if the specified subject type is authorized based on the provided login session and roles.
        /// </summary>
        /// <param name="subjectType">The type of the subject to be authorized.</param>
        /// <param name="loginSession">The login session of the user.</param>
        /// <param name="roles">The roles required for authorization.</param>
        /// <returns><c>true</c> if the subject is authorized; otherwise, <c>false</c>.</returns>
        private static bool IsAuthorized(Type subjectType, LoginSession loginSession, params string[] roles)
        {
            static AuthorizeAttribute? GetClassAuthorization(Type classType)
            {
                var runType = classType;
                var result = default(AuthorizeAttribute);

                do
                {
                    result = runType.GetCustomAttributes<AuthorizeAttribute>().FirstOrDefault();
                    runType = runType.BaseType;
                } while (result == null && runType != null);
                return result;
            }
            var result = true;
            var authorization = GetClassAuthorization(subjectType)
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
        /// <param name="action">The action.</param>
        /// <param name="infoData">The info data.</param>
        /// <param name="roles">The roles.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        private static async Task CheckAuthorizationInternalAsync(string? sessionToken, MethodBase methodBase, string action, string infoData, params string[] roles)
        {
            if (string.IsNullOrEmpty(sessionToken))
            {
                if (IsAuthorizedRequired(methodBase))
                {
                    throw new AuthorizationException(ErrorType.NotLogedIn);
                }
            }
            else if (sessionToken.Equals(SystemAuthorizationToken) == false)
            {
                var curSession = await AccountManager.QueryAliveSessionAsync(sessionToken).ConfigureAwait(false);

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
        /// Determines whether authorization is required for the specified method.
        /// </summary>
        /// <param name="methodBase">The method to check for authorization.</param>
        /// <returns><c>true</c> if authorization is required; otherwise, <c>false</c>.</returns>
        private static bool IsAuthorizedRequired(MethodBase methodBase)
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

