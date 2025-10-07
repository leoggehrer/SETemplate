//@BaseCode
namespace SETemplate.Common.Models.Accounts
{
    /// <summary>
    /// Represents a logon class.
    /// </summary>
    public partial class Logon
    {
        /// <summary>
        /// Gets or sets the email address.
        /// </summary>
        public string Email { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        public string Password { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the information.
        /// </summary>
        public string Info { get; set; } = string.Empty;
    }
}
