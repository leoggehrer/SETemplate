//@BaseCode
namespace SETemplate.Common.Models.Accounts
{
    /// <summary>
    /// Represents a role.
    /// </summary>
    public partial class Role
    {
        /// <summary>
        /// Gets or sets the identifier type.
        /// </summary>
        public IdType Id { get; set; }
        /// <summary>
        /// Gets or sets the designation.
        /// </summary>
        /// <value>The designation value. The default value is an empty string.</value>
        public String Designation { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The optional description. Returns an empty string if not set.</value>
        public String? Description { get; set; } = string.Empty;
    }
}
