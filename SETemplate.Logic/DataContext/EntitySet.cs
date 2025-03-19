//@BaseCode
using Microsoft.EntityFrameworkCore;

namespace SETemplate.Logic.DataContext
{
    /// <summary>
    /// Represents a set of entities that can be queried from a database and provides methods to manipulate them.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    public abstract class EntitySet<TEntity> where TEntity : Entities.EntityObject, new()
    {
        #region fields
        protected DbSet<TEntity> _dbSet;
        #endregion fields

        #region properties
        /// <summary>
        /// Gets the database context.
        /// </summary>
        internal DbContext Context { get; private set; }

        /// <summary>
        /// Gets the queryable set of entities.
        /// </summary>
        public IQueryable<TEntity> QuerySet => _dbSet.AsQueryable();
        #endregion properties

        /// <summary>
        /// Initializes a new instance of the <see cref="EntitySet{TEntity}"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="dbSet">The set of entities.</param>
        public EntitySet(DbContext context, DbSet<TEntity> dbSet)
        {
            Context = context;
            _dbSet = dbSet;
        }

        #region methods
        protected abstract void CopyProperties(TEntity target, TEntity source);

        /// <summary>
        /// Creates a new instance of the entity.
        /// </summary>
        /// <returns>A new instance of the entity.</returns>
        public virtual TEntity Create()
        {
            return new TEntity();
        }

        /// <summary>
        /// Adds the specified entity to the set.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        /// <returns>The added entity.</returns>
        public virtual TEntity Add(TEntity entity)
        {
            return _dbSet.Add(entity).Entity;
        }

        /// <summary>
        /// Asynchronously adds the specified entity to the set.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the added entity.</returns>
        public virtual async Task<TEntity> AddAsync(TEntity entity)
        {
            var result = await _dbSet.AddAsync(entity).ConfigureAwait(false);

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
            var existingEntity = _dbSet.Find(id);
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
            var existingEntity = await _dbSet.FindAsync(id).ConfigureAwait(false);
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
            var entity = _dbSet.Find(id);
            if (entity != null)
            {
                _dbSet.Remove(entity);
            }
            return entity;
        }
        #endregion methods
    }
}
