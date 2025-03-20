﻿//@BaseCode
namespace SETemplate.Logic.Entities
{
    /// <summary>
    /// Represents an abstract base class for entities with an identifier.
    /// </summary>
    public abstract partial class EntityObject : Common.Contracts.IIdentifiable
    {
        /// <summary>
        /// Gets or sets the identifier of the entity.
        /// </summary>
        [Key]
        public int Id { get; set; }
    }
}
