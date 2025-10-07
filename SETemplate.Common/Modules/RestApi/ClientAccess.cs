//@BaseCode
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace SETemplate.Common.Modules.RestApi
{
    /// <summary>
    /// This class provides async methods for accessing Rest service endpoints.
    /// </summary>
    /// <remarks>
    /// Creates a new instance of the ClientAccess class.
    /// </remarks>
    /// <param name="baseAddress">The base address for the client.</param>
    /// <param name="sessionToken">The session token for the client.</param>
    public static partial class ClientAccess
    {
        #region static properties
        /// <summary>
        /// Returns the media type for JSON formatting.
        /// </summary>
        /// <returns>The media type string.</returns>
        public static string MediaType => "application/json";
        /// <summary>
        /// Gets the options for deserializing JSON data into objects.
        /// </summary>
        /// <value>
        /// The options for deserializing JSON data into objects.
        /// </value>
        public static JsonSerializerOptions DeserializerOptions => new() { PropertyNameCaseInsensitive = true };
        #endregion static properties
        
        #region static methods
        /// <summary>
        /// Creates a new instance of HttpClient with the specified base address.
        /// </summary>
        /// <param name="baseAddress">The base address of the server.</param>
        /// <returns>A new instance of HttpClient.</returns>
        public static HttpClient CreateClient(string baseAddress)
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
        /// Creates a new instance of HttpClient with the specified base address and session token.
        /// </summary>
        /// <param name="baseAddress">The base address of the HTTP client.</param>
        /// <param name="sessionToken">The session token to be used for authorization.</param>
        /// <returns>A new instance of HttpClient.</returns>
        public static HttpClient CreateClient(string baseAddress, string sessionToken)
        {
            HttpClient client = CreateClient(baseAddress);
            
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Convert.ToBase64String(Encoding.ASCII.GetBytes($"{sessionToken}")));

            return client;
        }
        #endregion static methods
    }
}
