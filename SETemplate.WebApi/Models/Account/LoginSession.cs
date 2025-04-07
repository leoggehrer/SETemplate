//@BaseCode
#if ACCOUNT_ON
namespace SETemplate.WebApi.Models.Account
{
    /// <summary>
    /// This model represents a login session.
    /// </summary>
    public partial class LoginSession
    {
        /// <summary>
        /// Gets the property data.
        /// </summary>
        public IdType IdentityId { get; init; }
        /// <summary>
        /// Gets the property data.
        /// </summary>
        public string SessionToken { get; init; } = string.Empty;
        /// <summary>
        /// Gets the property data.
        /// </summary>
        public DateTime LoginTime { get; init; }
        /// <summary>
        /// Gets the property data.
        /// </summary>
        public string Name { get; init; } = string.Empty;
        /// <summary>
        /// Gets the property data.
        /// </summary>
        public string Email { get; init; } = string.Empty;
        /// <summary>
        /// Gets the property data.
        /// </summary>
        public string? OptionalInfo { get; init; }
        /// <summary>
        /// Gets the property data.
        /// </summary>
        public Role[] Roles { get; init; } = [];
    }
}
#endif
