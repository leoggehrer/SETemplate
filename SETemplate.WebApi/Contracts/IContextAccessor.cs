//@BaseCode
using SETemplate.Logic.Contracts;
using SETemplate.Logic.DataContext;

namespace SETemplate.WebApi.Contracts
{
    /// <summary>
    /// Provides access to the context and entity sets.
    /// </summary>
    public partial interface IContextAccessor : IDisposable
    {
        /// <summary>
        /// Gets the current context.
        /// </summary>
        /// <returns>The current context.</returns>
        IContext GetContext();

        /// <summary>
        /// Gets the entity set for the specified entity type.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <returns>The entity set for the specified entity type, or null if not found.</returns>
        EntitySet<TEntity>? GetEntitySet<TEntity>() where TEntity : Logic.Entities.EntityObject, new();
    }
}
