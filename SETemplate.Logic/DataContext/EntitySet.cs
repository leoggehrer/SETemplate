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
    internal abstract partial class EntitySet<TEntity> : IEntitySet<TEntity>, IDisposable
        where TEntity : Entities.EntityObject, new()
    {
        #region constructors
        /// <summary>
        /// Initializes the static ApiControllerBase class.
        /// </summary>
        static EntitySet()
        {
            ClassConstructing();
            ClassConstructed();
        }
        /// <summary>
        /// Represents a partial method called before the constructor of the class is executed.
        /// </summary>
        /// <remarks>
        /// This method is automatically generated and can be implemented in partial classes.
        /// </remarks>
        static partial void ClassConstructing();
        /// <summary>
        /// This method is called after the class is constructed.
        /// </summary>
        static partial void ClassConstructed();

        /// <summary>
        /// Initializes a new instance of the ApiControllerBase class.
        /// </summary>
        internal EntitySet(ProjectDbContext context, DbSet<TEntity> dbSet)
        {
            Constructing();
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _dbSet = dbSet ?? throw new ArgumentNullException(nameof(dbSet));
            Constructed();
        }
        /// <summary>
        /// This method is called during the construction of the object.
        /// </summary>
        /// <remarks>
        /// This method can be overridden by a partial class to include additional custom logic.
        /// It is defined as "partial" so that multiple partial classes can provide their own implementation of this method.
        /// </remarks>
        partial void Constructing();
        /// <summary>
        /// This method is called after the object has been initialized.
        /// It represents a partial method without an implementation.
        /// </summary>
        partial void Constructed();
        #endregion constructors

        #region fields
        private ProjectDbContext? _context;
        private DbSet<TEntity>? _dbSet;
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
        /// Returns the entity with the specified identifier without tracking.
        /// </summary>
        /// <param name="id">The identifier of the entity.</param>
        /// <returns>The entity with the specified identifier, or null if not found.</returns>
        public virtual Task<TEntity?> QueryByIdAsync(IdType id)
        {
            BeforeReadAccessing(MethodBase.GetCurrentMethod()!.GetAsyncOriginal());

            return ExecuteQueryByIdAsync(id);
        }

        /// <summary>
        /// Retrieves all entities from the set without tracking changes.
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains a collection of entities limited to <see cref="MaxCount"/>.
        /// </returns>
        /// <remarks>
        /// This method queries entities without change tracking for better performance when read-only access is needed.
        /// The results are automatically limited to the maximum count defined by <see cref="MaxCount"/> to prevent excessive data retrieval.
        /// </remarks>
        public virtual Task<IEnumerable<TEntity>> GetAllAsync()
        {
            BeforeReadAccessing(MethodBase.GetCurrentMethod()!.GetAsyncOriginal());

            return ExecuteGetAllAsync();
        }

        /// <summary>
        /// Asynchronously queries entities from the set based on the provided query parameters.
        /// </summary>
        /// <param name="queryParams">The query parameters containing filter expression, filter values, and navigation properties to include.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a collection of entities matching the query criteria.</returns>
        /// <remarks>
        /// This method applies the filter expression with the provided values and includes the specified navigation properties.
        /// The query is executed against the database without tracking changes.
        /// </remarks>
        public virtual Task<IEnumerable<TEntity>> QueryAsync(Models.QueryParams queryParams)
        {
            BeforeReadAccessing(MethodBase.GetCurrentMethod()!.GetAsyncOriginal());

            return ExecuteQueryAsync(queryParams);
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

        #region access methods
        /// <summary>
        /// Method that is called before accessing any read operation in the EntitySet class.
        /// </summary>
        /// <param name="methodBase">The method that is being accessed.</param>
        /// <param name="roles">The roles required for authorization.</param>
        protected virtual void BeforeReadAccessing(MethodBase methodBase, params string[] roles)
        {
            CheckAccessBeforeReading(methodBase, roles);
        }
        /// <summary>
        /// Method that is called before accessing any create operation in the EntitySet class.
        /// </summary>
        /// <param name="methodBase">The method that is being accessed.</param>
        /// <param name="roles">The roles required for authorization.</param>
        protected virtual void BeforeCreateAccessing(MethodBase methodBase, params string[] roles)
        {
            CheckAccessBeforeCreating(methodBase, roles);
        }
        /// <summary>
        /// Method that is called before accessing any update operation in the EntitySet class.
        /// </summary>
        /// <param name="methodBase">The method that is being accessed.</param>
        /// <param name="roles">The roles required for authorization.</param>
        protected virtual void BeforeUpdateAccessing(MethodBase methodBase, params string[] roles)
        {
            CheckAccessBeforeUpdating(methodBase, roles);
        }
        /// <summary>
        /// Method that is called before accessing any delete operation in the EntitySet class.
        /// </summary>
        /// <param name="methodBase">The method that is being accessed.</param>
        /// <param name="roles">The roles required for authorization.</param>
        protected virtual void BeforeDeleteAccessing(MethodBase methodBase, params string[] roles)
        {
            CheckAccessBeforeDeleting(methodBase, roles);
        }
        #endregion access methods

        #region partial methods
        /// <summary>
        /// Partial method that is invoked before performing any read operation on the entity set.
        /// Implement this method in a partial class to add custom authorization or validation logic before reading data.
        /// </summary>
        /// <param name="methodBase">The reflection metadata of the method being accessed for the read operation.</param>
        /// <param name="roles">The roles required for authorization.</param>
        partial void CheckAccessBeforeReading(MethodBase methodBase, params string[] roles);
        /// <summary>
        /// Partial method that is invoked before performing any create operation on the entity set.
        /// Implement this method in a partial class to add custom authorization or validation logic before creating entities.
        /// </summary>
        /// <param name="methodBase">The reflection metadata of the method being accessed for the create operation.</param>
        /// <param name="roles">The roles required for authorization.</param>
        partial void CheckAccessBeforeCreating(MethodBase methodBase, params string[] roles);
        /// <summary>
        /// Partial method that is invoked before performing any update operation on the entity set.
        /// Implement this method in a partial class to add custom authorization or validation logic before updating entities.
        /// </summary>
        /// <param name="methodBase">The reflection metadata of the method being accessed for the update operation.</param>
        /// <param name="roles">The roles required for authorization.</param>
        partial void CheckAccessBeforeUpdating(MethodBase methodBase, params string[] roles);
        /// <summary>
        /// Partial method that is invoked before performing any delete operation on the entity set.
        /// Implement this method in a partial class to add custom authorization or validation logic before deleting entities.
        /// </summary>
        /// <param name="methodBase">The reflection metadata of the method being accessed for the delete operation.</param>
        /// <param name="roles">The roles required for authorization.</param>
        partial void CheckAccessBeforeDeleting(MethodBase methodBase, params string[] roles);
        #endregion partial methods
    }
}
