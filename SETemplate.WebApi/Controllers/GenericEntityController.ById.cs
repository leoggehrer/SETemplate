//@BaseCode
#if EXTERNALGUID_OFF
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
        /// <param name="id">The ID.</param>
        /// <returns>The model.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public virtual async Task<ActionResult<TModel?>> GetByIdAsync(IdType id)
        {
            var result = await EntitySet.GetByIdAsync(id);

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

                return CreatedAtAction("Get", new { id = entity.Id }, ToModel(entity));
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
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public virtual async Task<ActionResult<TModel>> PutAsync(IdType id, [FromBody] TModel model)
        {
            try
            {
                var entity = await NoTrackingSet.FirstOrDefaultAsync(e => e.Id == id);

                if (entity != null)
                {
                    model.Id = id;
                    entity = ToEntity(model, entity);
                    await EntitySet.UpdateAsync(id, entity);
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
        /// <param name="id">The ID.</param>
        /// <param name="patchModel">The patch document.</param>
        /// <returns>The updated model.</returns>
        [HttpPatch("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public virtual async Task<ActionResult<TModel>> PatchAsync(IdType id, [FromBody] JsonPatchDocument<TModel> patchModel)
        {
            try
            {
                var entity = await NoTrackingSet.FirstOrDefaultAsync(e => e.Id == id);

                if (entity != null)
                {
                    var model = ToModel(entity);

                    patchModel.ApplyTo(model);

                    entity = ToEntity(model, entity);
                    await EntitySet.UpdateAsync(id, entity);
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
        /// <param name="id">The ID.</param>
        /// <returns>No content.</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public virtual async Task<ActionResult> DeleteAsync(IdType id)
        {
            try
            {
                var entity = await NoTrackingSet.FirstOrDefaultAsync(e => e.Id == id);

                if (entity != null)
                {
                    await EntitySet.RemoveAsync(entity.Id);
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
