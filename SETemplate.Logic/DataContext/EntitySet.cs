//@BaseCode
using Microsoft.EntityFrameworkCore.ChangeTracking;
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
            BeforeCreateAccessing(MethodBase.GetCurrentMethod()!);

            return ExecuteCreate();
        }

        /// <summary>
        /// Returns the count of entities in the set asynchronously.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains the count of entities.</returns>
        public virtual Task<int> CountAsync()
        {
            BeforeReadAccessing(MethodBase.GetCurrentMethod()!.GetAsyncOriginal());

            return ExecuteCountAsync();
        }

        /// <summary>
        /// Returns an <see cref="IQueryable{TEntity}"/> that can be used to query the set of entities without tracking changes.
        /// </summary>
        /// <returns>An <see cref="IQueryable{TEntity}"/> that can be used to query the set of entities without tracking changes.</returns>
        public virtual IQueryable<TEntity> AsNoTrackingSet()
        {
            BeforeReadAccessing(MethodBase.GetCurrentMethod()!);

            return ExecuteAsNoTrackingSet();
        }

        /// <summary>
        /// Returns the element of type T with the identification of id.
        /// </summary>
        /// <param name="id">The identification.</param>
        /// <returns>The element of the type T with the corresponding identification.</returns>
        public virtual ValueTask<TEntity?> GetByIdAsync(IdType id)
        {
            BeforeReadAccessing(MethodBase.GetCurrentMethod()!.GetAsyncOriginal());

            return ExecuteGetByIdAsync(id);
        }

        /// <summary>
        /// Asynchronously adds the specified entity to the set.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the added entity.</returns>
        public virtual Task<TEntity> AddAsync(TEntity entity)
        {
            BeforeCreateAccessing(MethodBase.GetCurrentMethod()!.GetAsyncOriginal());

            return ExecuteAddAsync(entity);
        }

        /// <summary>
        /// Asynchronously adds a range of entities to the set.
        /// </summary>
        /// <param name="entities">The collection of entities to add.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the added entities.</returns>
        public virtual Task<IEnumerable<TEntity>> AddRangeAsync(IEnumerable<TEntity> entities)
        {
            BeforeCreateAccessing(MethodBase.GetCurrentMethod()!.GetAsyncOriginal());

            return ExecuteAddRangeAsync(entities);
        }

        /// <summary>
        /// Asynchronously updates the specified entity in the set.
        /// </summary>
        /// <param name="id">The identifier of the entity to update.</param>
        /// <param name="entity">The entity with updated values.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the updated entity, or null if the entity was not found.</returns>
        public virtual Task<TEntity?> UpdateAsync(IdType id, TEntity entity)
        {
            BeforeUpdateAccessing(MethodBase.GetCurrentMethod()!.GetAsyncOriginal());

            return ExecuteUpdateAsync(id, entity);
        }

        /// <summary>
        /// Removes the entity with the specified identifier from the set.
        /// </summary>
        /// <param name="id">The identifier of the entity to remove.</param>
        /// <returns>The removed entity, or null if the entity was not found.</returns>
        public virtual Task<TEntity?> RemoveAsync(IdType id)
        {
            BeforeDeleteAccessing(MethodBase.GetCurrentMethod()!.GetAsyncOriginal());

            return ExecuteRemoveAsync(id);
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

        #region internal methods
        /// <summary>
        /// Method that is called before saving changes to the database.
        /// This method allows custom logic to be executed prior to persisting entity changes.
        /// Override this method in derived classes to implement entity-specific pre-save operations.
        /// </summary>
        /// <param name="changedEntries">A read-only list of entity entries that are about to be saved to the database.</param>
        internal virtual void BeforeSaveChanges(IReadOnlyList<ChangedEntry> changedEntries)
        {
        }
        /// <summary>
        /// Method that is called after saving changes to the database.
        /// This method allows custom logic to be executed after persisting entity changes.
        /// Override this method in derived classes to implement entity-specific post-save operations.
        /// </summary>
        /// <param name="entries">A read-only list of entity entries that have been saved to the database.</param>
        internal virtual void AfterSaveChanges(IReadOnlyList<ChangedEntry> changedEntries)
        {
        }
        #endregion internal methods

        #region partial methods
        /// <summary>
        /// Method that is called before accessing any read operation in the EntitySet class.
        /// </summary>
        /// <param name="methodBase">The method that is being accessed.</param>
        partial void BeforeReadAccessing(MethodBase methodBase);
        /// <summary>
        /// Method that is called before accessing any create operation in the EntitySet class.
        /// </summary>
        /// <param name="methodBase">The method that is being accessed.</param>
        partial void BeforeCreateAccessing(MethodBase methodBase);
        /// <summary>
        /// Method that is called before accessing any update operation in the EntitySet class.
        /// </summary>
        /// <param name="methodBase">The method that is being accessed.</param>
        partial void BeforeUpdateAccessing(MethodBase methodBase);
        /// <summary>
        /// Method that is called before accessing any delete operation in the EntitySet class.
        /// </summary>
        /// <param name="methodBase">The method that is being accessed.</param>
        partial void BeforeDeleteAccessing(MethodBase methodBase);
        #endregion partial methods
    }
}
