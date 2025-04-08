//@CodeCopy
namespace SETemplate.Common.Models
{
    /// <summary>
    /// Represents an abstract base class for model objects that are identifiable.
    /// </summary>
    public abstract partial class ModelObject : Contracts.IIdentifiable
    {
        /// <summary>
        /// Gets or sets the unique identifier for the model object.
        /// </summary>
        public IdType Id { get; set; }
    }
}
