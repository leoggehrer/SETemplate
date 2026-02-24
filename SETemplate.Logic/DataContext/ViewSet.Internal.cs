//@BaseCode
using System.Linq.Dynamic.Core;
using System.Linq.Dynamic.Core.Exceptions;

namespace SETemplate.Logic.DataContext
{
    partial class ViewSet<TView>
    {
        #region overridables
        /// <summary>
        /// Copies properties from the source entity to the target entity.
        /// </summary>
        /// <param name="target">The target entity.</param>
        /// <param name="source">The source entity.</param>
        protected abstract void CopyProperties(TView target, TView source);
        #endregion overridables

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

        #region methods
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
        internal virtual IQueryable<TView> ExecuteAsQuerySet() => DbSet.AsQueryable();

        /// <summary>
        /// Gets the no-tracking queryable set of entities.
        /// </summary>
        /// <returns>An <see cref="IQueryable{TEntity}"/> that can be used to query the set of entities without tracking changes.</returns>
        internal virtual IQueryable<TView> ExecuteAsNoTrackingSet() => ExecuteAsQuerySet().AsNoTracking();

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
        internal virtual Task<IEnumerable<TView>> ExecuteGetAllAsync()
        {
            return ExecuteAsNoTrackingSet().Take(MaxCount).ToArrayAsync().ContinueWith(t => (IEnumerable<TView>)t.Result);
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
        internal virtual async Task<IEnumerable<TView>> ExecuteQueryAsync(Models.QueryParams queryParams)
        {
            if (string.IsNullOrWhiteSpace(queryParams.Filter))
                throw new Modules.Exceptions.LogicException("The filter printout must not be empty.");

            try
            {
                var set = ExecuteAsNoTrackingSet();
                var query = default(TView[]);

                foreach (var include in queryParams.Includes ?? [])
                {
                    if (!string.IsNullOrWhiteSpace(include))
                        set = set.Include(include);
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
    }
}
