//@BaseCode
#if ACCOUNT_ON
namespace SETemplate.Logic.Entities.Account
{
    /// <summary>
    /// Represents a identity to role in the account system.
    /// </summary>
#if SQLITE_ON
    [Table("IdentityXRoles")]
#else
    [Table("IdentityXRoles", Schema = "account")]
#endif
    [Index(nameof(IdentityId), nameof(RoleId), IsUnique = true)]
    internal partial class IdentityXRole : VersionEntityObject
    {
        /// <summary>
        /// Gets or sets the identity ID.
        /// </summary>
        /// <value>The identity ID.</value>
        public IdType IdentityId { get; set; }
        ///<summary>
        ///Gets or sets the role ID.
        ///</summary>
        public IdType RoleId { get; set; }
        
        /// <summary>
        /// Gets or sets the identity navigation.
        /// </summary>
        public Identity? Identity { get; set; }
        /// <summary>
        /// Gets or sets the role navigation.
        /// </summary>
        public Role? Role { get; set; }
    }
}
#endif
