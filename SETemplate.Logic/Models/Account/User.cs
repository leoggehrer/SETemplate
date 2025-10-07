//@BaseCode
#if ACCOUNT_ON
namespace SETemplate.Logic.Models.Account
{
    /// <summary>
    /// Represents an user model.
    /// </summary>
    public partial class User : ModelObject
    {
        /// <summary>
        /// Initializes the <see cref="User"/> class.
        /// </summary>
        static User()
        {
            ClassConstructing();
            ClassConstructed();
        }
        /// <summary>
        /// This method is called when the class is being constructed.
        /// </summary>
        static partial void ClassConstructing();
        /// <summary>
        /// This method is called when the class is constructed.
        /// </summary>
        static partial void ClassConstructed();
        /// <summary>
        /// Initializes a new instance of the <see cref="User"/> class.
        /// </summary>
        public User()
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
        /// Gets or sets the identity identifier.
        /// </summary>
        public IdType IdentityId { get; internal set; }
        /// <summary>
        /// Gets or sets the first name of the source object.
        /// </summary>
        public string FirstName { get; internal set; } = string.Empty;
        /// <summary>
        /// Gets or sets the last name.
        /// </summary>
        /// <value>
        /// The last name.
        /// </value>
        public string LastName { get; internal set; } = string.Empty;
    }
}
#endif
