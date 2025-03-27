//@BaseCode
//MdStart
#if ACCOUNT_ON
namespace SETemplate.Logic.Contracts.Account
{
    /// <summary>
    /// Represents a role.
    /// </summary>
    public partial interface IRole
    {
        /// <summary>
        /// Gets the Id of the object.
        /// </summary>
        IdType Id { get; }
        /// <summary>
        /// Gets the GUID value.
        /// </summary>
        Guid Guid { get; }
        /// <summary>
        /// Gets the designation of an object.
        /// </summary>
        string Designation { get; }
        /// <summary>
        /// Gets the description of the object.
        /// </summary>
        string? Description { get; }
    }
}
#endif
//MdEnd
