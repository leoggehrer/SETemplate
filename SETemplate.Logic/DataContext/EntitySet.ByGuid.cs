//@BaseCode
#if EXTERNALGUID_ON
using System.Reflection;

namespace SETemplate.Logic.DataContext
{
    partial class EntitySet<TEntity>
    {
        /// <summary>
        /// Returns the element of type T with the identification of id.
        /// </summary>
        /// <param name="guid">The identification.</param>
        /// <returns>The element of the type T with the corresponding identification.</returns>
        public virtual Task<TEntity?> GetByGuidAsync(Guid guid)
        {
            BeforeReadAccessing(MethodBase.GetCurrentMethod()!.GetAsyncOriginal());

            return ExecuteGetByGuidAsync(guid);
        }

        /// <summary>
        /// Updates the specified entity in the set.
        /// </summary>
        /// <param name="guid">The identifier of the entity to update.</param>
        /// <param name="entity">The entity with updated values.</param>
        /// <returns>The updated entity, or null if the entity was not found.</returns>
        public virtual Task<TEntity?> UpdateByGuidAsync(Guid guid, TEntity entity)
        {
            BeforeUpdateAccessing(MethodBase.GetCurrentMethod()!.GetAsyncOriginal());

            return ExecuteUpdateByGuidAsync(guid, entity);
        }

        /// <summary>
        /// Removes the entity with the specified identifier from the set.
        /// </summary>
        /// <param name="guid">The identifier of the entity to remove.</param>
        /// <returns>The removed entity, or null if the entity was not found.</returns>
        public virtual Task<TEntity?> RemoveByGuidAsync(Guid guid)
        {
            BeforeUpdateAccessing(MethodBase.GetCurrentMethod()!.GetAsyncOriginal());

            return ExecuteRemoveByGuidAsync(guid);
        }
    }
}
#endif
