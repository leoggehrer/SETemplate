//@BaseCode
#if ACCOUNT_ON
using SETemplate.Logic.Models.Account;
using SETemplate.Logic.Modules.Account;

namespace SETemplate.Logic
{
    /// <summary>
    /// Provides access to various account-related operations.
    /// </summary>
    public static partial class AccountAccess
    {
        /// <summary>
        /// Checks if the session associated with the given session token is alive.
        /// </summary>
        /// <param name="sessionToken">The session token used for authentication.</param>
        /// <returns>A task representing the asynchronous operation. The task result contains a boolean indicating whether the session is alive or not.</returns>
        public static Task<bool> IsSessionAliveAsync(string sessionToken)
        {
            return AccountManager.IsSessionAliveAsync(sessionToken);
        }
        
        /// <summary>
        /// Initializes application access for a user by calling the AccountManager's InitAppAccessAsync method.
        /// </summary>
        /// <param name="name">The name of the user.</param>
        /// <param name="email">The email of the user.</param>
        /// <param name="password">The password of the user.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        public static Task InitAppAccessAsync(string name, string email, string password)
        {
            return AccountManager.InitAppAccessAsync(name, email, password);
        }
        /// <summary>
        /// Asynchronously adds application access for a user.
        /// </summary>
        /// <param name="sessionToken">The session token of the user.</param>
        /// <param name="name">The name of the user.</param>
        /// <param name="email">The email of the user.</param>
        /// <param name="password">The password of the user.</param>
        /// <param name="timeOutInMinutes">The timeout duration for the access in minutes.</param>
        /// <param name="roles">An optional array of roles to assign to the user.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        /// <remarks>
        /// This method adds application access for a user by calling the AddAppAccessAsync method in the AccountManager class.
        /// </remarks>
        public static Task AddAppAccessAsync(string sessionToken, string name, string email, string password, int timeOutInMinutes, params string[] roles)
        {
            return AccountManager.AddAppAccessAsync(sessionToken, name, email, password, timeOutInMinutes, roles);
        }
        
        /// <summary>
        /// Logs a user on asynchronously using the provided email and password.
        /// </summary>
        /// <param name="email">The email of the user.</param>
        /// <param name="password">The password of the user.</param>
        /// <returns>A task that represents the asynchronous logon operation. The task result contains a <see cref="LoginSession"/> object.</returns>
        public static async Task<LoginSession> LoginAsync(string email, string password)
        {
            var result = await AccountManager.LoginAsync(email, password).ConfigureAwait(false);
            
            return LoginSession.CloneFrom(result);
        }
        /// <summary>
        /// Asynchronously logs on a user using the specified email, password, and optional information.
        /// </summary>
        /// <param name="email">The email of the user.</param>
        /// <param name="password">The password of the user.</param>
        /// <param name="optionalInfo">Optional information related to the user.</param>
        /// <returns>The login session object.</returns>
        public static async Task<LoginSession> LoginAsync(string email, string password, string optionalInfo)
        {
            var result = await AccountManager.LoginAsync(email, password, optionalInfo).ConfigureAwait(false);

            return LoginSession.CloneFrom(result);
        }
        /// <summary>
        /// Queries the login using the provided session token asynchronously.
        /// </summary>
        /// <param name="sessionToken">The session token to be used for querying login.</param>
        /// <returns>
        /// A task representing the asynchronous operation. The task result is a nullable LoginSession.
        /// If the login is successfully queried, a new LoginSession instance is created and returned.
        /// If the login query returns null, null is returned.
        /// </returns>
        public static async Task<LoginSession?> QueryLoginAsync(string sessionToken)
        {
            var result = await AccountManager.QueryLoginAsync(sessionToken).ConfigureAwait(false);
            
            return result != null ? LoginSession.CloneFrom(result) : null;
        }
        
        /// <summary>
        /// Checks if a user, identified by the sessionToken, has a specific role.
        /// </summary>
        /// <param name="sessionToken">The session token of the user.</param>
        /// <param name="role">The role to check.</param>
        /// <returns>
        /// A Task representing the asynchronous operation that yields a boolean value indicating
        /// if the user has the specified role. True if the user has the role, false otherwise.
        /// </returns>
        public static Task<bool> HasRoleAsync(string sessionToken, string role)
        {
            return AccountManager.HasRoleAsync(sessionToken, role);
        }
        /// <summary>
        /// Asynchronously queries the roles associated with a session token.
        /// </summary>
        /// <param name="sessionToken">The session token used to authenticate the user.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the enumerable collection of role names.</returns>
        public static Task<IEnumerable<string>> QueryRolesAsync(string sessionToken)
        {
            return AccountManager.QueryRolesAsync(sessionToken);
        }
        
        /// <summary>
        /// Changes the password for a user asynchronously.
        /// </summary>
        /// <param name="sessionToken">The session token for the user.</param>
        /// <param name="oldPassword">The old password for the user.</param>
        /// <param name="newPassword">The new password for the user.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public static Task ChangePasswordAsync(string sessionToken, string oldPassword, string newPassword)
        {
            return AccountManager.ChangePasswordAsync(sessionToken, oldPassword, newPassword);
        }
        /// <summary>
        /// Changes the password for the specified email address asynchronously.
        /// </summary>
        /// <param name="sessionToken">The session token associated with the account.</param>
        /// <param name="email">The email address of the account.</param>
        /// <param name="newPassword">The new password to set.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public static Task ChangePasswordForAsync(string sessionToken, string email, string newPassword)
        {
            return AccountManager.ChangePasswordAsync(sessionToken, email, newPassword);
        }
        /// <summary>
        /// Resets the failed login count for a user asynchronously.
        /// </summary>
        /// <param name="sessionToken">The session token of the authenticated user.</param>
        /// <param name="email">The email address of the user.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public static Task ResetFailedCountForAsync(string sessionToken, string email)
        {
            return AccountManager.ResetFailedCountForAsync(sessionToken, email);
        }
        
        /// <summary>
        /// Logs out the specified user session asynchronously.
        /// </summary>
        /// <param name="sessionToken">The session token of the user.</param>
        /// <returns>A task representing the asynchronous logout operation.</returns>
        public static Task LogoutAsync(string sessionToken)
        {
            return AccountManager.LogoutAsync(sessionToken);
        }
        
        /// <summary>
        /// Retrieves an Identity asynchronously by session token and ID.
        /// </summary>
        /// <param name="sessionToken">The session token to use for authentication.</param>
        /// <param name="id">The ID of the Identity to retrieve.</param>
        /// <returns>An asynchronous task that represents the operation. The task result contains the retrieved Identity.</returns>
        /// <exception cref="Modules.Exceptions.LogicException">Thrown when the provided ID is invalid.</exception>
        public static async Task<Identity> GetIdentityByAsync(string sessionToken, IdType id)
        {
#if GENERATEDCODE_ON
            using var context = new DataContext.ProjectDbContext(sessionToken);
            var identitySet = context.IdentitySet as DataContext.Account.IdentitySet;
            var result = await identitySet!.GetByIdAsync(id).ConfigureAwait(false);

            return result != null ? Identity.CloneFrom(result) : throw new Modules.Exceptions.LogicException(CommonModules.Exceptions.ErrorType.InvalidId);
#else
            return await Task.FromResult<Identity>(new Identity()).ConfigureAwait(false);
#endif
        }
    }
}
#endif
