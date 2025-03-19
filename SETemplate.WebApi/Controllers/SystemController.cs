//@BaseCode
using SETemplate.WebApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace SETemplate.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public partial class SystemController : ControllerBase
    {
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult Post([FromBody] UserData model)
        {
            ActionResult? result;

            if (model.UserName.Equals("Admin", StringComparison.CurrentCultureIgnoreCase)
                && model.Password == "passme1234!")
            {
                try
                {
#if DEBUG
                    Logic.DataContext.Factory.InitDatabase();
#endif
                    result = Ok();
                }
                catch (Exception ex)
                {
                    result = BadRequest(ex.Message);
                }
            }
            else
            {
                result = BadRequest();
            }
            return result;
        }
    }
}
