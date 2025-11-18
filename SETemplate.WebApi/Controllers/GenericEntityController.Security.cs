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
        [HttpGet("hasCurrentUserPermission/{actionName}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public virtual ActionResult HasCurrentUserPermission(string actionName)
        {
            var result = false;

            if (actionName.Equals("count", StringComparison.CurrentCultureIgnoreCase))
            {
                result = EntitySet.HasCurrentUserPermission(nameof(EntitySet.CountAsync));
            }
            else if (actionName.Equals("get", StringComparison.CurrentCultureIgnoreCase))
            {
                result = EntitySet.HasCurrentUserPermission(nameof(EntitySet.GetAsync));
            }
            else if (actionName.Equals("query", StringComparison.CurrentCultureIgnoreCase))
            {
                result = EntitySet.HasCurrentUserPermission(nameof(EntitySet.QueryAsync));
            }
            else if (actionName.Equals("create", StringComparison.CurrentCultureIgnoreCase))
            {
                result = EntitySet.HasCurrentUserPermission(nameof(EntitySet.AddAsync));
            }
            else if (actionName.Equals("update", StringComparison.CurrentCultureIgnoreCase))
            {
                result = EntitySet.HasCurrentUserPermission(nameof(EntitySet.UpdateAsync));
            }
            else if (actionName.Equals("delete", StringComparison.CurrentCultureIgnoreCase))
            {
                result = EntitySet.HasCurrentUserPermission(nameof(EntitySet.RemoveAsync));
            }
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
