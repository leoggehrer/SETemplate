//@BaseCode
#if EXTERNALGUID_OFF
namespace SETemplate.Logic.Contracts
{
    /// <summary>
    /// Interface for a set of entities.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    partial interface IEntitySet<TEntity>
    {
        /// <summary>
        /// Returns the element of type T with the identification of id.
        /// </summary>
        /// <param name = "id">The identification.</param>
        /// <returns>The element of the type T with the corresponding identification.</returns>
        ValueTask<TEntity?> GetByIdAsync(IdType id);

        /// <summary>
        /// Returns the entity with the specified identifier without tracking.
        /// </summary>
        /// <param name="id">The identifier of the entity.</param>
        /// <returns>The entity with the specified identifier, or null if not found.</returns>
        Task<TEntity?> QueryByIdAsync(IdType id);

        /// <summary>
        /// Asynchronously updates an entity in the set by its identifier.
        /// </summary>
        /// <param name="id">The identifier of the entity to update.</param>
        /// <param name="entity">The updated entity.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the updated entity, or null if the entity was not found.</returns>
        Task<TEntity?> UpdateAsync(IdType id, TEntity entity);

        /// <summary>
        /// Asynchronously removes an entity from the set by its identifier.
        /// </summary>
        /// <param name="id">The identifier of the entity to remove.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains the removed entity, or null if the entity was not found.
        /// </returns>
        Task<TEntity?> RemoveAsync(IdType id);
    }
}
#endif
