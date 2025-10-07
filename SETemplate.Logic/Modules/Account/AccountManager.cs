//@BaseCode
#if ACCOUNT_ON
using SETemplate.Logic.Entities.Account;
using SETemplate.Logic.Modules.Exceptions;
using SETemplate.Logic.Modules.Security;
using System.Text;

namespace SETemplate.Logic.Modules.Account
{
    using PasswordRules = CommonModules.Security.PasswordRules;
    using Error = CommonModules.Exceptions.ErrorType;
    using System.Reflection;
    using SETemplate.Common.Modules.ThreadSafe;

    /// <summary>
    /// Provides functionality for managing user accounts and authentication.
    /// </summary>
    internal static partial class AccountManager
    {
        /// <summary>
        /// Static constructor for the AccountManager class.
        /// </summary>
        static AccountManager()
        {
            ClassConstructing();
            UpdateSessionAsync();
            ClassConstructed();
        }
        /// <summary>
        /// This method is called when the class is being constructed.
        /// </summary>
        static partial void ClassConstructing();
        ///<summary>
        ///This method is called when the class is constructed.
        ///</summary>
        static partial void ClassConstructed();

        /// <summary>
        /// Represents the update delay in milliseconds.
        /// </summary>
        /// <value>
        /// The update delay.
        /// </value>
        private static int UpdateDelay => 60000;
        /// <summary>
        /// Gets or sets the date and time of the last login update.
        /// </summary>
        private static DateTime LastLoginUpdate { get; set; } = DateTime.Now;
        /// <summary>
        /// Represents a thread-safe collection of login sessions.
        /// </summary>
        /// <value>
        /// The thread-safe list of <see cref="LoginSession"/>.
        /// </value>
        private static ThreadSafeList<LoginSession> LoginSessions { get; } = [];

        #region Create accounts
        /// <summary>
        /// Initializes the application access for the first identity.
        /// </summary>
        /// <param name="name">The name of the user.</param>
        /// <param name="email">The email of the user.</param>
        /// <param name="password">The password for the user.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <exception cref="AuthorizationException">Thrown when there is an authorization error.</exception>
        /// <exception cref="Exception">Thrown when an unexpected exception occurs.</exception>
        internal static async Task InitAppAccessAsync(string name, string email, string password)
        {
            email = email.RemoveLeftAndRight(' ').ToLowerInvariant();
            if (Validator.CheckEmailSyntax(email) == false)
                throw new AuthorizationException(Error.InvalidEmailSyntax);

            if (Validator.CheckPasswordSyntax(password) == false)
                throw new AuthorizationException(Error.InvalidPasswordSyntax, PasswordRules.SyntaxRoles);

#if GENERATEDCODE_ON
            using var context = new DataContext.ProjectDbContext();
            var identitySet = context.IdentitySet as DataContext.Account.IdentitySet
                            ?? throw new AuthorizationException(Error.InvalidEntitySet);
            var secureIdentitySet = context.SecureIdentitySet as DataContext.Account.SecureIdentitySet
                            ?? throw new AuthorizationException(Error.InvalidEntitySet);

            var identityCount = await identitySet.ExecuteCountAsync().ConfigureAwait(false);

            if (identityCount == 0)
            {
                var roleSet = context.RoleSet as DataContext.Account.RoleSet
                            ?? throw new AuthorizationException(Error.InvalidEntitySet);
                var identityXRoleSet = context.IdentityXRoleSet as DataContext.Account.IdentityXRoleSet
                            ?? throw new AuthorizationException(Error.InvalidEntitySet);

                try
                {
                    var (Hash, Salt) = CreatePasswordHash(password);
                    var role = new Role
                    {
                        Designation = StaticLiterals.RoleSysAdmin,
                        Description = "Created by the system (first identity).",
                    };
                    var identity = new SecureIdentity
                    {
                        Name = name,
                        Email = email,
                        PasswordHash = Hash,
                        PasswordSalt = Salt,
                    };
                    var IdentityXRole = new IdentityXRole
                    {
                        Identity = identity,
                        Role = role,
                    };

                    await roleSet.ExecuteAddAsync(role).ConfigureAwait(false);
                    await identityXRoleSet.ExecuteAddAsync(IdentityXRole).ConfigureAwait(false);
                    await secureIdentitySet.ExecuteAddAsync(identity).ConfigureAwait(false);
                    await context.ExecuteSaveChangesAsync().ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error in {MethodBase.GetCurrentMethod()!.Name}: {ex.Message}");
                    throw new AuthorizationException(Error.InitAppAccess);
                }
            }
            else
            {
                throw new AuthorizationException(Error.InitAppAccess);
            }
#else
            await Task.Delay(1000);
#endif
        }
        /// <summary>
        /// Adds a new application access with the specified parameters.
        /// </summary>
        /// <param name="sessionToken">The session token for authentication.</param>
        /// <param name="name">The name of the user.</param>
        /// <param name="email">The email of the user.</param>
        /// <param name="password">The password of the user.</param>
        /// <param name="timeOutInMinutes">The timeout in minutes for the user's session.</param>
        /// <param name="roles">The roles assigned to the user.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <exception cref="AuthorizationException">Thrown when the password syntax is invalid.</exception>
        /// <exception cref="Exception">Thrown when an error occurs during the operation.</exception>
        [Authorize("SysAdmin", "AppAdmin")]
        internal static async Task AddAppAccessAsync(string sessionToken, string name, string email, string password, int timeOutInMinutes, params string[] roles)
        {
            Authorization.CheckAuthorization(sessionToken, MethodBase.GetCurrentMethod()!.GetAsyncOriginal(), roles);

            email = email.RemoveLeftAndRight(' ').ToLowerInvariant();
            if (Validator.CheckEmailSyntax(email) == false)
                throw new AuthorizationException(Error.InvalidEmailSyntax);

            if (Validator.CheckPasswordSyntax(password) == false)
                throw new AuthorizationException(Error.InvalidPasswordSyntax, PasswordRules.SyntaxRoles);

#if GENERATEDCODE_ON
            using var context = new DataContext.ProjectDbContext();
            var identitySet = context.IdentitySet as DataContext.Account.IdentitySet
                            ?? throw new AuthorizationException(Error.InvalidEntitySet);
            var secureIdentitySet = context.SecureIdentitySet as DataContext.Account.SecureIdentitySet
                            ?? throw new AuthorizationException(Error.InvalidEntitySet);

            try
            {
                var (Hash, Salt) = CreatePasswordHash(password);
                var identity = new SecureIdentity
                {
                    Name = name,
                    Email = email,
                    PasswordHash = Hash,
                    PasswordSalt = Salt,
                    TimeOutInMinutes = timeOutInMinutes,
                };

                if (roles.Length > 0)
                {
                    var roleSet = context.RoleSet as DataContext.Account.RoleSet
                                ?? throw new AuthorizationException(Error.InvalidEntitySet);
                    var identityXRoleSet = context.IdentityXRoleSet as DataContext.Account.IdentityXRoleSet
                                ?? throw new AuthorizationException(Error.InvalidEntitySet);
                    var rolesInDb = await roleSet.ExecuteAsNoTrackingSet().ToArrayAsync().ConfigureAwait(false);

                    foreach (var role in roles)
                    {
                        var accRole = role.Trim();
                        var dbRole = rolesInDb.FirstOrDefault(r => r.Designation.Equals(accRole, StringComparison.CurrentCultureIgnoreCase));

                        if (dbRole != null)
                        {
                            var identityXRole = new IdentityXRole
                            {
                                RoleId = dbRole.Id,
                                Identity = identity,
                            };
                            await identityXRoleSet.ExecuteAddAsync(identityXRole).ConfigureAwait(false);
                        }
                        else
                        {
                            var newRole = new Role
                            {
                                Designation = accRole,
                                Description = "Created by the system.",
                            };
                            var identityXRole = new IdentityXRole
                            {
                                Role = newRole,
                                Identity = identity,
                            };
                            await roleSet.ExecuteAddAsync(newRole).ConfigureAwait(false);
                            await identityXRoleSet.ExecuteAddAsync(identityXRole).ConfigureAwait(false);
                        }
                    }
                    await secureIdentitySet.ExecuteAddAsync(identity).ConfigureAwait(false);
                }
                else
                {
                    await secureIdentitySet.ExecuteAddAsync(identity).ConfigureAwait(false);
                }
                await context.ExecuteSaveChangesAsync().ConfigureAwait(false);
            }
            catch (Exception)
            {
                throw;
            }
#else
            await Task.Delay(1000);
#endif
        }
        #endregion Create accounts

        #region Logon and Logout
        /// <summary>
        /// Asynchronously logs in a user with the specified email and password.
        /// </summary>
        /// <param name="email">The email of the user.</param>
        /// <param name="password">The password of the user.</param>
        /// <returns>A task that represents the asynchronous logon operation. The task result contains a LoginSession object representing the logged-in session.</returns>
        internal static Task<LoginSession> LoginAsync(string email, string password)
        {
            return LoginAsync(email, password, string.Empty);
        }
        /// <summary>
        /// Logs in a user asynchronously using the provided email, password, and optional information.
        /// </summary>
        /// <param name="email">The email of the user.</param>
        /// <param name="password">The password of the user.</param>
        /// <param name="optionalInfo">Optional information provided during login.</param>
        /// <returns>Returns a task representing the asynchronous operation with the logged in session.</returns>
        /// <exception cref="AuthorizationException">Thrown when the account is invalid.</exception>
        internal static async Task<LoginSession> LoginAsync(string email, string password, string optionalInfo)
        {
            email = email.RemoveLeftAndRight(' ').ToLowerInvariant();

            return await QueryLoginByEmailAsync(email, password, optionalInfo).ConfigureAwait(false)
                ?? throw new AuthorizationException(Error.InvalidAccount);
        }
        /// <summary>
        /// Logs out a user with the specified session token.
        /// </summary>
        /// <param name="sessionToken">The session token of the user.</param>
        /// <returns>A Task representing the asynchronous logout operation.</returns>
        /// <exception cref="AuthorizationException">Thrown when the authorization check fails.</exception>
        /// <remarks>
        /// This method logs out the user by updating the logout time in the database for all active sessions
        /// associated with the provided session token. It also updates the logout time for in-memory sessions
        /// with the same session token.
        /// </remarks>
        [Authorize]
        internal static async Task LogoutAsync(string sessionToken)
        {
            Authorization.CheckAuthorization(sessionToken, MethodBase.GetCurrentMethod()!.GetAsyncOriginal());

#if GENERATEDCODE_ON
            try
            {
                var saveChanges = false;
                var logoutTime = DateTime.UtcNow;
                using var context = new DataContext.ProjectDbContext();
                var identitySet = context.IdentitySet as DataContext.Account.IdentitySet
                                ?? throw new AuthorizationException(Error.InvalidEntitySet);
                var loginSessionSet = context.LoginSessionSet as DataContext.Account.LoginSessionSet
                                ?? throw new AuthorizationException(Error.InvalidEntitySet);
                var dbSessions = await loginSessionSet.ExecuteAsQuerySet()
                                                      .Where(e => e.SessionToken.Equals(sessionToken))
                                                      .ToArrayAsync()
                                                      .ConfigureAwait(false);

                foreach (var dbSession in dbSessions)
                {
                    if (dbSession != null && dbSession.IsActive)
                    {
                        saveChanges = true;
                        dbSession.LogoutTime = logoutTime;

                        await loginSessionSet.ExecuteUpdateAsync(dbSession.Id, dbSession).ConfigureAwait(false);
                    }
                }
                if (saveChanges)
                {
                    await context.ExecuteSaveChangesAsync().ConfigureAwait(false);
                }

                var memSessions = LoginSessions.Where(ls => ls.SessionToken.Equals(sessionToken));

                foreach (var memSession in memSessions)
                {
                    if (memSession != null)
                    {
                        memSession.LogoutTime = logoutTime;
                    }
                }
            }
            catch (AuthorizationException ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in {typeof(AccountManager)?.Name}: {ex.Message}");
            }
#else
            await Task.Delay(1000);
#endif
        }
        #endregion Logon and Logout

        #region Query logon data
        /// <summary>
        /// Checks if the session is alive asynchronously.
        /// </summary>
        /// <param name="sessionToken">The session token to check.</param>
        /// <returns>True if the session is alive, otherwise false.</returns>
        internal static async Task<bool> IsSessionAliveAsync(string sessionToken)
        {
            return await QueryAliveSessionAsync(sessionToken).ConfigureAwait(false) != null;
        }
        /// <summary>
        /// Queries the roles associated with the specified session token.
        /// </summary>
        /// <param name="sessionToken">The session token to authorize the request.</param>
        /// <returns>
        /// The <see cref="IEnumerable{T}"/> of role names associated with the session token.
        /// </returns>
        [Authorize]
        internal static async Task<IEnumerable<string>> QueryRolesAsync(string sessionToken)
        {
            Authorization.CheckAuthorization(sessionToken, MethodBase.GetCurrentMethod()!.GetAsyncOriginal());

            var loginSession = await QueryAliveSessionAsync(sessionToken).ConfigureAwait(false);

            return loginSession != null ? loginSession.Roles.Select(r => r.Designation) : [];
        }
        /// <summary>
        /// Checks if a user has a specific role.
        /// </summary>
        /// <param name="sessionToken">The session token of the user.</param>
        /// <param name="role">The role to check.</param>
        /// <returns>True if the user has the specified role, otherwise false.</returns>
        [Authorize]
        internal static async Task<bool> HasRoleAsync(string sessionToken, string role)
        {
            Authorization.CheckAuthorization(sessionToken, MethodBase.GetCurrentMethod()!.GetAsyncOriginal());

            var loginSession = await QueryAliveSessionAsync(sessionToken).ConfigureAwait(false);

            return loginSession != null && loginSession.Roles.Any(r => r.Designation.Equals(role, StringComparison.CurrentCultureIgnoreCase));
        }
        /// <summary>
        /// Queries the login session asynchronously.
        /// </summary>
        /// <param name="sessionToken">The session token.</param>
        /// <returns>The login session if found; otherwise, null.</returns>
        [Authorize]
        internal static Task<LoginSession?> QueryLoginAsync(string sessionToken)
        {
            Authorization.CheckAuthorization(sessionToken, MethodBase.GetCurrentMethod()!.GetAsyncOriginal());

            return QueryAliveSessionAsync(sessionToken);
        }
        #endregion Query logon data

        #region Change and reset password
        /// <summary>
        /// Changes the password for the logged-in user.
        /// </summary>
        /// <param name="sessionToken">The session token of the logged-in user.</param>
        /// <param name="oldPassword">The current password of the user.</param>
        /// <param name="newPassword">The new password to be set.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        /// <exception cref="AuthorizationException">Thrown when the user is not authorized or there is an invalid token, invalid password syntax, or invalid current password.</exception>
        [Authorize]
        internal static async Task ChangePasswordAsync(string sessionToken, string oldPassword, string newPassword)
        {
            Authorization.CheckAuthorization(sessionToken, MethodBase.GetCurrentMethod()!.GetAsyncOriginal());

            if (Validator.CheckPasswordSyntax(newPassword) == false)
                throw new AuthorizationException(Error.InvalidPasswordSyntax, PasswordRules.SyntaxRoles);

            var login = await QueryAliveSessionAsync(sessionToken).ConfigureAwait(false)
                      ?? throw new AuthorizationException(Error.InvalidToken);
#if GENERATEDCODE_ON
            using var context = new DataContext.ProjectDbContext();
            var identitySet = context.IdentitySet as DataContext.Account.IdentitySet
                            ?? throw new AuthorizationException(Error.InvalidEntitySet);
            var secureIdentitySet = context.SecureIdentitySet as DataContext.Account.SecureIdentitySet
                            ?? throw new AuthorizationException(Error.InvalidEntitySet);
            var identity = await secureIdentitySet.ExecuteGetByIdAsync(login.IdentityId)
                                                  .ConfigureAwait(false);

            if (identity != null)
            {
                if (VerifyPasswordHash(oldPassword, identity.PasswordHash, identity.PasswordSalt) == false)
                    throw new AuthorizationException(Error.InvalidPassword);

                var (Hash, Salt) = CreatePasswordHash(newPassword);

                identity.PasswordHash = Hash;
                identity.PasswordSalt = Salt;

                await secureIdentitySet.ExecuteUpdateAsync(identity.Id, identity).ConfigureAwait(false);
                await context.ExecuteSaveChangesAsync().ConfigureAwait(false);
                if (login.Identity != null)
                {
                    login.Identity.PasswordHash = Hash;
                    login.Identity.PasswordSalt = Salt;
                }
            }
#else
            await Task.Delay(1000);
#endif
        }
        /// <summary>
        /// Changes the password for a given user account.
        /// </summary>
        /// <param name="sessionToken">The session token for authorization.</param>
        /// <param name="email">The email of the user account.</param>
        /// <param name="newPassword">The new password to set.</param>
        /// <returns>A task representing the asynchronous password change operation.</returns>
        /// <exception cref="AuthorizationException">Thrown when authorization fails or an error occurs during the password change process.</exception>
        [Authorize("SysAdmin", "AppAdmin")]
        internal static async Task ChangePasswordForAsync(string sessionToken, string email, string newPassword)
        {
            Authorization.CheckAuthorization(sessionToken, MethodBase.GetCurrentMethod()!.GetAsyncOriginal());

            if (Validator.CheckPasswordSyntax(newPassword) == false)
                throw new AuthorizationException(Error.InvalidPasswordSyntax, PasswordRules.SyntaxRoles);

#if GENERATEDCODE_ON
            var login = await QueryAliveSessionAsync(sessionToken).ConfigureAwait(false)
                      ?? throw new AuthorizationException(Error.InvalidToken);

            using var context = new DataContext.ProjectDbContext();
            var identitySet = context.IdentitySet as DataContext.Account.IdentitySet
                            ?? throw new AuthorizationException(Error.InvalidEntitySet);
            var secureIdentitySet = context.SecureIdentitySet as DataContext.Account.SecureIdentitySet
                            ?? throw new AuthorizationException(Error.InvalidEntitySet);
            var identity = await secureIdentitySet.ExecuteAsQuerySet()
                                                  .FirstOrDefaultAsync(e => e.State == CommonEnums.State.Active
                                                                         && e.AccessFailedCount < 4
                                                                         && e.Email.Equals(email, StringComparison.CurrentCultureIgnoreCase))
                                                  .ConfigureAwait(false)
                            ?? throw new AuthorizationException(Error.InvalidAccount);
            var (Hash, Salt) = CreatePasswordHash(newPassword);

            identity.AccessFailedCount = 0;
            identity.PasswordHash = Hash;
            identity.PasswordSalt = Salt;

            await secureIdentitySet.ExecuteUpdateAsync(identity.Id, identity).ConfigureAwait(false);
            await context.ExecuteSaveChangesAsync().ConfigureAwait(false);
            if (login.Identity != null)
            {
                login.Identity.PasswordHash = Hash;
                login.Identity.PasswordSalt = Salt;
            }
#else
            await Task.Delay(1000);
#endif
        }
        /// <summary>
        /// Resets the failed count for a given session token and email.
        /// </summary>
        /// <param name="sessionToken">The session token for authorization.</param>
        /// <param name="email">The email associated with the account.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        [Authorize("SysAdmin", "AppAdmin")]
        internal static async Task ResetFailedCountForAsync(string sessionToken, string email)
        {
            Authorization.CheckAuthorization(sessionToken, MethodBase.GetCurrentMethod()!.GetAsyncOriginal());

            var login = await QueryAliveSessionAsync(sessionToken).ConfigureAwait(false)
                      ?? throw new AuthorizationException(Error.InvalidToken);
#if GENERATEDCODE_ON
            using var context = new DataContext.ProjectDbContext();
            var identitySet = context.IdentitySet as DataContext.Account.IdentitySet
                            ?? throw new AuthorizationException(Error.InvalidEntitySet);
            var identity = await identitySet.ExecuteAsQuerySet()
                                            .FirstOrDefaultAsync(e => e.State == CommonEnums.State.Active
                                                                   && e.Email.Equals(email, StringComparison.CurrentCultureIgnoreCase))
                                            .ConfigureAwait(false)
                            ?? throw new AuthorizationException(Error.InvalidAccount);

            identity.AccessFailedCount = 0;
            await identitySet.ExecuteUpdateAsync(identity.Id, identity).ConfigureAwait(false);
            await context.ExecuteSaveChangesAsync().ConfigureAwait(false);
#else
            await Task.Delay(1000);
#endif
        }
        #endregion Change and reset password

        #region Internal logon
        /// <summary>
        /// Queries the login session based on the provided session token.
        /// </summary>
        /// <param name="sessionToken">The session token to search for.</param>
        /// <returns>
        /// The active login session matching the provided session token, if found; otherwise, null.
        /// </returns>
        internal static LoginSession? QueryLoginSession(string sessionToken)
        {
            return LoginSessions.FirstOrDefault(ls => ls.IsActive && ls.SessionToken.Equals(sessionToken));
        }
        /// <summary>
        /// Queries the alive session based on the given session token.
        /// </summary>
        /// <param name="sessionToken">The session token to query.</param>
        /// <returns>The alive session if found, null otherwise.</returns>
        internal static async Task<LoginSession?> QueryAliveSessionAsync(string sessionToken)
        {
            var result = LoginSessions.FirstOrDefault(ls => ls.IsActive && ls.SessionToken.Equals(sessionToken));

#if GENERATEDCODE_ON
            if (result == null)
            {
                using var context = new DataContext.ProjectDbContext();
                var loginSessionSet = context.LoginSessionSet as DataContext.Account.LoginSessionSet
                                ?? throw new AuthorizationException(Error.InvalidEntitySet);
                var session = await loginSessionSet.ExecuteAsQuerySet()
                                                   .FirstOrDefaultAsync(e => e.SessionToken.Equals(sessionToken))
                                                   .ConfigureAwait(false);

                if (session != null && session.IsActive)
                {
                    var secureIdentitySet = context.SecureIdentitySet as DataContext.Account.SecureIdentitySet
                                    ?? throw new AuthorizationException(Error.InvalidEntitySet);
                    var identity = await secureIdentitySet.ExecuteAsQuerySet()
                                                          .Include(e => e.IdentityXRoles)
                                                          .ThenInclude(e => e.Role)
                                                          .FirstOrDefaultAsync(e => e.Id == session.IdentityId)
                                                          .ConfigureAwait(false);

                    if (identity != null)
                    {
                        session.Name = identity.Name;
                        session.Email = identity.Email;
                        session.Identity = identity;
                        session.Roles.AddRange(identity.IdentityXRoles.Select(e => e.Role!));

                        result = session.Clone();
                        LoginSessions.Add(session);
                    }
                }
            }
            return result;
#else
            return await Task.FromResult(result);
#endif
        }
        /// <summary>
        /// Queries the alive session for the specified email and password.
        /// </summary>
        /// <param name="email">The email associated with the session.</param>
        /// <param name="password">The password used to verify the session.</param>
        /// <returns>
        /// A task representing the asynchronous operation. The task result is a nullable LoginSession object.
        /// The result is null if the session was not found or the password verification failed.
        /// </returns>
        internal static async Task<LoginSession?> QueryAliveSessionAsync(string email, string password)
        {
            var result = LoginSessions.FirstOrDefault(e => e.IsActive && e.Email.Equals(email, StringComparison.CurrentCultureIgnoreCase));

#if GENERATEDCODE_ON
            if (result == null)
            {
                using var context = new DataContext.ProjectDbContext();
                var secureIdentitySet = context.SecureIdentitySet as DataContext.Account.SecureIdentitySet
                                ?? throw new AuthorizationException(Error.InvalidEntitySet);
                var identity = await secureIdentitySet.ExecuteAsQuerySet()
                                                .Include(e => e.IdentityXRoles)
                                                .ThenInclude(e => e.Role)
                                                .FirstOrDefaultAsync(e => e.Email == email
                                                                       && e.State == CommonEnums.State.Active
                                                                       && e.AccessFailedCount < 4)
                                                .ConfigureAwait(false);

                if (identity != null && VerifyPasswordHash(password, identity.PasswordHash, identity.PasswordSalt))
                {
                    var loginSessionSet = context.LoginSessionSet as DataContext.Account.LoginSessionSet
                                    ?? throw new AuthorizationException(Error.InvalidEntitySet);
                    var session = await loginSessionSet.ExecuteAsQuerySet()
                                                       .FirstOrDefaultAsync(e => e.IdentityId == identity.Id
                                                                              && e.LogoutTime == null)
                                                       .ConfigureAwait(false);

                    if (session != null && session.IsActive)
                    {
                        session.Name = identity.Name;
                        session.Email = identity.Email;
                        session.Identity = identity;
                        session.Roles.AddRange(identity.IdentityXRoles.Select(e => e.Role!));

                        result = session.Clone();
                        LoginSessions.Add(session);
                    }
                }
            }
            return result;
#else
            return await Task.FromResult(result);
#endif
        }
        /// <summary>
        /// Queries the login session by email asynchronously.
        /// </summary>
        /// <param name="email">The email of the user.</param>
        /// <param name="password">The password of the user.</param>
        /// <param name="optionalInfo">Optional additional information.</param>
        /// <returns>A task representing the asynchronous operation. The task result contains the LoginSession object if successful, otherwise null.</returns>
        internal static async Task<LoginSession?> QueryLoginByEmailAsync(string email, string password, string optionalInfo)
        {
            var result = default(LoginSession);
            var querySession = await QueryAliveSessionAsync(email, password).ConfigureAwait(false);

#if GENERATEDCODE_ON
            if (querySession == null)
            {
                using var context = new DataContext.ProjectDbContext();
                var secureIdentitySet = context.SecureIdentitySet as DataContext.Account.SecureIdentitySet
                                ?? throw new AuthorizationException(Error.InvalidEntitySet);
                var identity = await secureIdentitySet.ExecuteAsQuerySet()
                                                .Include(e => e.IdentityXRoles)
                                                .ThenInclude(e => e.Role)
                                                .FirstOrDefaultAsync(e => e.Email == email
                                                                       && e.State == CommonEnums.State.Active
                                                                       && e.AccessFailedCount < 4)
                                                .ConfigureAwait(false);

                if (identity != null && VerifyPasswordHash(password, identity.PasswordHash, identity.PasswordSalt))
                {
                    var identitySet = context.IdentitySet as DataContext.Account.IdentitySet
                                    ?? throw new AuthorizationException(Error.InvalidEntitySet);
                    var loginSessionSet = context.LoginSessionSet as DataContext.Account.LoginSessionSet
                                    ?? throw new AuthorizationException(Error.InvalidEntitySet);
                    var loginSession = new LoginSession
                    {
                        SessionToken = $"{Guid.NewGuid()}-{Guid.NewGuid()}",
                        IdentityId = identity.Id,
                        Name = identity.Name,
                        Email = identity.Email,
                        OptionalInfo = optionalInfo,
                        Identity = identity,
                    };
                    loginSession.Roles.AddRange(identity.IdentityXRoles.Select(e => e.Role!));

                    AfterQueryLoginByEmail(identity, loginSession);

                    var entity = await loginSessionSet.ExecuteAddAsync(loginSession).ConfigureAwait(false);

                    if (identity.AccessFailedCount > 0)
                    {
                        identity.AccessFailedCount = 0;
                        await identitySet.ExecuteUpdateAsync(identity.Id, identity).ConfigureAwait(false);
                    }
                    await context.ExecuteSaveChangesAsync().ConfigureAwait(false);

                    result = entity.Clone();
                    LoginSessions.Add(entity);
                }
            }
            else if (VerifyPasswordHash(password, querySession.PasswordHash, querySession.PasswordSalt))
            {
                querySession.LastAccess = DateTime.UtcNow;
                result = querySession.Clone();
            }
            return result;
#else
            return await Task.FromResult(result);
#endif
        }
        static partial void AfterQueryLoginByEmail(Identity identity, LoginSession session);
        #endregion Internal logon

        #region Update thread
        /// <summary>
        /// Update open login sessions asynchronously.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <remarks>
        /// This method updates the open login sessions by comparing the sessions stored in memory
        /// with the sessions stored in the database. It checks for any changes in last access time and
        /// logout status of the sessions, and updates the database accordingly. If any changes are made,
        /// the changes are saved to the database. Finally, it updates the last login update time
        /// and waits for a specified delay before repeating the process.
        /// </remarks>
        private static Task UpdateSessionAsync()
        {
            return Task.Run(async () =>
            {
                while (true)
                {
                    try
                    {
#if GENERATEDCODE_ON
                        using var context = new DataContext.ProjectDbContext();
                        var loginSessionSet = context.LoginSessionSet as DataContext.Account.LoginSessionSet
                                           ?? throw new AuthorizationException(Error.InvalidEntitySet);
                        var saveChanges = false;
                        var dbSessions = await loginSessionSet.ExecuteAsQuerySet()
                                                              .Where(e => e.LogoutTime.HasValue == false)
                                                              .ToArrayAsync()
                                                              .ConfigureAwait(false);
                        var uncheckSessions = LoginSessions.Where(i => dbSessions.Length == 0
                                                                    || dbSessions.Any(e => e.Id != i.Id));

                        foreach (var dbSession in dbSessions)
                        {
                            var dbUpdate = false;
                            var memSessionRemove = false;
                            var memSession = LoginSessions.FirstOrDefault(e => e.Id == dbSession.Id);

                            if (memSession != null && dbSession.LastAccess != memSession.LastAccess)
                            {
                                dbUpdate = true;
                                dbSession.LastAccess = memSession.LastAccess;
                            }
                            if (dbSession.IsTimeout)
                            {
                                dbUpdate = true;
                                if (memSession != null)
                                {
                                    memSessionRemove = true;
                                }
                                if (dbSession.LogoutTime.HasValue == false)
                                {
                                    dbSession.LogoutTime = DateTime.UtcNow;
                                }
                            }
                            if (dbUpdate)
                            {
                                saveChanges = true;
                                await loginSessionSet.ExecuteUpdateAsync(dbSession.Id, dbSession).ConfigureAwait(false);
                            }
                            if (memSessionRemove && memSession != null)
                            {
                                LoginSessions.Remove(memSession);
                            }
                        }
                        if (saveChanges)
                        {
                            await context.ExecuteSaveChangesAsync().ConfigureAwait(false);
                        }
                        foreach (var memItem in uncheckSessions)
                        {
                            var dbSession = await loginSessionSet.ExecuteAsQuerySet()
                                                                 .FirstOrDefaultAsync(e => e.Id == memItem.Id)
                                                                 .ConfigureAwait(false);

                            if (dbSession != null)
                            {
                                memItem.LastAccess = dbSession.LastAccess;
                                memItem.LogoutTime = dbSession.LogoutTime;
                            }
                        }
#endif
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error in {typeof(AccountManager)?.Name}: {ex.Message}");
                    }
                    LastLoginUpdate = DateTime.Now;
                    await Task.Delay(UpdateDelay).ConfigureAwait(false);
                }
            });
        }
        #endregion Update thread

        #region Helpers
        /// <summary>
        /// Creates a password hash and salt using HMACSHA512 algorithm.
        /// </summary>
        /// <param name="password">The password to be hashed.</param>
        /// <returns>A tuple containing the password hash and salt.</returns>
        internal static (byte[] Hash, byte[] Salt) CreatePasswordHash(string password)
        {
            using var hmac = new System.Security.Cryptography.HMACSHA512();

            var passwordSalt = hmac.Key;
            var passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            return (passwordHash, passwordSalt);
        }
        /// <summary>
        /// Verifies the given password against the stored password hash and password salt.
        /// </summary>
        /// <param name="password">The password to verify.</param>
        /// <param name="passwordHash">The stored password hash.</param>
        /// <param name="passwordSalt">The stored password salt.</param>
        /// <returns>True if the password matches the stored password hash, otherwise false.</returns>
        internal static bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            var result = computedHash.Length == passwordHash.Length;

            for (int i = 0; i < passwordHash.Length && result; i++)
            {
                result = passwordHash[i] == computedHash[i];
            }
            return result;
        }
        #endregion Helpers
    }
}
#endif
