//@BaseCode
using Microsoft.AspNetCore.Mvc;

namespace SETemplate.WebApi.Contracts
{
    /// <summary>
    /// Defines a generic controller interface for handling CRUD operations.
    /// </summary>
    /// <typeparam name="TModel">The type of the model object.</typeparam>
    /// <typeparam name="TContract">The type of the contract object.</typeparam>
    public partial interface IGenericEntityController<TModel, TContract>
        where TModel : CommonModels.ModelObject, TContract, new()
        where TContract : CommonContracts.IIdentifiable
    {
        /// <summary>
        /// Retrieves the count of all entities.
        /// </summary>
        /// <returns>An <see cref="ActionResult"/> representing the count of entities.</returns>
        Task<ActionResult> CountAsync();

        /// <summary>
        /// Retrieves all entities.
        /// </summary>
        /// <returns>An <see cref="ActionResult"/> containing an enumerable of <typeparamref name="TModel"/>.</returns>
        Task<ActionResult<IEnumerable<TModel>>> GetAsync();

        /// <summary>
        /// Queries entities based on the provided parameters.
        /// </summary>
        /// <param name="queryParams">The query parameters containing predicates and values.</param>
        /// <returns>An <see cref="ActionResult"/> containing an enumerable of <typeparamref name="TModel"/> that match the query.</returns>
        Task<ActionResult<IEnumerable<TModel>>> QueryAsync([FromBody] Models.QueryParams queryParams);

        /// <summary>
        /// Creates a new entity.
        /// </summary>
        /// <param name="model">The model object to create.</param>
        /// <returns>An <see cref="ActionResult"/> containing the created <typeparamref name="TModel"/>.</returns>
        Task<ActionResult<TModel>> PostAsync([FromBody] TModel model);
    }
}
