//@BaseCode
#if ACCOUNT_ON
namespace SETemplate.Logic.Modules.Security
{
    /// <summary>
    /// Represents an attribute used to authorize access to a class or method.
    /// </summary>
    /// <remarks>
    /// This attribute can be applied to classes or methods. It indicates that access to the class or method
    /// is restricted and requires authorization.
    /// </remarks>
    /// <seealso cref="System.Attribute" />
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    internal partial class AuthorizeAttribute : Attribute
    {
        /// <summary>
        /// Gets a value indicating whether this property is required.
        /// </summary>
        /// <value><c>true</c> if this property is required; otherwise, <c>false</c>.</value>
        public bool Required { get; set; }
        /// <summary>
        /// Gets the roles associated with the user.
        /// </summary>
        /// <value>
        /// The roles associated with the user.
        /// </value>
        public IEnumerable<string> Roles { get; }
        /// <summary>
        /// Initializes a new instance of the <see cref="AuthorizeAttribute"/> class.
        /// </summary>
        public AuthorizeAttribute()
        {
            Required = true;
            Roles = [];
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="AuthorizeAttribute"/> class with specified roles.
        /// </summary>
        /// <param name="roles">The roles to assign.</param>
        public AuthorizeAttribute(params string[] roles)
        {
            Required = true;
            Roles = roles ?? [];
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="AuthorizeAttribute"/> class.
        /// </summary>
        /// <param name="required">Determines if authorization is required.</param>
        /// <param name="roles">The list of roles allowed for authorization.</param>
        protected AuthorizeAttribute(bool required, params string[] roles)
        {
            Required = required;
            Roles = roles ?? [];
        }

        /// <summary>
        /// Creates a clone of the current <see cref="AuthorizeAttribute"/> instance.
        /// </summary>
        /// <param name="addRolse">Additional roles to include in the cloned attribute. May be <c>null</c> or empty.</param>
        /// <returns>
        /// A new <see cref="AuthorizeAttribute"/> with the same <see cref="Required"/> flag and a combined set
        /// of roles consisting of the original <see cref="Roles"/> and the <paramref name="addRolse"/> values.
        /// </returns>
        /// <remarks>
        /// The resulting roles collection is produced by performing a union of the existing roles and the additional roles,
        /// which removes duplicates. The parameter name <c>addRolse</c> is preserved to match the existing method signature.
        /// </remarks>
        public AuthorizeAttribute Clone(params string[] addRolse)
        {
            return new AuthorizeAttribute(Required, Roles.Union(addRolse).ToArray());
        }
    }
}
#endif

