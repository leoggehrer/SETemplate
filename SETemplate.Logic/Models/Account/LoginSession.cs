//@BaseCode
#if ACCOUNT_ON
namespace SETemplate.Logic.Models.Account
{
    /// <summary>
    /// Represents a login session model.
    /// </summary>
    public partial class LoginSession : ModelObject
    {
        ///<summary>
        /// Gets or sets the identity ID.
        ///</summary>
        public IdType IdentityId { get; internal set; }
        /// <summary>
        /// Gets or sets the session token.
        /// </summary>
        /// <value>The session token.</value>
        public string SessionToken { get; internal set; } = string.Empty;
        /// <summary>
        /// Gets or sets the login time of the user.
        /// </summary>
        public DateTime LoginTime { get; internal set; }
        /// <summary>
        /// Gets or sets the logout time of the user.
        /// </summary>
        public DateTime? LogoutTime { get; internal set; }
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; internal set; } = string.Empty;
        /// <summary>
        /// Gets or sets the email address value.
        /// </summary>
        public string Email { get; internal set; } = string.Empty;
        /// <summary>
        /// Gets or sets the optional information.
        /// </summary>
        public string? OptionalInfo { get; internal set; }
        /// <summary>
        /// Gets or sets the array of roles.
        /// </summary>
        public Role[] Roles { get; internal set; } = [];

        /// <summary>
        /// Clones a LoginSession entity to a LoginSession model.
        /// </summary>
        /// <param name="entity">The LoginSession entity to clone from.</param>
        /// <returns>A new LoginSession model with values copied from the entity.</returns>
        internal static LoginSession CloneFrom(Entities.Account.LoginSession entity)
        {
            return new LoginSession
            {
                IdentityId = entity.IdentityId,
                SessionToken = entity.SessionToken,
                LoginTime = entity.LoginTime,
                LogoutTime = entity.LogoutTime,
                Name = entity.Name,
                Email = entity.Email,
                OptionalInfo = entity.OptionalInfo,
                Roles = [.. entity.Roles.Select(Role.CloneFrom)]
            };
        }
    }
}
#endif
