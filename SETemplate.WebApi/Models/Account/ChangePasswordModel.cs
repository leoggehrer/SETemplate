//@BaseCode
#if ACCOUNT_ON
namespace SETemplate.WebApi.Models.Account
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// This model represents the data required to change a user's password.
    /// </summary>
    public partial class ChangePasswordModel
    {
        /// <summary>
        /// Gets or sets the current (old) password.
        /// </summary>
        [Required]
        public string OldPassword { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the new password.
        /// </summary>
        [Required]
        [StringLength(128, MinimumLength = 8)]
        public string NewPassword { get; set; } = string.Empty;
    }
}
#endif
