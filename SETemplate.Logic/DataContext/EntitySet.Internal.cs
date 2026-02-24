//@BaseCode
using System.Linq.Dynamic.Core;
using System.Linq.Dynamic.Core.Exceptions;

namespace SETemplate.Logic.DataContext
{
    partial class EntitySet<TEntity>
    {
        #region properties
        protected virtual int MaxCount { get; } = 500;
        protected virtual ParsingConfig ParsingConfig
        {
            get
            {
                return new ParsingConfig
                {
                    // Erlaubt Class-Names („DateTime“) ohne Namensraum
                    ResolveTypesBySimpleName = true,

                    // Verhindert, dass über „new“ beliebige Typen zur Laufzeit erzeugt werden
                    AllowNewToEvaluateAnyType = false,

                    // Optional: Wenn du LINQ.GroupBy schon auf der Datenbank ausführen lassen willst:
                    EvaluateGroupByAtDatabase = true,
                };
            }
        }
        #endregion properties

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
        protected virtual TEntity? CreateInstance() { return default; }
        /// <summary>
        /// Performs actions after an entity is created.
        /// </summary>
        /// <param name="entity">The newly created entity.</param>
        protected virtual void OnCreated(TEntity entity) { }

        /// <summary>
        /// Performs actions before adding an entity to the set.
        /// </summary>
        /// <param name="entity">The entity to be added.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        protected virtual Task BeforePersistingAddAsync(TEntity entity) => Task.CompletedTask;

        /// <summary>
        /// Performs actions after an entity has been added to the set asynchronously.
        /// </summary>
        /// <param name="entity">The entity that was added.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        protected virtual Task OnAddedAsync(TEntity entity) => Task.CompletedTask;
        /// <summary>
        /// Performs actions before updating an entity in the set.
        /// </summary>
        /// <param name="entity">The entity to be updated.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        protected virtual Task BeforePersistingUpdateAsync(TEntity entity) => Task.CompletedTask;
        /// <summary>
        /// Performs actions after an entity has been updated in the set asynchronously.
        /// </summary>
        /// <param name="entity">The entity that was updated.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        protected virtual Task OnUpdatedAsync(TEntity entity) => Task.CompletedTask;

        /// <summary>
        /// Performs actions before removing an entity from the set.
        /// </summary>
        /// <param name="entity">The entity to be removed.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        protected virtual Task BeforePersistingRemoveAsync(TEntity entity) => Task.CompletedTask;
        /// <summary>
        /// Performs actions after an entity has been removed from the set asynchronously.
        /// </summary>
        /// <param name="entity">The entity that was removed.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        protected virtual Task OnRemovedAsync(TEntity entity) => Task.CompletedTask;
        #endregion overridables

        #region methods
        /// <summary>
        /// Creates a new instance of the entity.
        /// </summary>
        /// <returns>A new instance of the entity.</returns>
        internal virtual TEntity ExecuteCreate()
        {
            var result = CreateInstance();

            if (result == default)
            {
                result = new TEntity();
            }
            OnCreated(result);
            return result;
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
        /// Returns the entity with the specified identifier without tracking.
        /// </summary>
        /// <param name="id">The identifier of the entity.</param>
        /// <returns>The entity with the specified identifier, or null if not found.</returns>
        internal virtual Task<TEntity?> ExecuteQueryByIdAsync(IdType id)
        {
            return ExecuteAsNoTrackingSet().FirstOrDefaultAsync(e => e.Id == id);
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
        internal virtual Task<IEnumerable<TEntity>> ExecuteGetAllAsync()
        {
            return ExecuteAsNoTrackingSet().Take(MaxCount).ToArrayAsync().ContinueWith(t => (IEnumerable<TEntity>)t.Result);
        }

        /// <summary>
        /// Queries entities from the set based on the provided query parameters.
        /// </summary>
        /// <param name="queryParams">The query parameters containing filter, values, and includes.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a collection of entities matching the query criteria.</returns>
        /// <exception cref="Modules.Exceptions.LogicException">Thrown when the filter expression is empty or invalid.</exception>
        /// <remarks>
        /// The query will include the specified navigation properties and apply the provided filter expression.
        /// Results are limited to the <see cref="MaxCount"/> value to prevent excessive data retrieval.
        /// </remarks>
        internal virtual async Task<IEnumerable<TEntity>> ExecuteQueryAsync(Models.QueryParams queryParams)
        {
            if (string.IsNullOrWhiteSpace(queryParams.Filter))
                throw new Modules.Exceptions.LogicException("The filter printout must not be empty.");

            try
            {
                var set = ExecuteAsNoTrackingSet();
                var query = default(TEntity[]);

                foreach (var include in queryParams.Includes ?? [])
                {
                    // There is no ThenInclude overload for string-based includes in EF Core.
                    // Only the first-level .Include(string) is supported for string paths (dot-separated).
                    // For nested navigation properties, use a single string with dot-notation: Include("Parent.Child.GrandChild")
                    // So, just call set = set.Include(include); once per include string.
                    if (!string.IsNullOrWhiteSpace(include))
                    {
#if ACCOUNT_ON                        
                        var entityType = typeof(TEntity);
                        var includeItems = include.Split('.');

                        for (int i = 0; i < includeItems.Length; i++)
                        {
                            if (i == 0)
                            {
                                CheckQueryIncludeAccess(entityType, includeItems[i]);
                            }
                            else
                            {
                                var navPropertyType = GetNavigationPropertyType(entityType, includeItems[i - 1])
                                                   ?? throw new Modules.Exceptions.LogicException($"The navigation property '{includeItems[i - 1]}' does not exist on type '{entityType.FullName}'.");

                                entityType = navPropertyType;
                                CheckQueryIncludeAccess(entityType, includeItems[i]);
                            }
                        }
#endif
                        set = set.Include(include);
                    }
                }

                if (string.IsNullOrWhiteSpace(queryParams.SortBy) == false)
                {
                    set = set.OrderBy(queryParams.SortBy);
                }

                if (queryParams.Filter != null
                    && queryParams.Values != null
                    && queryParams.Values.Length > 0)
                {
                    query = await set.Where(ParsingConfig, queryParams.Filter, queryParams.Values)
                                     .Take(MaxCount)
                                     .ToArrayAsync()
                                     .ConfigureAwait(false);
                }
                else
                {
                    query = await set.Take(MaxCount)
                                     .ToArrayAsync()
                                     .ConfigureAwait(false);
                }
                return query;
            }
            catch (ParseException ex)
            {
                throw new Modules.Exceptions.LogicException($"Invalid filter expression: {ex.Message}");
            }
        }

        /// <summary>
        /// Asynchronously adds the specified entity to the set.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the added entity.</returns>
        internal virtual async Task<TEntity> ExecuteAddAsync(TEntity entity)
        {
            PrepareAdd(entity);

            await BeforePersistingAddAsync(entity).ConfigureAwait(false);
            var result = await DbSet.AddAsync(entity).ConfigureAwait(false);

            await OnAddedAsync(result.Entity).ConfigureAwait(false);
            return result.Entity;
        }

        /// <summary>
        /// Asynchronously adds a range of entities to the set.
        /// </summary>
        /// <param name="entities">The collection of entities to add.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains a collection of the added entities.
        /// </returns>
        internal virtual async Task<IEnumerable<TEntity>> ExecuteAddRangeAsync(IEnumerable<TEntity> entities)
        {
            var result = new List<TEntity>();

            foreach (var e in entities)
            {
                PrepareAdd(e);
                await BeforePersistingAddAsync(e).ConfigureAwait(false);
                result.Add(e);
            }
            await DbSet.AddRangeAsync(result).ConfigureAwait(false);
            foreach (var entity in result)
            {
                await OnAddedAsync(entity).ConfigureAwait(false);
            }
            return result;
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
            PrepareUpdate(entity);

            var existingEntity = await DbSet.FindAsync(id).ConfigureAwait(false);

            if (existingEntity != null)
            {
                await BeforePersistingUpdateAsync(existingEntity).ConfigureAwait(false);
                CopyProperties(existingEntity, entity);
                await OnUpdatedAsync(existingEntity).ConfigureAwait(false);
            }
            return existingEntity;
        }

        /// <summary>
        /// Asynchronously removes the entity with the specified identifier from the set.
        /// </summary>
        /// <param name="id">The identifier of the entity to remove.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains the removed entity, or null if the entity was not found.
        /// </returns>
        internal virtual async Task<TEntity?> ExecuteRemoveAsync(IdType id)
        {
            var existingEntity = await DbSet.FindAsync(id).ConfigureAwait(false);

            if (existingEntity != null)
            {
                PrepareRemove(existingEntity);
                await BeforePersistingRemoveAsync(existingEntity).ConfigureAwait(false);
                DbSet.Remove(existingEntity);
                await OnRemovedAsync(existingEntity).ConfigureAwait(false);
            }
            return existingEntity;
        }
        #endregion methods

        #region partial methods
        /// <summary>
        /// Performs actions before adding an entity to the set.
        /// </summary>
        /// <param name="entity">The entity to be prepared for addition.</param>
        partial void PrepareAdd(TEntity entity);
        /// <summary>
        /// Performs actions before updating an entity in the set.
        /// </summary>
        /// <param name="entity">The entity to be updated.</param>
        partial void PrepareUpdate(TEntity entity);
        /// <summary>
        /// Performs actions before removing an entity from the set.
        /// </summary>
        /// <param name="entity">The entity to be prepared for removal.</param>
        partial void PrepareRemove(TEntity entity);
        /// <summary>
        /// Checks if the current context has access to include the specified navigation property.
        /// </summary>
        /// <param name="entityType">The entity type for which to check include access.</param>
        /// <param name="navigationPropertyName">The navigation property name to check access for.</param>
        partial void CheckQueryIncludeAccess(Type entityType, string navigationPropertyName);
        #endregion partial methods
    }
}
