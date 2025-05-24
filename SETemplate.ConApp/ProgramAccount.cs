//@BaseCode
#if ACCOUNT_ON
namespace SETemplate.ConApp
{
    partial class Program
    {
        /// <summary>
        /// Gets or sets the SA user.
        /// </summary>
        private static string SaUser => "LeoAdmin";
        /// <summary>
        /// Gets or sets the system administrator email address.
        /// </summary>
        private static string SaEmail => "LeoAdmin.SETemplate@gmx.at";
        /// <summary>
        /// Gets the password for Sa account.
        /// </summary>
        private static string SaPwd => "1234LeoAdmin";

        /// <summary>
        /// The username of the AppAdmin user.
        /// </summary>
        /// <value>The value is fixed as "AppAdmin".</value>
        private static string AaUser => "AppAdmin";
        /// <summary>
        /// Gets the email address for the AppAdmin SETemplate.
        /// </summary>
        private static string AaEmail => "AppAdmin.SETemplate@gmx.at";
        /// <summary>
        /// Gets or sets the password for the AaPwd.
        /// </summary>
        private static string AaPwd => "1234AppAdmin";
        /// <summary>
        /// Gets the value "AppAdmin" representing the AA role.
        /// </summary>
        private static string AaRole => "AppAdmin";

        /// <summary>
        /// Gets the AppUser property.
        /// </summary>
        private static string AppUser => "AppUser";
        /// <summary>
        /// Represents the email address used by the application.
        /// </summary>
        private static string AppEmail => "AppUser.SETemplate@gmx.at";

        /// <summary>
        /// Gets or sets the application password.
        /// </summary>
        private static string AppPwd => "1234AppUser";
        /// <summary>
        /// Gets the application role.
        /// </summary>
        private static string AppRole => "AppUser";

        static partial void CreateAccounts()
        {
            Task.Run(async () =>
            {
                await Logic.AccountAccess.InitAppAccessAsync(SaUser, SaEmail, SaPwd);
                await AddAppAccessAsync(SaEmail, SaPwd, AaUser, AaEmail, AaPwd, 30, AaRole);
                await AddAppAccessAsync(SaEmail, SaPwd, AppUser, AppEmail, AppPwd, 35, AppRole);
                await AddAppAccessAsync(SaEmail, SaPwd, "g.gehrer", "   g.gehrer@htl-leonding.ac.at ", AppPwd, 35, AppRole);
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
