//@BaseCode
#if EXTERNALGUID_OFF
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace SETemplate.WebApi.Contracts
{
    /// <summary>
    /// Defines a generic controller interface for handling operations by ID.
    /// </summary>
    /// <typeparam name="TModel">The type of the model.</typeparam>
    /// <typeparam name="TContract">The type of the contract.</typeparam>
    partial interface IGenericEntityController<TModel, TContract>
    {
        /// <summary>
        /// Retrieves a model by its ID.
        /// </summary>
        /// <param name="id">The ID of the model to retrieve.</param>
        /// <returns>An <see cref="ActionResult"/> containing the model if found, or null if not found.</returns>
        Task<ActionResult<TModel?>> GetByIdAsync(IdType id);

        /// <summary>
        /// Updates a model by its ID.
        /// </summary>
        /// <param name="id">The ID of the model to update.</param>
        /// <param name="model">The updated model data.</param>
        /// <returns>An <see cref="ActionResult"/> containing the updated model.</returns>
        Task<ActionResult<TModel>> PutAsync(IdType id, [FromBody] TModel model);

        /// <summary>
        /// Applies a JSON Patch to a model by its ID.
        /// </summary>
        /// <param name="id">The ID of the model to patch.</param>
        /// <param name="patchModel">The JSON Patch document containing the changes.</param>
        /// <returns>An <see cref="ActionResult"/> containing the patched model.</returns>
        Task<ActionResult<TModel>> PatchAsync(IdType id, [FromBody] JsonPatchDocument<TModel> patchModel);

        /// <summary>
        /// Deletes a model by its ID.
        /// </summary>
        /// <param name="id">The ID of the model to delete.</param>
        /// <returns>An <see cref="ActionResult"/> indicating the result of the delete operation.</returns>
        Task<ActionResult> DeleteAsync(IdType id);
    }
}
#endif
