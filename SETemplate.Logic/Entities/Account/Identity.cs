//@BaseCode
#if ACCOUNT_ON
namespace SETemplate.Logic.Entities.Account
{
    /// <summary>
    /// Represents an identity in the account system.
    /// </summary>
#if SQLITE_ON
        [Table("Identities")]
#else
    [Table("Identities", Schema = "account")]
#endif
    [Index(nameof(Email), IsUnique = true)]
    internal partial class Identity : VersionEntityObject
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        [MaxLength(128)]
        public string Name { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the email address.
        /// </summary>
        [MaxLength(128)]
        public string Email { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the time-out value in minutes.
        /// </summary>
        public int TimeOutInMinutes { get; set; } = 30;
        /// <summary>
        /// Gets or sets the number of failed access attempts for the user.
        /// </summary>
        public int AccessFailedCount { get; set; }
        /// <summary>
        /// Gets or sets the state of the object.
        /// </summary>
        public CommonEnums.State State { get; set; } = CommonEnums.State.Active;

        #region Navigation properties
        /// <summary>
        /// Gets or sets the list of IdentityXRole objects associated with this entity.
        /// </summary>
        public List<IdentityXRole> IdentityXRoles { get; internal set; } = [];
        #endregion Navigation properties
    }
}
#endif
