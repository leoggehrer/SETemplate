//@BaseCode
#if ACCOUNT_ON
using SETemplate.Common.Modules.Exceptions;
using SETemplate.Logic.Modules.Exceptions;
using SETemplate.Logic.Modules.Security;
using System.Reflection;

namespace SETemplate.Logic.DataContext
{
    /// <summary>
    /// Represents a secure entity set with authorization checks.
    /// </summary>
    [Authorize]
    partial class EntitySet<TEntity>
    {
        #region methods
        /// Determines whether the current user has permission to perform the specified action.
        /// </summary>
        /// <param name="methodName">The name of the method to check permissions for.</param>
        /// <returns>True if the user has permission; otherwise, false.</returns>
        public bool HasCurrentUserPermission(string methodName)
        {
            var result = true;
            var methodBase = GetType().GetMethod(methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);

            if (methodBase != null)
            {
                try
                {
                    CheckAccessing(methodBase);
                }
                catch (AuthorizationException)
                {
                    result = false;
                }
            }
            return result;
        }
        #endregion methods

        #region methods authorization
        /// <summary>
        /// Sets authorization for Get operations.
        /// </summary>
        /// <param name="type">The type for which to set authorization.</param>
        /// <param name="authorize">The authorization settings to apply.</param>
        internal static void SetAuthorization4Read(Type type, Modules.Security.AuthorizeAttribute authorize)
        {
            SetAuthorization(type, nameof(EntitySet<TEntity>.GetAllAsync), authorize);
            SetAuthorization(type, nameof(EntitySet<TEntity>.GetByIdAsync), authorize);
            SetAuthorization(type, nameof(EntitySet<TEntity>.QueryByIdAsync), authorize);
            SetAuthorization(type, nameof(EntitySet<TEntity>.QueryAsync), authorize);
        }
        /// <summary>
        /// Sets authorization for Create operations.
        /// </summary>
        /// <param name="type">The type for which to set authorization.</param>
        /// <param name="authorize">The authorization settings to apply.</param>
        internal static void SetAuthorization4Create(Type type, Modules.Security.AuthorizeAttribute authorize)
        {
            SetAuthorization(type, nameof(EntitySet<TEntity>.Create), authorize);
            SetAuthorization(type, nameof(EntitySet<TEntity>.AddAsync), authorize);
            SetAuthorization(type, nameof(EntitySet<TEntity>.AddRangeAsync), authorize);
        }
        /// <summary>
        /// Sets authorization for Update operations.
        /// </summary>
        /// <param name="type">The type for which to set authorization.</param>
        /// <param name="authorize">The authorization settings to apply.</param>
        internal static void SetAuthorization4Update(Type type, Modules.Security.AuthorizeAttribute authorize)
        {
            SetAuthorization(type, nameof(EntitySet<TEntity>.UpdateAsync), authorize);
        }
        /// <summary>
        /// Sets authorization for Delete operations.
        /// </summary>
        /// <param name="type">The type for which to set authorization.</param>
        /// <param name="authorize">The authorization settings to apply.</param>
        internal static void SetAuthorization4Delete(Type type, Modules.Security.AuthorizeAttribute authorize)
        {
            SetAuthorization(type, nameof(EntitySet<TEntity>.Create), authorize);
            SetAuthorization(type, nameof(EntitySet<TEntity>.AddAsync), authorize);
            SetAuthorization(type, nameof(EntitySet<TEntity>.UpdateAsync), authorize);
            SetAuthorization(type, nameof(EntitySet<TEntity>.RemoveAsync), authorize);
        }
        #endregion methods authorization

        #region partial methods
        /// <summary>
        /// Executes logic before accessing a method for read, including authorization checks.
        /// </summary>
        /// <param name="methodBase">The method being accessed for read.</param>
        /// <param name="roles">The roles required for authorization.</param>
        partial void CheckAccessBeforeReading(MethodBase methodBase, params string[] roles)
        {
            CheckReadAccessing(methodBase, roles);
        }
        /// <summary>
        /// Executes logic before accessing a method for create, including authorization checks.
        /// </summary>
        /// <param name="methodBase">The method being accessed for create.</param>
        /// <param name="roles">The roles required for authorization.</param>
        partial void CheckAccessBeforeCreating(MethodBase methodBase, params string[] roles)
        {
            CheckCreateAccessing(methodBase, roles);
        }
        /// <summary>
        /// Executes logic before accessing a method for update, including authorization checks.
        /// </summary>
        /// <param name="methodBase">The method being accessed for update.</param>
        /// <param name="roles">The roles required for authorization.</param>
        partial void CheckAccessBeforeUpdating(MethodBase methodBase, params string[] roles)
        {
            CheckUpdateAccessing(methodBase, roles);
        }
        /// <summary>
        /// Executes logic before accessing a method for delete, including authorization checks.
        /// </summary>
        /// <param name="methodBase">The method being accessed for delete.</param>
        /// <param name="roles">The roles required for authorization.</param>
        partial void CheckAccessBeforeDeleting(MethodBase methodBase, params string[] roles)
        {
            CheckDeleteAccessing(methodBase, roles);
        }
        /// <summary>
        /// Checks if the current context has access to include the specified navigation property.
        /// </summary>
        /// <param name="entityType">The type of the entity.</param>
        /// <param name="navigationPropertyName">The name of the navigation property to include.</param>
        /// <exception cref="LogicException"></exception>
        partial void CheckQueryIncludeAccess(Type entityType, string navigationPropertyName)
        {
            var navPropertySetType = GetNavigationEntitySetType(entityType, navigationPropertyName)
                                  ?? throw new LogicException(ErrorType.InvalidEntitySet, $"The entity set for the navigation property '{navigationPropertyName}' does not exist in the context.");

            var methodBase = navPropertySetType.GetMethod("QueryAsync", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)
                          ?? throw new LogicException(ErrorType.InvalidEntitySet, $"The method 'QueryAsync' does not exist on entity set type '{navPropertySetType.FullName}'.");

            CheckReadAccessing(navPropertySetType, methodBase);
        }
        #endregion partial methods
    }
}
#endif
