//@BaseCode
using System;

namespace SETemplate.MVVMApp.Models.Account
{
    public partial class LogonSession: ModelObject
    {
        ///<summary>
        /// Gets or sets the identity ID.
        ///</summary>
        public IdType IdentityId { get; set; }
        /// <summary>
        /// Gets or sets the session token.
        /// </summary>
        /// <value>The session token.</value>
        public string SessionToken { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the login time of the user.
        /// </summary>
        public DateTime LoginTime { get; set; }
        /// <summary>
        /// Gets or sets the logout time of the user.
        /// </summary>
        public DateTime? LogoutTime { get; set; }
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the email address value.
        /// </summary>
        public string Email { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the optional information.
        /// </summary>
        public string? OptionalInfo { get; set; }
        /// <summary>
        /// Gets or sets the array of roles.
        /// </summary>
        public Role[] Roles { get; set; } = [];
    }
}
