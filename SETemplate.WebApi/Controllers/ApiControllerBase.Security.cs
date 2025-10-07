//@BaseCode
#if ACCOUNT_ON

namespace SETemplate.WebApi.Controllers
{
    partial class ApiControllerBase
    {
        #region fields
        private string? _sessionToken;
        #endregion fields

        #region properties
        protected string SessionToken
        {
            get => _sessionToken ??= GetSessionToken();
            set => _sessionToken = value;
        }
        #endregion properties

        #region methods
        /// <summary>
        /// Retrieves the session token from the header.
        /// </summary>
        /// <returns>The session token.</returns>
        protected string GetSessionToken()
        {
            var authHeader = HttpContext.Request.Headers.Authorization;

            return Task.Run(async () => await GetSessionTokenAsync(authHeader!)).Result;
        }
        /// <summary>
        /// Retrieves the session token from the header.
        /// </summary>
        /// <returns>The session token.</returns>
        protected Task<string> GetSessionTokenAsync()
        {
            var authHeader = HttpContext.Request.Headers.Authorization;

            return GetSessionTokenAsync(authHeader!);
        }
        /// <summary>
        /// Retrieves the session token from the header.
        /// </summary>
        /// <param name="authHeader">The authorization header.</param>
        /// <returns>The session token.</returns>
        protected static async Task<string> GetSessionTokenAsync(string authHeader)
        {
            string result = string.Empty;

            if (authHeader.HasContent())
            {
                if (authHeader.StartsWith("Bearer"))
                {
                    var encoding = System.Text.Encoding.GetEncoding("iso-8859-1");
                    var encodedToken = authHeader["Bearer ".Length..].Trim();

                    result = encoding.GetString(Convert.FromBase64String(encodedToken));
                }
                else if (authHeader.StartsWith("Basic"))
                {
                    var encodedUseridPassword = authHeader["Basic ".Length..].Trim();
                    System.Text.Encoding encoding = System.Text.Encoding.GetEncoding("iso-8859-1");
                    var useridPassword = encoding.GetString(Convert.FromBase64String(encodedUseridPassword));

                    var seperatorIndex = useridPassword.IndexOf(':');
                    var userid = useridPassword[..seperatorIndex];
                    var password = useridPassword[(seperatorIndex + 1)..];
                    var login = await Logic.AccountAccess.LoginAsync(userid, password, string.Empty).ConfigureAwait(false);

                    result = login.SessionToken;
                }
                else if (authHeader.StartsWith("SessionToken"))
                {
                    result = authHeader["SessionToken ".Length..].Trim();
                }
                else
                {
                    result = authHeader;
                }
            }
            return result;
        }
        #endregion methods
    }
}
#endif
