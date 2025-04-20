//@BaseCode
#if EXTERNALGUID_ON
namespace SETemplate.Logic.Contracts
{
    partial interface IEntitySet<TEntity>
    {
        /// <summary>
        /// Returns the element of type T with the identification of id.
        /// </summary>
        /// <param name = "guid">The identification.</param>
        /// <returns>The element of the type T with the corresponding identification.</returns>
        Task<TEntity?> GetByGuidAsync(Guid guid);

        /// <summary>
        /// Updates an entity in the set by its external Guid.
        /// </summary>
        /// <param name="guid">The external Guid of the entity to update.</param>
        /// <param name="entity">The updated entity.</param>
        /// <returns>The updated entity, or null if the entity was not found.</returns>
        TEntity? UpdateByGuid(Guid guid, TEntity entity);
        /// <summary>
        /// Asynchronously updates an entity in the set by its external Guid.
        /// </summary>
        /// <param name="guid">The external Guid of the entity to update.</param>
        /// <param name="entity">The updated entity.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the updated entity, or null if the entity was not found.</returns>
        Task<TEntity?> UpdateByGuidAsync(Guid guid, TEntity entity);
        /// <summary>
        /// Removes an entity from the set by its external Guid.
        /// </summary>
        /// <param name="guid">The external Guid of the entity to remove.</param>
        /// <returns>The removed entity, or null if the entity was not found.</returns>
        TEntity? RemoveByGuid(Guid guid);
    }
}
#endif
