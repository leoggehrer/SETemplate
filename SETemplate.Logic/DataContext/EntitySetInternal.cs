//@BaseCode
namespace SETemplate.Logic.DataContext
{
    partial class EntitySet<TEntity>
    {
        #region methods
        /// <summary>
        /// Creates a new instance of the entity.
        /// </summary>
        /// <returns>A new instance of the entity.</returns>
        internal virtual TEntity ExecuteCreate()
        {
            return new TEntity();
        }

        /// <summary>
        /// Returns the count of entities in the set.
        /// </summary>
        /// <returns>The count of entities.</returns>
        internal virtual int ExecuteCount()
        {
            return DbSet.Count();
        }

        /// <summary>
        /// Returns the count of entities in the set asynchronously.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains the count of entities.</returns>
        internal virtual Task<int> ExecuteCountAsync()
        {
            return DbSet.CountAsync();
        }

        /// <summary>
        /// Gets the queryable set of entities.
        /// </summary>
        /// <returns>An <see cref="IQueryable{TEntity}"/> that can be used to query the set of entities.</returns>
        internal virtual IQueryable<TEntity> ExecuteAsQuerySet() => DbSet.AsQueryable();

        /// <summary>
        /// Gets the no tracking queryable set of entities.
        /// </summary>
        /// <returns>An <see cref="IQueryable{TEntity}"/> that can be used to query the set of entities.</returns>
        internal virtual IQueryable<TEntity> ExecuteAsNoTrackingSet() => ExecuteAsQuerySet().AsNoTracking();

        /// <summary>
        /// Returns the element of type T with the identification of id.
        /// </summary>
        /// <param name="id">The identification.</param>
        /// <returns>The element of the type T with the corresponding identification.</returns>
        internal virtual ValueTask<TEntity?> ExecuteGetByIdAsync(IdType id)
        {
            return DbSet.FindAsync(id);
        }

        /// <summary>
        /// Adds the specified entity to the set.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        /// <returns>The added entity.</returns>
        internal virtual TEntity ExecuteAdd(TEntity entity)
        {
            BeforeAdding(entity);
            return DbSet.Add(entity).Entity;
        }

        /// <summary>
        /// Adds the specified range of entities to the set.
        /// </summary>
        /// <param name="entities">The entities to add.</param>
        internal virtual void ExecuteAddRange(IEnumerable<TEntity> entities)
        {
            DbSet.AddRange(entities);
        }

        /// <summary>
        /// Asynchronously adds the specified entity to the set.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the added entity.</returns>
        internal virtual async Task<TEntity> ExecuteAddAsync(TEntity entity)
        {
            BeforeAdding(entity);
            var result = await DbSet.AddAsync(entity).ConfigureAwait(false);

            return result.Entity;
        }

        /// <summary>
        /// Asynchronously adds the specified range of entities to the set.
        /// </summary>
        /// <param name="entities">The entities to add.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        internal virtual async Task ExecuteAddRangeAsync(IEnumerable<TEntity> entities)
        {
            await DbSet.AddRangeAsync(entities).ConfigureAwait(false);
        }

        /// <summary>
        /// Updates the specified entity in the set.
        /// </summary>
        /// <param name="id">The identifier of the entity to update.</param>
        /// <param name="entity">The entity with updated values.</param>
        /// <returns>The updated entity, or null if the entity was not found.</returns>
        internal virtual TEntity? ExecuteUpdate(IdType id, TEntity entity)
        {
            BeforeUpdating(entity);

            var existingEntity = DbSet.Find(id);
            if (existingEntity != null)
            {
                CopyProperties(existingEntity, entity);
            }
            return existingEntity;
        }

        /// <summary>
        /// Asynchronously updates the specified entity in the set.
        /// </summary>
        /// <param name="id">The identifier of the entity to update.</param>
        /// <param name="entity">The entity with updated values.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the updated entity, or null if the entity was not found.</returns>
        internal virtual async Task<TEntity?> ExecuteUpdateAsync(IdType id, TEntity entity)
        {
            BeforeUpdating(entity);

            var existingEntity = await DbSet.FindAsync(id).ConfigureAwait(false);
            if (existingEntity != null)
            {
                CopyProperties(existingEntity, entity);
            }
            return existingEntity;
        }

        /// <summary>
        /// Removes the entity with the specified identifier from the set.
        /// </summary>
        /// <param name="id">The identifier of the entity to remove.</param>
        /// <returns>The removed entity, or null if the entity was not found.</returns>
        internal virtual TEntity? ExecuteRemove(IdType id)
        {
            var entity = DbSet.Find(id);

            if (entity != null)
            {
                BeforeRemoving(entity);
                DbSet.Remove(entity);
            }
            return entity;
        }
        #endregion methods
    }
}
