//@BaseCode
#if ACCOUNT_ON
using Microsoft.AspNetCore.Mvc;
using SETemplate.WebApi.Contracts;

namespace SETemplate.WebApi.Controllers
{
    partial class GenericEntityController<TModel, TEntity, TContract>
    {
        #region methods
        /// <summary>
        /// Checks if the current user has permission for the specified action.
        /// </summary>
        /// <param name="actionName">The name of the action to check permissions for.</param>
        /// <returns>True if the user has permission; otherwise, false.</returns>
        private static readonly HashSet<string> AllowedActionNames =
            new(StringComparer.OrdinalIgnoreCase) { "count", "get", "query", "create", "update", "delete" };

        [HttpGet("hasCurrentUserPermission/{actionName}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public virtual ActionResult HasCurrentUserPermission(string actionName)
        {
            if (!AllowedActionNames.Contains(actionName))
            {
                return BadRequest("Invalid action name.");
            }

            var result = false;

            if (actionName.Equals("count", StringComparison.OrdinalIgnoreCase))
            {
                result = EntitySet.HasCurrentUserPermission(nameof(EntitySet.CountAsync));
            }
            else if (actionName.Equals("get", StringComparison.OrdinalIgnoreCase))
            {
                result = EntitySet.HasCurrentUserPermission(nameof(EntitySet.GetAllAsync));
            }
            else if (actionName.Equals("query", StringComparison.OrdinalIgnoreCase))
            {
                result = EntitySet.HasCurrentUserPermission(nameof(EntitySet.QueryAsync));
            }
            else if (actionName.Equals("create", StringComparison.OrdinalIgnoreCase))
            {
                result = EntitySet.HasCurrentUserPermission(nameof(EntitySet.AddAsync));
            }
#if EXTERNALGUID_ON
            else if (actionName.Equals("update", StringComparison.OrdinalIgnoreCase))
            {
                result = EntitySet.HasCurrentUserPermission(nameof(EntitySet.UpdateByGuidAsync));
            }
            else if (actionName.Equals("delete", StringComparison.OrdinalIgnoreCase))
            {
                result = EntitySet.HasCurrentUserPermission(nameof(EntitySet.RemoveByGuidAsync));
            }
#else
            else if (actionName.Equals("update", StringComparison.OrdinalIgnoreCase))
            {
                result = EntitySet.HasCurrentUserPermission(nameof(EntitySet.UpdateAsync));
            }
            else if (actionName.Equals("delete", StringComparison.OrdinalIgnoreCase))
            {
                result = EntitySet.HasCurrentUserPermission(nameof(EntitySet.RemoveAsync));
            }
#endif
            return Ok(result);
        }
        #endregion methods

        #region partial methods
        partial void OnReadContextAccessor(IContextAccessor contextAccessor)
        {
            contextAccessor.SessionToken = SessionToken;
        }
        #endregion partial methods
    }
}
#endif
