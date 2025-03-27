//@BaseCode
//MdStart
#if ACCOUNT_ON
namespace SETemplate.Logic.Entities.Account
{
    using SETemplate.Logic.Contracts.Account;
    
    /// <summary>
    /// Represents a rule in the account system.
    /// </summary>
#if SQLITE_ON
    [System.ComponentModel.DataAnnotations.Schema.Table("Roles")]
#else
    [System.ComponentModel.DataAnnotations.Schema.Table("Roles", Schema = "account")]
#endif
    [Microsoft.EntityFrameworkCore.Index(nameof(Designation), IsUnique = true)]
    public partial class Role : EntityObject, IRole
    {
#if GUID_OFF
        /// <summary>
        /// Gets or sets the unique identifier.
        /// </summary>
        public Guid Guid { get; internal set; }
#endif
        /// <summary>
        /// Gets or sets the designation of a person.
        /// </summary>
        [MaxLength(64)]
        public string Designation { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        [MaxLength(256)]
        public string? Description { get; set; }
    }
}
#endif
//MdEnd
