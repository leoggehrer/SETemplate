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
#if GUID_OFF
        /// <summary>
        /// Gets or sets the unique identifier.
        /// </summary>
        public Guid Guid { get; set; }
#endif
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
        /// Gets or sets the confirmation code.
        /// </summary>
        public string? ConfirmationCode { get; set; } = null;
        /// <summary>
        /// Gets or sets the date and time until the confirmation code is valid.
        /// </summary>
        public DateTime? ConfirmationCodeValidUntil { get; set; }
        /// <summary>
        /// Gets or sets the date and time when the account was activated.
        /// </summary>
        public DateTime? ActivatedOn { get; set; }
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
        public CommonEnums.State State { get; set; } = Common.Enums.State.Active;

        #region Navigation properties
        /// <summary>
        /// Gets or sets the list of IdentityXRole objects associated with this entity.
        /// </summary>
        public List<IdentityXRole> IdentityXRoles { get; internal set; } = [];
        #endregion Navigation properties

        /// <summary>
        /// Checks if the user has a role with the specified GUID.
        /// </summary>
        /// <param name="guid">The GUID of the role to check.</param>
        /// <returns>True if the user has the role, otherwise false.</returns>
        public bool HasRole(Guid guid)
        {
            return IdentityXRoles.Any(iXr => iXr.Role != null && iXr.Role.Guid == guid);
        }
    }
}
#endif
