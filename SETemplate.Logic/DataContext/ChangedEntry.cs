//@BaseCode
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace SETemplate.Logic.DataContext
{
    /// <summary>
    /// Represents a changed entity entry wrapper that provides information about entity state changes.
    /// </summary>
    internal partial class ChangedEntry(EntityEntry entry)
    {
        /// <summary>
        /// Gets the Entity Framework entity entry that contains the tracked entity and its metadata.
        /// </summary>
        public EntityEntry Entry { get; } = entry;

        /// <summary>
        /// Gets the current state of the entity (Added, Modified, Deleted, etc.).
        /// </summary>
        public EntityState State { get; } = entry.State;
    }
}
