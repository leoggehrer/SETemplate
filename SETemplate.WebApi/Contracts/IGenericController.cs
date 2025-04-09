//@BaseCode
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace SETemplate.WebApi.Contracts
{
    public interface IGenericController<TModel, TContract>
        where TModel : CommonModels.ModelObject, TContract, new()
        where TContract : CommonContracts.IIdentifiable
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
