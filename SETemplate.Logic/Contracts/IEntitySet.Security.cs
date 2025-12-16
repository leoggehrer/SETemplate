//@BaseCode
#if ACCOUNT_ON
using SETemplate.Logic.Entities;

namespace SETemplate.Logic.Contracts
{
    public partial interface IEntitySet<TEntity> where TEntity : EntityObject, new()
    {
        #region properties
        /// <summary>
        /// Sets the session token.
        /// </summary>
        string SessionToken { set; }
        #endregion properties

        #region methods
        /// <summary>
        /// Determines whether the current user has permission for the specified method.
        /// </summary>
        /// <param name="methodName">The name of the method to check permissions for.</param>
        /// <returns>True if the user has permission; otherwise, false.</returns>
        bool HasCurrentUserPermission(string methodName);
        #endregion methods
    }
}
#endif
