//@BaseCode
#if EXTERNALGUID_ON
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace SETemplate.WebApi.Controllers
{
    partial class GenericEntityController<TModel, TEntity, TContract>
    {
        /// <summary>
        /// Gets a model by ID.
        /// </summary>
        /// <param name="guid">The ID.</param>
        /// <returns>The model.</returns>
        [HttpGet("{guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public virtual async Task<ActionResult<TModel?>> GetByIdAsync(Guid guid)
        {
            var result = await EntitySet.GetByGuidAsync(guid);

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
        public virtual async Task<ActionResult<TModel>> PostAsync([FromBody] TModel model)
        {
            try
            {
                var entity = ToEntity(model, null);

                await EntitySet.AddAsync(entity);
                await Context.SaveChangesAsync();

                return CreatedAtAction("Get", new { id = entity.Guid }, ToModel(entity));
            }
            catch (Exception ex)
            {
                return BadRequest(GetErrorMessage(ex));
            }
        }

        /// <summary>
        /// Updates a model by ID.
        /// </summary>
        /// <param name="id">The ID.</param>
        /// <param name="model">The model.</param>
        /// <returns>The updated model.</returns>
        [HttpPut("{guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public virtual async Task<ActionResult<TModel>> PutAsync(Guid guid, [FromBody] TModel model)
        {
            try
            {
                var entity = await NoTrackingSet.FirstOrDefaultAsync(e => e.Guid == guid);

                if (entity != null)
                {
                    model.Guid = guid;
                    entity = ToEntity(model, entity);
                    await EntitySet.UpdateByGuidAsync(guid, entity);
                    await Context.SaveChangesAsync();
                }
                return entity == null ? NotFound() : Ok(ToModel(entity));
            }
            catch (Exception ex)
            {
                return BadRequest(GetErrorMessage(ex));
            }
        }
        /// <summary>
        /// Partially updates a model by ID.
        /// </summary>
        /// <param name="guid">The ID.</param>
        /// <param name="patchModel">The patch document.</param>
        /// <returns>The updated model.</returns>
        [HttpPatch("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public virtual async Task<ActionResult<TModel>> PatchAsync(Guid guid, [FromBody] JsonPatchDocument<TModel> patchModel)
        {
            try
            {
                var entity = await NoTrackingSet.FirstOrDefaultAsync(e => e.Guid == guid);

                if (entity != null)
                {
                    var model = ToModel(entity);

                    patchModel.ApplyTo(model);

                    entity = ToEntity(model, entity);
                    await EntitySet.UpdateByGuidAsync(guid, entity);
                    await Context.SaveChangesAsync();
                }
                return entity == null ? NotFound() : Ok(ToModel(entity));
            }
            catch (Exception ex)
            {
                return BadRequest(GetErrorMessage(ex));
            }
        }

        /// <summary>
        /// Deletes a model by ID.
        /// </summary>
        /// <param name="guid">The ID.</param>
        /// <returns>No content.</returns>
        [HttpDelete("{guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public virtual async Task<ActionResult> DeleteAsync(Guid guid)
        {
            try
            {
                var entity = await NoTrackingSet.FirstOrDefaultAsync(e => e.Guid == guid);

                if (entity != null)
                {
                    await EntitySet.RemoveByGuidAsync(entity.Guid);
                    await Context.SaveChangesAsync();
                }
                return entity == null ? NotFound() : NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(GetErrorMessage(ex));
            }
        }
    }
}
#endif
