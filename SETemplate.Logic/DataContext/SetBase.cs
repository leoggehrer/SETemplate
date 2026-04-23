//@BaseCode
namespace SETemplate.Logic.DataContext
{
    /// <summary>
    /// Provides a common base class for <see cref="EntitySet{TEntity}"/> and <see cref="ViewSet{TView}"/>,
    /// encapsulating shared database context, DbSet access, and disposal logic.
    /// </summary>
    /// <typeparam name="TElement">The type of the database element.</typeparam>
    internal abstract partial class SetBase<TElement> : IDisposable
        where TElement : Entities.DbObject, new()
    {
        #region fields
        private ProjectDbContext? _context;
        private DbSet<TElement>? _dbSet;
        #endregion fields

        #region constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="SetBase{TElement}"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="dbSet">The set of elements.</param>
        internal SetBase(ProjectDbContext context, DbSet<TElement> dbSet)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _dbSet = dbSet ?? throw new ArgumentNullException(nameof(dbSet));
        }
        #endregion constructors

        #region properties
        /// <summary>
        /// Gets the database context.
        /// </summary>
        internal ProjectDbContext Context => _context!;
        /// <summary>
        /// Gets the database set.
        /// </summary>
        protected DbSet<TElement> DbSet => _dbSet!;
        #endregion properties

        #region methods
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            _dbSet = null;
            _context = null;
            GC.SuppressFinalize(this);
        }
        #endregion methods
    }
}
