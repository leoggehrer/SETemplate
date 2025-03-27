//@BaseCode
//MdStart
#if ACCOUNT_ON
namespace SETemplate.Logic.Entities.Account
{
    /// <summary>
    /// Represents a secure identity in the account system.
    /// </summary>
#if SQLITE_ON
    [System.ComponentModel.DataAnnotations.Schema.Table("SecureIdentities")]
#else
    [System.ComponentModel.DataAnnotations.Schema.Table("SecureIdentities", Schema = "account")]
#endif
    [Microsoft.EntityFrameworkCore.Index(nameof(Email), IsUnique = true)]
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
        
        #region Transient properties
        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        /// <value>The password.</value>
        /// <remarks>The password is not mapped to the database.</remarks>
        [NotMapped]
        public string Password { get; set; } = string.Empty;
        #endregion Transient properties
    }
}
#endif
//MdEnd
