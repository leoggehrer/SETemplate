//@BaseCode
using Microsoft.AspNetCore.Mvc;
using SETemplate.Logic.Contracts;
using SETemplate.WebApi.Contracts;
using System.Linq.Dynamic.Core;

namespace SETemplate.WebApi.Controllers
{
    /// <summary>
    /// A generic controller for handling CRUD operations.
    /// </summary>
    /// <typeparam name="TModel">The type of the model.</typeparam>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TContract">The type of the interface.</typeparam>
    [Route("api/[controller]")]
    [ApiController]
    public abstract partial class GenericEntityController<TModel, TEntity, TContract> : ApiControllerBase, IGenericEntityController<TModel, TContract> where TContract : CommonContracts.IIdentifiable
        where TModel : CommonModels.ModelObject, TContract, new()
        where TEntity : Logic.Entities.EntityObject, TContract, new()
    {
        #region fields
        private readonly IContextAccessor _contextAccessor;
        #endregion fields

        #region properties
        /// <summary>
        /// Gets the context accessor.
        /// </summary>
        protected IContextAccessor ContextAccessor
        {
            get
            {
                OnReadContextAccessor(_contextAccessor);
                return _contextAccessor;
            }
        }
        partial void OnReadContextAccessor(IContextAccessor contextAccessor);

        /// <summary>
        /// Gets the context.
        /// </summary>
        protected virtual IContext Context => ContextAccessor.GetContext();
        /// <summary>
        /// Gets the DbSet.
        /// </summary>
        protected virtual IEntitySet<TEntity> EntitySet => ContextAccessor.GetEntitySet<TEntity>() ?? throw new Exception($"Invalid DbSet<{typeof(TEntity)}>");
        /// <summary>
        /// Gets the IQueriable<TEntity>.
        /// </summary>
        //protected virtual IQueryable<TEntity> NoTrackingSet => EntitySet.AsNoTrackingSet();
        /// <summary>
        /// Initializes a new instance of the <see cref="GenericEntityController{TModel, TEntity, TContract}"/> class.
        /// </summary>
        /// <param name="contextAccessor">The context accessor.</param>
        public GenericEntityController(IContextAccessor contextAccessor)
        {
            Constructing();
            BeforeSetContextAccessor(contextAccessor);
            _contextAccessor = contextAccessor;
            AfterSetContextAccessor(ContextAccessor);
            Constructed();
        }
        /// <summary>
        /// This method is called when the object is constructed.
        /// </summary>
        partial void Constructing();

        /// <summary>
        /// This method is called before setting the context accessor.
        /// </summary>
        /// <param name="contextAccessor">The context accessor.</param>
        partial void BeforeSetContextAccessor(IContextAccessor contextAccessor);
        /// <summary>
        /// This method is called after setting the context accessor.
        /// </summary>
        /// <param name="contextAccessor">The context accessor.</param>
        partial void AfterSetContextAccessor(IContextAccessor contextAccessor);
        /// <summary>
        /// This method is called after the object has been initialized.
        /// </summary>
        partial void Constructed();
        #endregion constructors

        /// <summary>
        /// Converts an entity to a model.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>The model.</returns>
        protected abstract TModel ToModel(TEntity entity);

        /// <summary>
        /// Converts an model to a entity.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="entity">The entity.</param>
        /// <returns>The entity.</returns>
        protected abstract TEntity ToEntity(TModel model, TEntity? entity);

        /// <summary>
        /// Returns a new instance object with default values.
        /// </summary>
        /// <returns> A 200 OK response containing a default-initialized object.</returns>
        [HttpGet("template")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public virtual ActionResult GetTemplate()
        {
            var result = EntitySet.Create();

            return Ok(result);
        }

        /// <summary>
        /// Counts the models.
        /// </summary>
        /// <returns>The count of models.</returns>
        [HttpGet("count")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public virtual async Task<ActionResult> CountAsync()
        {
            var result = await EntitySet.CountAsync();

            return Ok(result);
        }

        /// <summary>
        /// Gets all models.
        /// </summary>
        /// <returns>A list of models.</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public virtual async Task<ActionResult<IEnumerable<TModel>>> GetAsync()
        {
            var query = await EntitySet.GetAllAsync();
            var result = query.Select(e => ToModel(e));

            return Ok(result);
        }

        /// <summary>
        /// Queries models based on a predicate.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <returns>A list of models.</returns>
        [HttpPost("query")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public virtual async Task<ActionResult<IEnumerable<TModel>>> QueryAsync([FromBody] Logic.Models.QueryParams queryParams)
        {
            if (string.IsNullOrWhiteSpace(queryParams.Filter))
                return BadRequest("The filter printout must not be empty.");

            var query = await EntitySet.QueryAsync(queryParams);
            var result = query.Select(e => ToModel(e));

            return Ok(result);
        }
    }
}
