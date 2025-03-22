//@BaseCode
namespace SETemplate.WebApi.Models
{
    /// <summary>
    /// Represents an abstract base class for model objects that are identifiable.
    /// </summary>
    public abstract partial class ModelObject : Common.Contracts.IIdentifiable
    {
        /// <summary>
        /// Gets or sets the unique identifier for the model object.
        /// </summary>
        public IdType Id { get; set; }
    }
}
