//@BaseCode
namespace SETemplate.Logic.Entities
{
    /// <summary>
    /// Represents an abstract base class for entities with a row version.
    /// </summary>
    public abstract partial class VersionEntityObject : EntityObject
    {
#if ROWVERSION_ON
        /// <summary>
        /// Gets or sets the row version of the entity.
        /// </summary>
        [Timestamp]
        public byte[]? RowVersion { get; set; } = [];
#endif
    }
}
