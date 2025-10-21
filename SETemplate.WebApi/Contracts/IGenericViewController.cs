//@BaseCode
using Microsoft.AspNetCore.Mvc;

namespace SETemplate.WebApi.Contracts
{
    /// <summary>
    /// Defines a generic controller interface for handling view operations.
    /// </summary>
    /// <typeparam name="TModel">The type of the model object.</typeparam>
    /// <typeparam name="TContract">The type of the contract object.</typeparam>
    public partial interface IGenericViewController<TModel, TContract>
    where TModel : CommonModels.ViewModelObject, TContract, new()
    where TContract : CommonContracts.IViewObject
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
        Task<ActionResult<IEnumerable<TModel>>> QueryAsync([FromBody] Logic.Models.QueryParams queryParams);
    }
}
