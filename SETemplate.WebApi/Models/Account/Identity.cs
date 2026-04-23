//@BaseCode
#if ACCOUNT_ON
namespace SETemplate.WebApi.Models.Account
{
    /// <summary>
    /// Represents an identity model.
    /// </summary>
    public partial class Identity : ModelObject
    {
        /// <summary>
        /// Represents the static <see cref="Identity"/> class.
        /// </summary>
        static Identity()
        {
            ClassConstructing();
            ClassConstructed();
        }
        /// <summary>
        /// Represents a partial method called when the class is being constructed.
        /// </summary>
        static partial void ClassConstructing();
        /// <summary>
        /// This method is called when a class is constructed.
        /// </summary>
        static partial void ClassConstructed();
        /// <summary>
        /// Initializes a new instance of the <see cref="Identity"/> class.
        /// </summary>
        public Identity()
        {
            Constructing();
            Constructed();
        }
        /// <summary>
        /// This method is called during the construction of an object.
        /// </summary>
        partial void Constructing();
        /// <summary>
        /// This method is a partial method that can be implemented in a partial class.
        /// It is called when an object is constructed.
        /// </summary>
        partial void Constructed();
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; internal set; } = string.Empty;
        /// <summary>
        /// Gets or sets the email associated with the source object.
        /// </summary>
        /// <value>A string value representing an email address.</value>
        public string Email { get; internal set; } = string.Empty;
        ///<summary>
        /// Gets or sets the timeout value in minutes.
        ///</summary>
        public int TimeOutInMinutes { get; internal set; }
        /// <summary>
        /// Gets or sets the number of times the access to the source has failed.
        /// </summary>
        public int AccessFailedCount { get; internal set; }
        /// <summary>
        /// Gets or sets the state of the module.
        /// </summary>
        public CommonEnums.State State { get; internal set; }
    }
}
#endif
