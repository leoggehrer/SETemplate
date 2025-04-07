//@BaseCode
#if ACCOUNT_ON
using SETemplate.Common.Modules.Exceptions;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace SETemplate.Common.Modules.RestApi
{
    /// <summary>
    /// Provides access to login-related operations.
    /// </summary>
    public partial class LoginAccess
    {
        #region static properties
        /// <summary>
        /// Gets the media type of the property.
        /// </summary>
        /// <value>The media type.</value>
        protected static string MediaType => "application/json";
        /// <summary>
        /// Gets or sets the options used for deserializing JSON data.
        /// </summary>
        /// <value>
        /// The options used for deserializing JSON data.
        /// </value>
        protected static JsonSerializerOptions DeserializerOptions => new() { PropertyNameCaseInsensitive = true };
        #endregion static properties
        
        #region static methods
        /// <summary>
        /// Creates a new instance of the <see cref="HttpClient"/> class with the specified base address.
        /// </summary>
        /// <param name="baseAddress">The base address for the HTTP client.</param>
        /// <returns>A new instance of the <see cref="HttpClient"/> class.</returns>
        protected static HttpClient CreateClient(string baseAddress)
        {
            HttpClient client = new();
            
            if (baseAddress.HasContent())
            {
                if (baseAddress.EndsWith('/') == false
                && baseAddress.EndsWith('\\') == false)
                {
                    baseAddress += "/";
                }
                
                client.BaseAddress = new Uri(baseAddress);
            }
            client.DefaultRequestHeaders.Accept.Clear();
            
            // Add an Accept header for JSON format.
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(MediaType));
            return client;
        }
        /// <summary>
        /// Asynchronously logs in a user to the specified base address using the provided email, password, and information.
        /// </summary>
        /// <typeparam name="T">The type of data to be returned.</typeparam>
        /// <param name="baseAddress">The base address of the server.</param>
        /// <param name="email">The email of the user.</param>
        /// <param name="password">The password of the user.</param>
        /// <param name="info">Additional information about the logon request.</param>
        /// <returns>An awaitable task that returns an object of type T, or null if not successful.</returns>
        /// <exception cref="RestApiException">Thrown when the request is not successful.</exception>
        public static async Task<T?> LogonAsync<T>(string baseAddress, string email, string password, string info)
        {
            using var client = CreateClient(baseAddress);
            var model = new Models.Accounts.Logon { Email = email, Password = password, Info = info };
            var jsonData = JsonSerializer.Serialize(model);
            using var content = new StringContent(jsonData, Encoding.UTF8, MediaType);
            var response = await client.PostAsync($"Accounts/Logon", content).ConfigureAwait(false);
            
            if (response.IsSuccessStatusCode)
            {
                var contentData = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
                var result = await JsonSerializer.DeserializeAsync<T>(contentData, DeserializerOptions).ConfigureAwait(false);
                
                return result;
            }
            else
            {
                var stringData = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                var errorMessage = $"{response.ReasonPhrase}: {stringData}";
                
                System.Diagnostics.Debug.WriteLine("{0} ({1})", (int)response.StatusCode, errorMessage);
                throw new RestApiException(errorMessage);
            }
        }
        /// <summary>
        /// Logs the user out asynchronously.
        /// </summary>
        /// <param name="baseAddress">The base address of the API.</param>
        /// <param name="sessionToken">The session token to be included in the request.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        /// <exception cref="RestApiException">Thrown when the API request fails.</exception>
        public static async Task LogoutAsync(string baseAddress, string sessionToken)
        {
            using var client = CreateClient(baseAddress);
            var jsonData = JsonSerializer.Serialize(sessionToken);
            using var content = new StringContent(jsonData, Encoding.UTF8, MediaType);
            var response = await client.PutAsync($"Accounts/Logout", content).ConfigureAwait(false);
            
            if (response.IsSuccessStatusCode == false)
            {
                var stringData = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                var errorMessage = $"{response.ReasonPhrase}: {stringData}";
                
                System.Diagnostics.Debug.WriteLine("{0} ({1})", (int)response.StatusCode, errorMessage);
                throw new RestApiException(errorMessage);
            }
        }
        /// <summary>
        /// Checks if a session is alive.
        /// </summary>
        /// <param name="baseAddress">The base address of the API.</param>
        /// <param name="sessionToken">The session token to check.</param>
        /// <returns>True if the session is alive, false otherwise.</returns>
        public static async Task<bool> IsSessionAlive(string baseAddress, string sessionToken)
        {
            bool result;
            using var client = CreateClient(baseAddress);
            var response = await client.GetAsync($"Accounts/IsSessionAlive/{sessionToken}").ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var contentData = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                result = Convert.ToBoolean(contentData);
            }
            else
            {
                var stringData = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                var errorMessage = $"{response.ReasonPhrase}: {stringData}";

                System.Diagnostics.Debug.WriteLine("{0} ({1})", (int)response.StatusCode, errorMessage);
                throw new RestApiException(errorMessage);
            }
            return result;
        }
        #endregion static methods
    }
}
#endif
