//@BaseCode
#if ACCOUNT_ON
namespace SETemplate.Logic.Entities.Account
{
    /// <summary>
    /// Represents a secure identity in the account system.
    /// </summary>
#if SQLITE_ON
    [Table("SecureIdentities")]
#else
    [Table("SecureIdentities", Schema = "account")]
#endif
    [Index(nameof(Email), IsUnique = true)]
    internal partial class SecureIdentity : Identity
    {
        /// <summary>
        /// Gets or sets the password hash for the user.
        /// </summary>
        /// <value>
        /// The password hash as an array of bytes.
        /// </value>
        [Required]
        [MaxLength(512)]
        public byte[] PasswordHash { get; set; } = [];
        /// <summary>
        /// Gets or sets the password salt.
        /// </summary>
        /// <value>
        /// The password salt.
        /// </value>
        [Required]
        [MaxLength(512)]
        public byte[] PasswordSalt { get; set; } = [];
    }
}
#endif

