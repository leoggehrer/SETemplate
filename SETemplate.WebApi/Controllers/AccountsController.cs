//@BaseCode
#if ACCOUNT_ON
namespace SETemplate.WebApi.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    /// <summary>
    /// A base class for an MVC controller without view support.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public partial class AccountsController : ControllerBase
    {
        /// <summary>
        /// This method checks the login data email/password and, if correct, returns a logon session.
        /// </summary>
        /// <param name="logonModel">The logon data.</param>
        /// <returns>The logon session object.</returns>
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<Models.Account.LoginSession>> LoginByAsync([FromBody] Models.Account.LogonModel logonModel)
        {
            var result = await Logic.AccountAccess.LoginAsync(logonModel.Email, logonModel.Password, logonModel.Info ?? string.Empty);
            
            return Ok(result);
        }

        /// <summary>
        /// This method performs a logout with the appropriate token.
        /// </summary>
        /// <param name="sessionToken">The session token.</param>
        /// <returns></returns>
        [HttpDelete("{sessionToken}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult> LogoutByAsync(string sessionToken)
        {
            await Logic.AccountAccess.LogoutAsync(sessionToken).ConfigureAwait(false);

            return NoContent();
        }

        /// <summary>
        /// Checks whether the session token is still valid.
        /// </summary>
        /// <param name="sessionToken">The session token to validate.</param>
        /// <returns>True if valid, otherwise false.</returns>
        [HttpPost("issessionalive")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<bool>> IsSessionAliveAsync([FromBody] Models.Account.SessionRequest sessionRequest)
        {
            if (sessionRequest == null || string.IsNullOrWhiteSpace(sessionRequest.SessionToken))
                return BadRequest("Session token is required.");

            var result = await Logic.AccountAccess.IsSessionAliveAsync(sessionRequest.SessionToken);

            return Ok(result);
        }
    }
}
#endif
