//@BaseCode
using SETemplate.Logic.Contracts;
using SETemplate.WebApi.Contracts;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using System.Web;

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
    public abstract partial class GenericController<TModel, TEntity, TContract> : ControllerBase, IGenericController<TModel, TContract> where TContract : Common.Contracts.IIdentifiable
        where TModel : Models.ModelObject, TContract, new()
        where TEntity : Logic.Entities.EntityObject, TContract, new()
    {
        #region fields
        private readonly IContextAccessor _contextAccessor;
        #endregion fields

        #region properties
        /// <summary>
        /// Gets the max count.
        /// </summary>
        protected virtual int MaxCount { get; } = 500;
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
        protected virtual IQueryable<TEntity> QuerySet => EntitySet.AsQuerySet();
        /// <summary>
        /// Initializes a new instance of the <see cref="GenericController{TModel, TEntity, TContract}"/> class.
        /// </summary>
        /// <param name="contextAccessor">The context accessor.</param>
        public GenericController(IContextAccessor contextAccessor)
        {
            Constructing();
            BeforeSetContextAccessor(contextAccessor);
            _contextAccessor = contextAccessor;
            AfterSetContextAccessor(ContextAccessor);
            Constructed();
        }
        /// <summary>
        /// This method is called the object is being constraucted.
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
        /// This method is called when the object is constructed.
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
        /// Gets all models.
        /// </summary>
        /// <returns>A list of models.</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public virtual ActionResult<IEnumerable<TModel>> Get()
        {
            var authHeader = HttpContext.Request.Headers.Authorization;

            var query = QuerySet.AsNoTracking().Take(MaxCount).ToArray();
            var result = query.Select(e => ToModel(e));

            return Ok(result);
        }

        /// <summary>
        /// Queries models based on a predicate.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <returns>A list of models.</returns>
        [HttpGet("/api/[controller]/query/{predicate}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public virtual ActionResult<IEnumerable<TModel>> Query(string predicate)
        {
            var query = QuerySet.AsNoTracking().Where(HttpUtility.UrlDecode(predicate)).Take(MaxCount).ToArray();
            var result = query.Select(e => ToModel(e)).ToArray();

            return Ok(result);
        }

        /// <summary>
        /// Gets a model by ID.
        /// </summary>
        /// <param name="id">The ID.</param>
        /// <returns>The model.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public virtual ActionResult<TModel?> GetById(IdType id)
        {
            var result = QuerySet.FirstOrDefault(e => e.Id == id);

            return result == null ? NotFound() : Ok(ToModel(result));
        }

        /// <summary>
        /// Creates a new model.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>The created model.</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public virtual ActionResult<TModel> Post([FromBody] TModel model)
        {
            try
            {
                var entity = ToEntity(model, null);

                EntitySet.Add(entity);
                Context.SaveChanges();

                return CreatedAtAction("Get", new { id = entity.Id }, ToModel(entity));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Updates a model by ID.
        /// </summary>
        /// <param name="id">The ID.</param>
        /// <param name="model">The model.</param>
        /// <returns>The updated model.</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public virtual ActionResult<TModel> Put(IdType id, [FromBody] TModel model)
        {
            try
            {
                var entity = QuerySet.FirstOrDefault(e => e.Id == id);

                if (entity != null)
                {
                    model.Id = id;
                    entity = ToEntity(model, entity);
                    Context.SaveChanges();
                }
                return entity == null ? NotFound() : Ok(ToModel(entity));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Partially updates a model by ID.
        /// </summary>
        /// <param name="id">The ID.</param>
        /// <param name="patchModel">The patch document.</param>
        /// <returns>The updated model.</returns>
        [HttpPatch("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public virtual ActionResult<TModel> Patch(IdType id, [FromBody] JsonPatchDocument<TModel> patchModel)
        {
            try
            {
                var entity = QuerySet.FirstOrDefault(e => e.Id == id);

                if (entity != null)
                {
                    var model = ToModel(entity);

                    patchModel.ApplyTo(model);

                    entity = ToEntity(model, entity);
                    Context.SaveChanges();
                }
                return entity == null ? NotFound() : Ok(ToModel(entity));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Deletes a model by ID.
        /// </summary>
        /// <param name="id">The ID.</param>
        /// <returns>No content.</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public virtual ActionResult Delete(IdType id)
        {
            try
            {
                var entity = QuerySet.FirstOrDefault(e => e.Id == id);

                if (entity != null)
                {
                    EntitySet.Remove(entity.Id);
                    Context.SaveChanges();
                }
                return entity == null ? NotFound() : NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
