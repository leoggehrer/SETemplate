//@BaseCode
namespace SETemplate.Common.Models.Accounts
{
    /// <summary>
    /// Represents a logon session for a user.
    /// </summary>
    public partial class LogonSession
    {
        /// <summary>
        /// Gets or sets the identity id.
        /// </summary>
        /// <value>The identity id.</value>
        public IdType IdentityId { get; set; }
        /// <summary>
        /// Gets or sets the session token.
        /// </summary>
        /// <value>
        /// The session token.
        /// </value>
        public string SessionToken { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the login time of the user.
        /// </summary>
        /// <value>
        /// The login time.
        /// </value>
        public DateTime LoginTime { get; set; }
        /// <summary>
        /// Gets or sets the logout time of the user.
        /// </summary>
        /// <value>
        /// The logout time of the user. It can be null if the user is still logged in.
        /// </value>
        public DateTime? LogoutTime { get; set; }
        /// Gets or sets the name.
        public string Name { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the email address.
        /// </summary>
        /// <value>
        /// The email address. The default value is an empty string.
        /// </value>
        public string Email { get; set; } = string.Empty;
        ///<summary>
        /// Gets or sets the optional information.
        ///</summary>
        ///<value>
        ///The optional information.
        ///</value>
        public string? OptionalInfo { get; set; }
        ///<summary>
        /// Gets or sets the array of roles.
        ///</summary>
        public Role[] Roles { get; set; } = [];
    }
}
