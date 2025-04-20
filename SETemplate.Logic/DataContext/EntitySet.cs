//@BaseCode
using SETemplate.Logic.Contracts;
using System.Reflection;

namespace SETemplate.Logic.DataContext
{
    /// <summary>
    /// Represents a set of entities that can be queried from a database and provides methods to manipulate them.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <param name="context">The database context.</param>
    /// <param name="dbSet">The set of entities.</param>
    internal abstract partial class EntitySet<TEntity>(ProjectDbContext context, DbSet<TEntity> dbSet) : IEntitySet<TEntity>, IDisposable
        where TEntity : Entities.EntityObject, new()
    {
        #region fields
        private ProjectDbContext? _context = context;
        private DbSet<TEntity>? _dbSet = dbSet;
        #endregion fields

        #region properties
        /// <summary>
        /// Gets the database context.
        /// </summary>
        internal ProjectDbContext Context => _context!;
        /// <summary>
        /// Gets the database context.
        /// </summary>
        protected DbSet<TEntity> DbSet => _dbSet!;
        #endregion properties

        #region methods
        /// <summary>
        /// Creates a new instance of the entity.
        /// </summary>
        /// <returns>A new instance of the entity.</returns>
        public virtual TEntity Create()
        {
            BeforeAccessing(MethodBase.GetCurrentMethod()!);

            return ExecuteCreate();
        }

        /// <summary>
        /// Returns the count of entities in the set.
        /// </summary>
        /// <returns>The count of entities.</returns>
        public virtual int Count()
        {
            BeforeAccessing(MethodBase.GetCurrentMethod()!);

            return ExecuteCount();
        }

        /// <summary>
        /// Returns the count of entities in the set asynchronously.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains the count of entities.</returns>
        public virtual Task<int> CountAsync()
        {
            BeforeAccessing(MethodBase.GetCurrentMethod()!.GetAsyncOriginal());

            return ExecuteCountAsync();
        }

        /// <summary>
        /// Gets the queryable set of entities.
        /// </summary>
        /// <returns>An <see cref="IQueryable{TEntity}"/> that can be used to query the set of entities.</returns>
        public virtual IQueryable<TEntity> AsQuerySet()
        {
            BeforeAccessing(MethodBase.GetCurrentMethod()!);

            return ExecuteAsQuerySet();
        }

        /// <summary>
        /// Returns an <see cref="IQueryable{TEntity}"/> that can be used to query the set of entities without tracking changes.
        /// </summary>
        /// <returns>An <see cref="IQueryable{TEntity}"/> that can be used to query the set of entities without tracking changes.</returns>
        public virtual IQueryable<TEntity> AsNoTrackingSet()
        {
            BeforeAccessing(MethodBase.GetCurrentMethod()!);

            return ExecuteAsNoTrackingSet();
        }

        /// <summary>
        /// Returns the element of type T with the identification of id.
        /// </summary>
        /// <param name="id">The identification.</param>
        /// <returns>The element of the type T with the corresponding identification.</returns>
        public virtual ValueTask<TEntity?> GetByIdAsync(IdType id)
        {
            BeforeAccessing(MethodBase.GetCurrentMethod()!.GetAsyncOriginal());

            return ExecuteGetByIdAsync(id);
        }

        /// <summary>
        /// Adds the specified entity to the set.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        /// <returns>The added entity.</returns>
        public virtual TEntity Add(TEntity entity)
        {
            BeforeAccessing(MethodBase.GetCurrentMethod()!);

            return ExecuteAdd(entity);
        }

        /// <summary>
        /// Adds a range of entities to the set.
        /// </summary>
        /// <param name="entities">The entities to add.</param>
        public virtual void AddRange(IEnumerable<TEntity> entities)
        {
            BeforeAccessing(MethodBase.GetCurrentMethod()!);

            ExecuteAddRange(entities);
        }

        /// <summary>
        /// Asynchronously adds the specified entity to the set.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the added entity.</returns>
        public virtual Task<TEntity> AddAsync(TEntity entity)
        {
            BeforeAccessing(MethodBase.GetCurrentMethod()!.GetAsyncOriginal());

            return ExecuteAddAsync(entity);
        }

        /// <summary>
        /// Asynchronously adds a range of entities to the set.
        /// </summary>
        /// <param name="entities">The entities to add.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public virtual Task AddRangeAsync(IEnumerable<TEntity> entities)
        {
            BeforeAccessing(MethodBase.GetCurrentMethod()!.GetAsyncOriginal());

            return ExecuteAddRangeAsync(entities);
        }

        /// <summary>
        /// Updates the specified entity in the set.
        /// </summary>
        /// <param name="id">The identifier of the entity to update.</param>
        /// <param name="entity">The entity with updated values.</param>
        /// <returns>The updated entity, or null if the entity was not found.</returns>
        public virtual TEntity? Update(IdType id, TEntity entity)
        {
            BeforeAccessing(MethodBase.GetCurrentMethod()!);

            return ExecuteUpdate(id, entity);
        }

        /// <summary>
        /// Asynchronously updates the specified entity in the set.
        /// </summary>
        /// <param name="id">The identifier of the entity to update.</param>
        /// <param name="entity">The entity with updated values.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the updated entity, or null if the entity was not found.</returns>
        public virtual Task<TEntity?> UpdateAsync(IdType id, TEntity entity)
        {
            BeforeAccessing(MethodBase.GetCurrentMethod()!.GetAsyncOriginal());

            return ExecuteUpdateAsync(id, entity);
        }

        /// <summary>
        /// Removes the specified entity from the set.
        /// </summary>
        /// <param name="entity">The entity to remove.</param>
        /// <returns>The removed entity, or null if the entity was not found.</returns>
        public virtual TEntity? Remove(TEntity entity)
        {
            BeforeAccessing(MethodBase.GetCurrentMethod()!);

            return Remove(entity.Id);
        }

        /// <summary>
        /// Removes the entity with the specified identifier from the set.
        /// </summary>
        /// <param name="id">The identifier of the entity to remove.</param>
        /// <returns>The removed entity, or null if the entity was not found.</returns>
        public virtual TEntity? Remove(IdType id)
        {
            BeforeAccessing(MethodBase.GetCurrentMethod()!);

            return ExecuteRemove(id);
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

        #region partial methods
        /// <summary>
        /// Method that is called before accessing any method in the EntitySet class.
        /// </summary>
        /// <param name="methodBase">The method that is being accessed.</param>
        partial void BeforeAccessing(MethodBase methodBase);
        #endregion partial methods
    }
}
