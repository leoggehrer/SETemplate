//@BaseCode
using System.Linq.Dynamic.Core;
using System.Linq.Dynamic.Core.Exceptions;
using System.Reflection;
using SQLitePCL;

namespace SETemplate.Logic.DataContext
{
    partial class SetBase<TElement>
    {
        #region properties
        protected virtual int MaxCount { get; } = 500;
        protected virtual ParsingConfig ParsingConfig
        {
            get
            {
                return new ParsingConfig
                {
                    ResolveTypesBySimpleName = true,
                    AllowNewToEvaluateAnyType = false,
                    EvaluateGroupByAtDatabase = true,
                };
            }
        }
        #endregion properties

        #region overridables
        /// <summary>
        /// Copies properties from the source element to the target element.
        /// </summary>
        /// <param name="target">The target element.</param>
        /// <param name="source">The source element.</param>
        protected abstract void CopyProperties(TElement target, TElement source);
        #endregion overridables

        #region methods
        /// <summary>
        /// Returns the count of elements in the set asynchronously.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains the count of elements.</returns>
        internal virtual Task<int> ExecuteCountAsync()
        {
            return DbSet.CountAsync();
        }

        /// <summary>
        /// Gets the queryable set of elements.
        /// </summary>
        /// <returns>An <see cref="IQueryable{TElement}"/> that can be used to query the set of elements.</returns>
        internal virtual IQueryable<TElement> ExecuteAsQuerySet() => DbSet.AsQueryable();

        /// <summary>
        /// Gets the no-tracking queryable set of elements.
        /// </summary>
        /// <returns>An <see cref="IQueryable{TElement}"/> that can be used to query the set of elements without tracking changes.</returns>
        internal virtual IQueryable<TElement> ExecuteAsNoTrackingSet() => ExecuteAsQuerySet().AsNoTracking();

        /// <summary>
        /// Retrieves all elements from the set without tracking changes.
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains a collection of elements limited to <see cref="MaxCount"/>.
        /// </returns>
        internal virtual async Task<IEnumerable<TElement>> ExecuteGetAllAsync()
        {
            var result = await ExecuteAsNoTrackingSet().Take(MaxCount).ToArrayAsync().ConfigureAwait(false);
            return (IEnumerable<TElement>)result;
        }

        /// <summary>
        /// Queries elements from the set based on the provided query parameters.
        /// </summary>
        /// <param name="queryParams">The query parameters containing filter, values, and includes.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a collection of elements matching the query criteria.</returns>
        /// <exception cref="Modules.Exceptions.LogicException">Thrown when the filter expression is empty or invalid.</exception>
        internal virtual async Task<IEnumerable<TElement>> ExecuteQueryAsync(Models.QueryParams queryParams)
        {
            try
            {
                var set = ExecuteAsNoTrackingSet();
                var query = default(TElement[]);

                foreach (var include in queryParams.Includes ?? [])
                {
                    var navPropertyInfo = typeof(TElement).GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                                                          .Where(e => e.Name.Equals(include, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault(); 
                    var propertyName = navPropertyInfo != null ? navPropertyInfo.Name : include;

                    if (!string.IsNullOrWhiteSpace(propertyName))
                    {
                        BeforeIncludeExecuting(propertyName);
                        set = set.Include(propertyName);
                    }
                }

                if (string.IsNullOrWhiteSpace(queryParams.SortBy) == false)
                {
                    set = set.OrderBy(queryParams.SortBy);
                }

                if (string.IsNullOrWhiteSpace(queryParams.Filter) == false
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
        /// Gets a <see cref="Type"/> by searching loaded assemblies for a type with the specified namespace part and type name.
        /// </summary>
        /// <param name="partOfNamespace">The part of the namespace to search for.</param>
        /// <param name="typeName">The name of the type to search for.</param>
        /// <returns>The <see cref="Type"/> if found; otherwise, null.</returns>
        protected static Type? GetTypeBy(string partOfNamespace, string typeName)
        {
            var result = default(Type?);
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var enumerator = assemblies.GetEnumerator();

            while (result == null && enumerator.MoveNext())
            {
                var assembly = (Assembly)enumerator.Current;

                if (assembly != null)
                {
                    result = assembly.GetTypes().FirstOrDefault(t => t.Namespace != null && t.Namespace.Contains(partOfNamespace, StringComparison.OrdinalIgnoreCase) && t.Name.Equals(typeName, StringComparison.OrdinalIgnoreCase));
                }
            }
            return result;
        }

        /// <summary>
        /// Gets the type of a navigation property for a given entity type.
        /// </summary>
        /// <param name="entityType">The type of the entity.</param>
        /// <param name="navigationPropertyName">The name of the navigation property.</param>
        /// <returns>The type of the navigation property, or null if not found.</returns>
        protected static Type? GetNavigationPropertyType(Type entityType, string navigationPropertyName)
        {
            var result = default(Type?);
            var navPropertyInfo = entityType.GetProperty(navigationPropertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            if (navPropertyInfo != null && navPropertyInfo.PropertyType.IsGenericType)
            {
                result = navPropertyInfo.PropertyType.GetGenericArguments().First();
            }
            else if (navPropertyInfo != null && navPropertyInfo.PropertyType.IsClass)
            {
                result = navPropertyInfo.PropertyType;
            }
            return result;
        }
        /// <summary>
        /// Gets the entity set type for a given navigation property of an entity type.
        /// </summary>
        /// <param name="entityType">The type of the entity.</param>
        /// <param name="navigationPropertyName">The name of the navigation property.</param>
        /// <returns>The type of the navigation entity set, or null if not found.</returns>
        protected static Type? GetNavigationEntitySetType(Type entityType, string navigationPropertyName)
        {
            var result = default(Type?);
            var navPropertyType = GetNavigationPropertyType(entityType, navigationPropertyName);

            if (navPropertyType != null)
            {
                result = GetTypeBy("Logic.DataContext", $"{navPropertyType.Name}Set");
            }
            return result;
        }
        #endregion methods

        #region partial methods
        /// <summary>
        /// Called before including a navigation property in a query. Override to add access checks.
        /// </summary>
        /// <param name="include">The include path being applied.</param>
        partial void BeforeIncludeExecuting(string include);
        #endregion partial methods
    }
}
