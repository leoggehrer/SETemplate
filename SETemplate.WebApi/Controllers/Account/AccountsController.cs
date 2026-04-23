//@BaseCode
#if ACCOUNT_ON
namespace SETemplate.WebApi.Controllers.Account
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.RateLimiting;
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
        [EnableRateLimiting("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<Models.Account.LoginSession>> LoginByAsync([FromBody] Models.Account.LogonModel logonModel)
        {
            var result = await Logic.AccountAccess.LoginAsync(logonModel.Email, logonModel.Password, logonModel.Info ?? string.Empty);
            
            return Ok(result);
        }

        /// <summary>
        /// This method performs a logout using the session token from the Authorization header.
        /// The token must not be passed as a URL segment to avoid leaking it in server logs.
        /// </summary>
        /// <returns>No content on success.</returns>
        [HttpDelete("logout")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult> LogoutAsync()
        {
            var authHeader = HttpContext.Request.Headers.Authorization.ToString();
            var sessionToken = string.Empty;

            if (!string.IsNullOrWhiteSpace(authHeader) && authHeader.StartsWith("SessionToken "))
            {
                sessionToken = authHeader["SessionToken ".Length..].Trim();
            }

            if (!string.IsNullOrWhiteSpace(sessionToken))
            {
                await Logic.AccountAccess.LogoutAsync(sessionToken).ConfigureAwait(false);
            }

            return NoContent();
        }

        // Insert change password method here, as it also requires the session token from the Authorization header.
        /// <summary>
        /// This method changes the password for the current session's user.
        /// The session token must be passed in the Authorization header to identify the user.
        /// </summary>
        /// <param name="currentPassword">The current password of the user.</param>
        /// <param name="newPassword">The new password to set for the user.</param>
        /// <returns>No content on success.</returns>
        [HttpPost("changePassword")]
        [EnableRateLimiting("mutations")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult> ChangePasswordAsync([FromBody] Models.Account.ChangePasswordModel model)
        {
            var authHeader = HttpContext.Request.Headers.Authorization.ToString();
            var sessionToken = string.Empty;

            if (!string.IsNullOrWhiteSpace(authHeader) && authHeader.StartsWith("SessionToken "))
            {
                sessionToken = authHeader["SessionToken ".Length..].Trim();
            }

            if (string.IsNullOrWhiteSpace(sessionToken))
                return BadRequest("Session token is required.");

            await Logic.AccountAccess.ChangePasswordAsync(sessionToken, model.OldPassword, model.NewPassword).ConfigureAwait(false);

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
