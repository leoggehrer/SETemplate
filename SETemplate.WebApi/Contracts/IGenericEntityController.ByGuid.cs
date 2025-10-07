//@BaseCode
#if EXTERNALGUID_ON
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace SETemplate.WebApi.Contracts
{
    /// <summary>
    /// Defines a generic controller interface for handling operations on entities identified by a GUID.
    /// </summary>
    /// <typeparam name="TModel">The type of the model entity.</typeparam>
    /// <typeparam name="TContract">The type of the contract associated with the model.</typeparam>
    partial interface IGenericEntityController<TModel, TContract>
    {
        /// <summary>
        /// Retrieves an entity by its GUID.
        /// </summary>
        /// <param name="guid">The GUID of the entity to retrieve.</param>
        /// <returns>An <see cref="ActionResult"/> containing the entity if found, or null if not found.</returns>
        Task<ActionResult<TModel?>> GetByIdAsync(Guid guid);

        /// <summary>
        /// Updates an entity with the specified GUID.
        /// </summary>
        /// <param name="guid">The GUID of the entity to update.</param>
        /// <param name="model">The updated model data.</param>
        /// <returns>An <see cref="ActionResult"/> containing the updated entity.</returns>
        Task<ActionResult<TModel>> PutAsync(Guid guid, [FromBody] TModel model);

        /// <summary>
        /// Applies a JSON Patch to an entity with the specified GUID.
        /// </summary>
        /// <param name="guid">The GUID of the entity to patch.</param>
        /// <param name="patchModel">The JSON Patch document containing the changes.</param>
        /// <returns>An <see cref="ActionResult"/> containing the patched entity.</returns>
        Task<ActionResult<TModel>> PatchAsync(Guid guid, [FromBody] JsonPatchDocument<TModel> patchModel);

        /// <summary>
        /// Deletes an entity with the specified GUID.
        /// </summary>
        /// <param name="guid">The GUID of the entity to delete.</param>
        /// <returns>An <see cref="ActionResult"/> indicating the result of the operation.</returns>
        Task<ActionResult> DeleteAsync(Guid guid);
    }
}
#endif
