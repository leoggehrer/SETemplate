//@BaseCode
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using SETemplate.Common.Contracts;
using SETemplate.WebApi.Models;

namespace SETemplate.WebApi.Contracts
{
    public interface IGenericController<TModel, TContract>
        where TModel : ModelObject, TContract, new()
        where TContract : IIdentifiable
    {
        ActionResult<IEnumerable<TModel>> Query(string predicate);
        ActionResult<IEnumerable<TModel>> Get();
        ActionResult<TModel?> GetById(IdType id);
        ActionResult<TModel> Post([FromBody] TModel model);
        ActionResult<TModel> Put(IdType id, [FromBody] TModel model);
        ActionResult<TModel> Patch(IdType id, [FromBody] JsonPatchDocument<TModel> patchModel);
        ActionResult Delete(IdType id);
    }
}
