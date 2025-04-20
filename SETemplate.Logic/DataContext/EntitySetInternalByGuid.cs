//@BaseCode
#if EXTERNALGUID_ON
namespace SETemplate.Logic.DataContext
{
    partial class EntitySet<TEntity>
    {
        /// <summary>
        /// Returns the entity with the specified identifier.
        /// </summary>
        /// <param name="guid">The identifier of the entity.</param>
        /// <returns>The entity with the specified identifier, or null if not found.</returns>
        internal virtual Task<TEntity?> ExecuteGetByGuidAsync(Guid guid)
        {
            return DbSet.FirstOrDefaultAsync(e => e.Guid == guid);
        }

        /// <summary>
        /// Updates the specified entity in the set by its identifier.
        /// </summary>
        /// <param name="guid">The identifier of the entity to update.</param>
        /// <param name="entity">The entity with updated values.</param>
        /// <returns>The updated entity, or null if the entity was not found.</returns>
        internal virtual TEntity? ExecuteUpdateByGuid(Guid guid, TEntity entity)
        {
            BeforeExecuteUpdating(entity);

            var existingEntity = DbSet.FirstOrDefault(e => e.Guid == guid);
            if (existingEntity != null)
            {
                CopyProperties(existingEntity, entity);
            }
            return existingEntity;
        }

        /// <summary>
        /// Asynchronously updates the specified entity in the set by its identifier.
        /// </summary>
        /// <param name="guid">The identifier of the entity to update.</param>
        /// <param name="entity">The entity with updated values.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the updated entity, or null if the entity was not found.</returns>
        internal virtual async Task<TEntity?> ExecuteUpdateByGuidAsync(Guid guid, TEntity entity)
        {
            BeforeExecuteUpdating(entity);

            var existingEntity = await DbSet.FirstOrDefaultAsync(e => e.Guid == guid).ConfigureAwait(false);
            if (existingEntity != null)
            {
                CopyProperties(existingEntity, entity);
            }
            return existingEntity;
        }

        /// <summary>
        /// Removes the entity with the specified identifier from the set.
        /// </summary>
        /// <param name="guid">The identifier of the entity to remove.</param>
        /// <returns>The removed entity, or null if the entity was not found.</returns>
        internal virtual TEntity? ExecuteRemoveByGuid(Guid guid)
        {
            var entity = DbSet.FirstOrDefault(e => e.Guid == guid);

            if (entity != null)
            {
                BeforeExecuteRemoving(entity);
                DbSet.Remove(entity);
            }
            return entity;
        }

        /// <summary>
        /// Assigns a new unique identifier (GUID) to the specified entity before it is added to the set.
        /// </summary>
        /// <param name="entity">The entity to which a new GUID will be assigned.</param>
        partial void BeforeAdding(TEntity entity)
        {
            entity.Guid = Guid.NewGuid();
        }
    }
}
#endif
