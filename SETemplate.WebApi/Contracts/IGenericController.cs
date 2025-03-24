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
        ActionResult Delete(int id);
        ActionResult<IEnumerable<TModel>> Get();
        ActionResult<TModel?> GetById(int id);
        ActionResult<TModel> Patch(int id, [FromBody] JsonPatchDocument<TModel> patchModel);
        ActionResult<TModel> Post([FromBody] TModel model);
        ActionResult<TModel> Put(int id, [FromBody] TModel model);
        ActionResult<IEnumerable<TModel>> Query(string predicate);
    }
}