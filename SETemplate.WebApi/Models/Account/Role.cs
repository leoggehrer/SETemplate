//@BaseCode
#if ACCOUNT_ON
namespace SETemplate.WebApi.Models.Account
{
    /// <summary>
    /// This model represents an account role.
    /// </summary>
    public partial class Role : ModelObject
    {
        /// <summary>
        /// Gets and sets a role designation.
        /// </summary>
        public string Designation { get; set; } = string.Empty;
        /// <summary>
        /// Gets and sets a role description.
        /// </summary>
        public string? Description { get; set; } = string.Empty;
    }
}
#endif
