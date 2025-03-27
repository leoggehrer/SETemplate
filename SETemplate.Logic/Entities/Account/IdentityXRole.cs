//@BaseCode
//MdStart
#if ACCOUNT_ON
namespace SETemplate.Logic.Entities.Account
{
    using SETemplate.Logic.Contracts.Account;
    
    /// <summary>
    /// Represents a identity to role in the account system.
    /// </summary>
#if SQLITE_ON
    [System.ComponentModel.DataAnnotations.Schema.Table("IdentityXRoles")]
#else
    [System.ComponentModel.DataAnnotations.Schema.Table("IdentityXRoles", Schema = "account")]
#endif
    [Microsoft.EntityFrameworkCore.Index(nameof(IdentityId), nameof(RoleId), IsUnique = true)]
    public partial class IdentityXRole : EntityObject, IIdentityXRole
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
//MdEnd
