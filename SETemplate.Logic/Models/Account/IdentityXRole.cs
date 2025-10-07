//@BaseCode
#if ACCOUNT_ON
namespace SETemplate.Logic.Models.Account
{
    /// <summary>
    /// Represents an identity to role model.
    /// </summary>
    internal partial class IdentityXRole : ModelObject
    {
        /// <summary>
        /// Documentation for the static constructor of the IdentityXRole class.
        /// </summary>
        static IdentityXRole()
        {
            ClassConstructing();
            ClassConstructed();
        }
        /// <summary>
        /// A partial method called when the class is being constructed.
        /// </summary>
        static partial void ClassConstructing();
        /// <summary>
        /// This method is called when a class is constructed.
        /// </summary>
        static partial void ClassConstructed();
        /// <summary>
        /// Represents a new instance of the IdentityXRole class.
        public IdentityXRole()
        {
            Constructing();
            Constructed();
        }
        /// <summary>
        /// This method is called during the construction of the object.
        /// </summary>
        partial void Constructing();
        /// <summary>
        /// This method is called after the object has been initialized.
        /// </summary>
        partial void Constructed();
        /// <summary>
        /// Gets or sets the ID type of the identity.
        /// </summary>

        public IdType IdentityId { get; internal set; }
        /// <summary>
        /// Gets or sets the unique identifier of the role.
        /// </summary>
        public IdType RoleId { get; internal set; }
    }
}
#endif
