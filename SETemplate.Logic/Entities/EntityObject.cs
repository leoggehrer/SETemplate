//@BaseCode
namespace SETemplate.Logic.Entities
{
    /// <summary>
    /// Represents an abstract base class for entities with an identifier.
    /// </summary>
    public abstract partial class EntityObject : DbObject, CommonContracts.IIdentifiable
    {
        /// <summary>
        /// Gets or sets the identifier of the entity.
        /// </summary>
        [Key]
        public IdType Id { get; set; }
#if EXTERNALGUID_ON
        /// <summary>
        /// Gets or sets the external Guid.
        /// </summary>
        public Guid Guid { get; set; }
#endif
    }
}
