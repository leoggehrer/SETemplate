#if ACCOUNT_ON
namespace SETemplate.ConApp.Apps
{
    partial class StarterApp
    {
        private static string EmailPostfix = "gmx.at";
        private static string PwdPrefix = "1234";

        /// <summary>
        /// Gets or sets the SA user.
        /// </summary>
        private static string SAUser => "SysAdmin";
        /// <summary>
        /// Gets or sets the system administrator email address.
        /// </summary>
        private static string SAEmail => SAUser + "@" + EmailPostfix;
        /// <summary>
        /// Gets the password for Sa account.
        /// </summary>
        private static string SAPwd => PwdPrefix + SAUser;

        /// <summary>
        /// Creates account data based on the provided username.
        /// </summary>
        /// <param name="userName">The username for which to create account data.</param>
        /// <returns>A tuple containing the username, email, password, timeout, and role.</returns>
        private static (string UserName, string Email, string Password, int Timeout, string Role) CreateAccountData(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
                throw new ArgumentException("Benutzername darf nicht leer sein.", nameof(userName));

            string email = $"{userName.ToLower()}@{EmailPostfix.ToLower()}";
            string password = $"{PwdPrefix}{userName}";
            string role = userName;

            return (userName, email, password, 30, role);
        }
        /// <summary>
        /// Creates a user account by prompting for input.
        /// </summary>
        static partial void CreateAccount()
        {
            static string ReadInput(string prompt)
            {
                Console.Write(prompt);
                return Console.ReadLine() ?? string.Empty;
            }
            PrintLine("Create an user account:");
            PrintLine("");

            var name = ReadInput("Name:     ");
            var email = ReadInput("Email:    ");
            var password = ReadInput("Password: ");
            var roles = ReadInput("Role(s) (comma separated): ");
                        
            Task.Run(async () =>
            {
                try
                {
                    await AddAppAccessAsync(SAEmail, SAPwd, name, email, password, 30, roles.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries));
                    PrintLine("");
                    PrintLine($"Account for user '{name}' created successfully.");
                }
                catch (Exception ex)
                {
                    var saveColor = ForegroundColor;
                    PrintLine("");
                    ForegroundColor = ConsoleColor.Red;
                    PrintLine($"Error during account creation: {ex.Message}");
                    if (ex.InnerException != null)
                    {
                        PrintLine($"Inner exception: {ex.InnerException.Message}");
                    }
                    ForegroundColor = saveColor;
                }
            }).Wait();
        }
        /// <summary>
        /// Creates predefined accounts for the application.
        /// </summary>
        static partial void CreateAccounts()
        {
            Task.Run(async () =>
            {
                await Logic.AccountAccess.InitAppAccessAsync(SAUser, SAEmail, SAPwd);

                var account = CreateAccountData("AppAdmin");

                await AddAppAccessAsync(SAEmail, SAPwd, account.UserName, account.Email, account.Password, account.Timeout, account.Role);

                account = CreateAccountData("AppUser");
                await AddAppAccessAsync(SAEmail, SAPwd, account.UserName, account.Email, account.Password, account.Timeout, account.Role);

                account = CreateAccountData("G.Gehrer");
                await AddAppAccessAsync(SAEmail, SAPwd, account.UserName, "   g.gehrer@htl-leonding.ac.at ", account.Password, account.Timeout, account.Role);
            }).Wait();
            AfterCreateAccountsCustom();
        }

        /// <summary>
        /// Adds application access for a user.
        /// </summary>
        /// <param name="loginEmail">The email of the user logging in.</param>
        /// <param name="loginPwd">The password of the user logging in.</param>
        /// <param name="user">The username of the user being granted access.</param>
        /// <param name="email">The email of the user being granted access.</param>
        /// <param name="pwd">The password of the user being granted access.</param>
        /// <param name="timeOutInMinutes">The timeout duration in minutes for the access.</param>
        /// <param name="roles">A string array representing the roles for the user.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        private static async Task AddAppAccessAsync(string loginEmail, string loginPwd, string user, string email, string pwd, int timeOutInMinutes, params string[] roles)
        {
            var login = await Logic.AccountAccess.LoginAsync(loginEmail, loginPwd, string.Empty);

            await Logic.AccountAccess.AddAppAccessAsync(login!.SessionToken, user, email, pwd, timeOutInMinutes, roles);
            await Logic.AccountAccess.LogoutAsync(login!.SessionToken);
        }

        #region partial methods
        static partial void AfterCreateAccountsCustom();
        #endregion partial methods
    }
}
#endif