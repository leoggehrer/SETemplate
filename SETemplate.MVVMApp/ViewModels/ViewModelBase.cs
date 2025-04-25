//@BaseCode
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Net.Http;
using System.Text.Json;

namespace SETemplate.MVVMApp.ViewModels
{
    /// <summary>
    /// Base class for all ViewModels in the application, providing common functionality and properties.
    /// </summary>
    public class ViewModelBase : ObservableObject
    {
        #region fields

        /// <summary>
        /// The base URL for the API endpoints.
        /// </summary>
        protected static readonly string API_BASE_URL = "https://localhost:7074/api/";

        /// <summary>
        /// Singleton instance of the application settings.
        /// </summary>
        protected static readonly Common.Modules.Configuration.AppSettings _appSettings = Common.Modules.Configuration.AppSettings.Instance;

        /// <summary>
        /// JSON serializer options with case-insensitive property name handling.
        /// </summary>
        protected static readonly JsonSerializerOptions _jsonSerializerOptions = new() { PropertyNameCaseInsensitive = true };

        #endregion fields

        /// <summary>
        /// Static constructor to initialize static fields and configurations.
        /// </summary>
        static ViewModelBase()
        {
            API_BASE_URL = _appSettings["Server:BASE_URL"] ?? API_BASE_URL;
        }

        /// <summary>
        /// Creates and configures an instance of <see cref="HttpClient"/> for making API requests.
        /// </summary>
        /// <returns>A configured <see cref="HttpClient"/> instance.</returns>
        protected static HttpClient CreateHttpClient()
        {
#if ACCOUNT_ON && GENERATEDCODE_ON

                HttpClient result;

                if (LogonViewModel.LogonSession?.SessionToken != null)
                {
                    // Create an HttpClient with session token for authentication.
                    result = CommonModules.RestApi.ClientAccess.CreateClient(API_BASE_URL, LogonViewModel.LogonSession?.SessionToken ?? string.Empty);
                }
                else
                {
                    // Create an HttpClient without session token.
                    result = CommonModules.RestApi.ClientAccess.CreateClient(API_BASE_URL);
                }
                return result;
#else
            // Create a default HttpClient with the base API URL.
            var httpClient = new HttpClient
            {
                BaseAddress = new Uri(API_BASE_URL)
            };
            return httpClient;
#endif
        }
    }
}
