//@BaseCode
#if ACCOUNT_ON
namespace SETemplate.WebApi.Models.Account
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// This model represents the logon data.
    /// </summary>
    public partial class LogonModel
    {
        /// <summary>
        /// Gets or sets the email for logon.
        /// </summary>
        [Required]
        [EmailAddress]
        [StringLength(256)]
        public string Email { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the password for logon.
        /// </summary>
        [Required]
        [StringLength(128, MinimumLength = 4)]
        public string Password { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets optional info data.
        /// </summary>
        [StringLength(500)]
        public string? Info { get; set; }
    }
}
#endif
