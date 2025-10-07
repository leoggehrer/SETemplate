//@BaseCode
namespace SETemplate.MVVMApp.Models.Account
{
    /// <summary>
    /// Represents a role model.
    /// </summary>
    public partial class Role : ModelObject
    {
        /// <summary>
        /// Initializes the static members of the <see cref="Role"/> class.
        /// </summary>
        static Role()
        {
            ClassConstructing();
            ClassConstructed();
        }
        /// <summary>
        /// Called when the class is being constructed.
        /// </summary>
        static partial void ClassConstructing();
        /// <summary>
        /// This method is called when the class is constructed.
        /// </summary>
        static partial void ClassConstructed();
        /// <summary>
        /// Initializes a new instance of the Role class.
        /// </summary>
        public Role()
        {
            Constructing();
            Constructed();
        }
        /// <summary>
        /// This method is called during the construction of the object.
        /// It can be implemented by subclasses to initialize any required data.
        /// </summary>
        partial void Constructing();
        ///<summary>
        /// This method is called after the object has been initialized.
        /// It can be overridden in a partial class.
        ///</summary>
        partial void Constructed();

        /// <summary>
        /// Gets or sets the Designation property.
        /// </summary>
        public string Designation { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        public string? Description { get; set; } = string.Empty;
    }
}
