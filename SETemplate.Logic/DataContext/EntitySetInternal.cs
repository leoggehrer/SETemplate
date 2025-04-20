//@BaseCode
namespace SETemplate.Logic.DataContext
{
    partial class EntitySet<TEntity>
    {
        #region overridables
        /// <summary>
        /// Copies properties from the source entity to the target entity.
        /// </summary>
        /// <param name="target">The target entity.</param>
        /// <param name="source">The source entity.</param>
        protected abstract void CopyProperties(TEntity target, TEntity source);

        /// <summary>
        /// Performs actions before creating an entity.
        /// </summary>
        /// <returns>A new instance of the entity or a custom instance if overridden.</returns>
        protected virtual TEntity? BeforeExecuteCreating() { return default; }

        /// <summary>
        /// Performs actions after an entity is created.
        /// </summary>
        /// <param name="entity">The newly created entity.</param>
        protected virtual void AfterExecuteCreated(TEntity entity) { }

        /// <summary>
        /// Performs actions before adding an entity to the set.
        /// </summary>
        /// <param name="entity">The entity to be added.</param>
        protected virtual void BeforeExecuteAdding(TEntity entity) { }

        /// <summary>
        /// Performs actions before updating an entity in the set.
        /// </summary>
        /// <param name="entity">The entity to be updated.</param>
        protected virtual void BeforeExecuteUpdating(TEntity entity) { }

        /// <summary>
        /// Performs actions before removing an entity from the set.
        /// </summary>
        /// <param name="entity">The entity to be removed.</param>
        protected virtual void BeforeExecuteRemoving(TEntity entity) { }
        #endregion overridables

        #region methods
        /// <summary>
        /// Creates a new instance of the entity.
        /// </summary>
        /// <returns>A new instance of the entity.</returns>
        internal virtual TEntity ExecuteCreate()
        {
            var result = BeforeExecuteCreating();

            if (result == default)
            {
                result = new TEntity();
            }
            AfterExecuteCreated(result);
            return result;
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
        /// Gets the no-tracking queryable set of entities.
        /// </summary>
        /// <returns>An <see cref="IQueryable{TEntity}"/> that can be used to query the set of entities without tracking changes.</returns>
        internal virtual IQueryable<TEntity> ExecuteAsNoTrackingSet() => ExecuteAsQuerySet().AsNoTracking();

        /// <summary>
        /// Returns the entity with the specified identifier.
        /// </summary>
        /// <param name="id">The identifier of the entity.</param>
        /// <returns>The entity with the specified identifier, or null if not found.</returns>
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
            BeforeExecuteAdding(entity);
            return DbSet.Add(entity).Entity;
        }

        /// <summary>
        /// Adds the specified range of entities to the set.
        /// </summary>
        /// <param name="entities">The entities to add.</param>
        internal virtual void ExecuteAddRange(IEnumerable<TEntity> entities)
        {
            entities.ForEach(e =>
            {
                BeforeAdding(e);
                BeforeExecuteAdding(e);
            });
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
            BeforeExecuteAdding(entity);
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
            entities.ForEach(e =>
            {
                BeforeAdding(e);
                BeforeExecuteAdding(e);
            });
            await DbSet.AddRangeAsync(entities).ConfigureAwait(false);
        }

        /// <summary>
        /// Updates the specified entity in the set.
        /// </summary>
        /// <param name="entity">The entity with updated values.</param>
        /// <returns>The updated entity, or null if the entity was not found.</returns>
        internal virtual TEntity? ExecuteUpdate(TEntity entity)
        {
            return ExecuteUpdate(entity.Id, entity);
        }

        /// <summary>
        /// Updates the specified entity in the set by its identifier.
        /// </summary>
        /// <param name="id">The identifier of the entity to update.</param>
        /// <param name="entity">The entity with updated values.</param>
        /// <returns>The updated entity, or null if the entity was not found.</returns>
        internal virtual TEntity? ExecuteUpdate(IdType id, TEntity entity)
        {
            BeforeExecuteUpdating(entity);

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
        /// <param name="entity">The entity with updated values.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the updated entity, or null if the entity was not found.</returns>
        internal virtual Task<TEntity?> ExecuteUpdateAsync(TEntity entity)
        {
            return ExecuteUpdateAsync(entity.Id, entity);
        }

        /// <summary>
        /// Asynchronously updates the specified entity in the set by its identifier.
        /// </summary>
        /// <param name="id">The identifier of the entity to update.</param>
        /// <param name="entity">The entity with updated values.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the updated entity, or null if the entity was not found.</returns>
        internal virtual async Task<TEntity?> ExecuteUpdateAsync(IdType id, TEntity entity)
        {
            BeforeExecuteUpdating(entity);

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
                BeforeExecuteRemoving(entity);
                DbSet.Remove(entity);
            }
            return entity;
        }
        #endregion methods

        #region context methods
        /// <summary>
        /// Saves all changes made in the context to the database.
        /// </summary>
        /// <returns>The number of state entries written to the database.</returns>
        internal virtual int ExecuteSaveChanges()
        {
            return Context.ExecuteSaveChanges();
        }

        /// <summary>
        /// Asynchronously saves all changes made in the context to the database.
        /// </summary>
        /// <returns>A task that represents the asynchronous save operation. The task result contains the number of state entries written to the database.</returns>
        internal virtual Task<int> ExecuteSaveChangesAsync()
        {
            return Context.ExecuteSaveChangesAsync();
        }
        #endregion context methods

        #region partial methods
        partial void BeforeAdding(TEntity entity);
        #endregion partial methods
    }
}
