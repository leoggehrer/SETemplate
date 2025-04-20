//@BaseCode
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
        Logic.Contracts.IContext GetContext();

        /// <summary>
        /// Gets the entity set for the specified entity type.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <returns>The entity set for the specified entity type, or null if not found.</returns>
        Logic.Contracts.IEntitySet<TEntity>? GetEntitySet<TEntity>() where TEntity : Logic.Entities.EntityObject, new();


        /// <summary>
        /// Gets the view set for the specified entity type.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <returns>The entity set for the specified entity type, or null if not found.</returns>
        Logic.Contracts.IViewSet<TEntity>? GetViewSet<TEntity>() where TEntity : Logic.Entities.ViewObject, new();
    }
}
