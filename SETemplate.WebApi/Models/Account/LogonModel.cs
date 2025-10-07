//@BaseCode
#if ACCOUNT_ON
namespace SETemplate.WebApi.Models.Account
{
    /// <summary>
    /// This model represents the logon data.
    /// </summary>
    public partial class LogonModel
    {
        /// <summary>
        /// Gets or sets the email for logon.
        /// </summary>
        public string Email { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the password for logon.
        /// </summary>
        public string Password { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets optional info data.
        /// </summary>
        public string? Info { get; set; }
    }
}
#endif
