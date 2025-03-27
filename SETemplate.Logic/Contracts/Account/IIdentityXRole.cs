//@BaseCode
//MdStart
#if ACCOUNT_ON
namespace SETemplate.Logic.Contracts.Account
{
    /// <summary>
    /// Represents a relationship between an identity and a role.
    /// </summary>
    public partial interface IIdentityXRole
    {
        /// <summary>
        /// Gets or sets the identity ID.
        /// </summary>
        IdType IdentityId { get; set; }
        /// <summary>
        /// Gets or sets the role ID.
        /// </summary>
        IdType RoleId { get; set; }
    }
}
#endif
//MdEnd
