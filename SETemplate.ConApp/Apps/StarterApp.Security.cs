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

        static (string UserName, string Email, string Password, int Timeout, string Role) CreateAccountData(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
                throw new ArgumentException("Benutzername darf nicht leer sein.", nameof(userName));

            string email = $"{userName.ToLower()}@{EmailPostfix.ToLower()}";
            string password = $"{PwdPrefix}{userName}";
            string role = userName;

            return (userName, email, password, 30, role);
        }

        partial void CreateAccounts()
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
    }
}
#endif
