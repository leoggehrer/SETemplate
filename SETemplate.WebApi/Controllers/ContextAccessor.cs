//@BaseCode
using SETemplate.Logic.DataContext;
using SETemplate.WebApi.Contracts;

namespace SETemplate.WebApi.Controllers
{
    /// <summary>
    /// Provides access to the database context and its DbSets.
    /// </summary>
    public sealed partial class ContextAccessor : IContextAccessor
    {
        #region fields
        private Logic.Contracts.IContext? _context = null;
        #endregion fields

        /// <summary>
        /// Gets the current context or creates a new one if it doesn't exist.
        /// </summary>
        /// <returns>The current context.</returns>
        public Logic.Contracts.IContext GetContext() => _context ??= Factory.CreateContext();

        /// <summary>
        /// Gets the entity set for the specified entity type.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <returns>The entity set for the specified entity type, or null if the entity type is not recognized.</returns>
        public Logic.Contracts.IEntitySet<TEntity>? GetEntitySet<TEntity>() where TEntity : Logic.Entities.EntityObject, new()
        {
            bool handled = false;
            Logic.Contracts.IEntitySet<TEntity>? result = default;

            GetEntitySetHandler<TEntity>(ref result, ref handled);

            return result;
        }

        /// <summary>
        /// Gets the view set for the specified view type.
        /// </summary>
        /// <typeparam name="TView">The type of the entity.</typeparam>
        /// <returns>The view set for the specified entity type, or null if the entity type is not recognized.</returns>
        public Logic.Contracts.IViewSet<TView>? GetViewSet<TView>() where TView : Logic.Entities.ViewObject, new()
        {
            bool handled = false;
            Logic.Contracts.IViewSet<TView>? result = default;

            GetViewSetHandler<TView>(ref result, ref handled);

            return result;
        }

        /// <summary>
        /// Disposes the current context.
        /// </summary>
        public void Dispose()
        {
            _context?.Dispose();
            _context = null;
        }

        #region partial methods
        /// <summary>
        /// Gets the DbSet for the specified entity type.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entitySet"></param>
        partial void GetEntitySetHandler<TEntity>(ref Logic.Contracts.IEntitySet<TEntity>? entitySet, ref bool handled) where TEntity : Logic.Entities.EntityObject, new();

        /// <summary>
        /// Gets the DbSet for the specified entity type.
        /// </summary>
        /// <typeparam name="TView"></typeparam>
        /// <param name="entitySet"></param>
        partial void GetViewSetHandler<TView>(ref Logic.Contracts.IViewSet<TView>? viewSet, ref bool handled) where TView : Logic.Entities.ViewObject, new();
        #endregion partial methods
    }
}
