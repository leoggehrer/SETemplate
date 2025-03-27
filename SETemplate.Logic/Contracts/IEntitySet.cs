//@BaseCode
using SETemplate.Logic.Entities;

namespace SETemplate.Logic.Contracts
{
    /// <summary>
    /// Interface for a set of entities.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    public partial interface IEntitySet<TEntity> where TEntity : EntityObject, new()
    {
        /// <summary>
        /// Creates a new instance of the entity.
        /// </summary>
        /// <returns>A new instance of the entity.</returns>
        TEntity Create();

        /// <summary>
        /// Returns an <see cref="IQueryable{TEntity}"/> that can be used to query the set of entities.
        /// </summary>
        /// <returns>An <see cref="IQueryable{TEntity}"/> that can be used to query the set of entities.</returns>
        IQueryable<TEntity> AsQuerySet();

        /// <summary>
        /// Adds a new entity to the set.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        /// <returns>The added entity.</returns>
        TEntity Add(TEntity entity);

        /// <summary>
        /// Asynchronously adds a new entity to the set.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the added entity.</returns>
        Task<TEntity> AddAsync(TEntity entity);

        /// <summary>
        /// Updates an entity in the set by its identifier.
        /// </summary>
        /// <param name="id">The identifier of the entity to update.</param>
        /// <param name="entity">The updated entity.</param>
        /// <returns>The updated entity, or null if the entity was not found.</returns>
        TEntity? Update(int id, TEntity entity);

        /// <summary>
        /// Asynchronously updates an entity in the set by its identifier.
        /// </summary>
        /// <param name="id">The identifier of the entity to update.</param>
        /// <param name="entity">The updated entity.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the updated entity, or null if the entity was not found.</returns>
        Task<TEntity?> UpdateAsync(int id, TEntity entity);

        /// <summary>
        /// Removes an entity from the set by its identifier.
        /// </summary>
        /// <param name="id">The identifier of the entity to remove.</param>
        /// <returns>The removed entity, or null if the entity was not found.</returns>
        TEntity? Remove(int id);

        /// <summary>
        /// Disposes the entity set.
        /// </summary>
        void Dispose();
    }
}