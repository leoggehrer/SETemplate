//@BaseCode
namespace SETemplate.Common.Contracts
{
    /// <summary>
    /// Represents an identifiable in the company manager.
    /// </summary>
    public partial interface IIdentifiable
    {
        #region Properties
        /// <summary>
        /// Gets the unique identifier for the entity.
        /// </summary>
        IdType Id { get; protected set; }
        #endregion Properties
    }
}
