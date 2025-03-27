//@BaseCode
using SETemplate.Logic.Contracts;

namespace SETemplate.Logic.DataContext
{
    /// <summary>
    /// Represents a set of entities that can be queried from a database and provides methods to manipulate them.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <param name="context">The database context.</param>
    /// <param name="dbSet">The set of entities.</param>
    public abstract class EntitySet<TEntity>(DbContext context, DbSet<TEntity> dbSet) : IEntitySet<TEntity>, IDisposable
        where TEntity : Entities.EntityObject, new()
    {
        #region fields
        private DbContext? _context = context;
        private DbSet<TEntity>? _dbSet = dbSet;
        #endregion fields

        #region properties
        /// <summary>
        /// Gets the database context.
        /// </summary>
        internal DbContext Context => _context!;
        /// <summary>
        /// Gets the database context.
        /// </summary>
        protected DbSet<TEntity> DbSet => _dbSet!;
        #endregion properties

        #region overridables
        /// <summary>
        /// Copies properties from the source entity to the target entity.
        /// </summary>
        /// <param name="target">The target entity.</param>
        /// <param name="source">The source entity.</param>
        protected abstract void CopyProperties(TEntity target, TEntity source);

        /// <summary>
        /// Performs actions before adding an entity.
        /// </summary>
        /// <param name="entity">The entity to be added.</param>
        protected virtual void BeforeAdding(TEntity entity) { }

        /// <summary>
        /// Performs actions before updating an entity.
        /// </summary>
        /// <param name="entity">The entity to be updated.</param>
        protected virtual void BeforeUpdating(TEntity entity) { }

        /// <summary>
        /// Performs actions before removing an entity.
        /// </summary>
        /// <param name="entity">The entity to be removed.</param>
        protected virtual void BeforeRemoving(TEntity entity) { }
        #endregion ovveridables

        #region methods
        /// <summary>
        /// Creates a new instance of the entity.
        /// </summary>
        /// <returns>A new instance of the entity.</returns>
        public virtual TEntity Create()
        {
            return new TEntity();
        }

        /// <summary>
        /// Gets the queryable set of entities.
        /// </summary>
        /// <returns>An <see cref="IQueryable{TEntity}"/> that can be used to query the set of entities.</returns>
        public IQueryable<TEntity> AsQuerySet() => DbSet.AsQueryable();

        /// <summary>
        /// Adds the specified entity to the set.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        /// <returns>The added entity.</returns>
        public virtual TEntity Add(TEntity entity)
        {
            BeforeAdding(entity);
            return DbSet.Add(entity).Entity;
        }

        /// <summary>
        /// Asynchronously adds the specified entity to the set.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the added entity.</returns>
        public virtual async Task<TEntity> AddAsync(TEntity entity)
        {
            BeforeAdding(entity);
            var result = await DbSet.AddAsync(entity).ConfigureAwait(false);

            return result.Entity;
        }

        /// <summary>
        /// Updates the specified entity in the set.
        /// </summary>
        /// <param name="id">The identifier of the entity to update.</param>
        /// <param name="entity">The entity with updated values.</param>
        /// <returns>The updated entity, or null if the entity was not found.</returns>
        public virtual TEntity? Update(int id, TEntity entity)
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
        public virtual async Task<TEntity?> UpdateAsync(int id, TEntity entity)
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
        public virtual TEntity? Remove(int id)
        {
            var entity = DbSet.Find(id);

            if (entity != null)
            {
                BeforeRemoving(entity);
                DbSet.Remove(entity);
            }
            return entity;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            _dbSet = null;
            _context = null;
            GC.SuppressFinalize(this);
        }
        #endregion methods
    }
}
