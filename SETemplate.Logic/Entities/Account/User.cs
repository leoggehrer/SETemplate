//@BaseCode

#if ACCOUNT_ON
namespace SETemplate.Logic.Entities.Account
{
    /// <summary>
    /// Represents an user in the account system.
    /// </summary>
#if SQLITE_ON
    [Table("Users")]
#else
    [Table("Users", Schema = "account")]
#endif
    public partial class User : VersionEntityObject
    {
        /// <summary>
        /// Gets or sets the identity ID.
        /// </summary>
        public IdType IdentityId { get; set; }
        /// <summary>
        /// Gets or sets the first name of the person.
        /// </summary>
        [MaxLength(64)]
        public string FirstName { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the last name of the person.
        /// </summary>
        [MaxLength(64)]
        public string LastName { get; set; } = string.Empty;

        #region Navigation properties
        /// <summary>
        /// Gets or sets the navigation property for the associated identity.
        /// </summary>
        internal Identity? Identity { get; set; }
        #endregion Navigation properties
    }
}
#endif
