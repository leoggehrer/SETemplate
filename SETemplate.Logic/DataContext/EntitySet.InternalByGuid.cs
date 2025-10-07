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
        /// Asynchronously updates the specified entity in the set by its identifier.
        /// </summary>
        /// <param name="guid">The identifier of the entity to update.</param>
        /// <param name="entity">The entity with updated values.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the updated entity, or null if the entity was not found.</returns>
        internal virtual async Task<TEntity?> ExecuteUpdateByGuidAsync(Guid guid, TEntity entity)
        {
            PrepareUpdate(entity);

            var existingEntity = await DbSet.FirstOrDefaultAsync(e => e.Guid == guid).ConfigureAwait(false);

            if (existingEntity != null)
            {
                await BeforePersistingUpdateAsync(existingEntity).ConfigureAwait(false);
                CopyProperties(existingEntity, entity);
                await OnUpdatedAsync(existingEntity).ConfigureAwait(false);
            }
            return existingEntity;
        }

        internal virtual async Task<TEntity?> ExecuteRemoveByGuidAsync(Guid guid)
        {
            var existingEntity = await DbSet.FirstOrDefaultAsync(e => e.Guid == guid).ConfigureAwait(false);

            if (existingEntity != null)
            {
                PrepareRemove(existingEntity);
                await BeforePersistingRemoveAsync(existingEntity).ConfigureAwait(false);
                DbSet.Remove(existingEntity);
                await OnRemovedAsync(existingEntity).ConfigureAwait(false);
            }
            return existingEntity;
        }
    }
}
#endif
