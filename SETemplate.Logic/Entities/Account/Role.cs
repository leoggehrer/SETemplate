//@BaseCode
#if ACCOUNT_ON
namespace SETemplate.Logic.Entities.Account
{
    /// <summary>
    /// Represents a rule in the account system.
    /// </summary>
#if SQLITE_ON
    [Table("Roles")]
#else
    [Table("Roles", Schema = "account")]
#endif
    [Index(nameof(Designation), IsUnique = true)]
    internal partial class Role : VersionEntityObject
    {
        /// <summary>
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
