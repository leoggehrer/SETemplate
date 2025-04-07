//@BaseCode
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Net.Http;
using System.Text.Json;

namespace SETemplate.MVVMApp.ViewModels
{
    public class ViewModelBase : ObservableObject
    {
        #region fields
        protected static readonly string API_BASE_URL = "https://localhost:7074/api/";
        protected static readonly Common.Modules.Configuration.AppSettings _appSettings = Common.Modules.Configuration.AppSettings.Instance;
        protected static readonly JsonSerializerOptions _jsonSerializerOptions = new() { PropertyNameCaseInsensitive = true };
        #endregion fields

        static ViewModelBase()
        {
            API_BASE_URL = _appSettings["Server:BASE_URL"] ?? API_BASE_URL;
        }

        protected static HttpClient CreateHttpClient()
        {
#if ACCOUNT_ON && GENERATEDCODE_ON

            HttpClient result;

            if (LogonViewModel.LogonSession?.SessionToken != null)
            {
                result = CommonModules.RestApi.ClientAccess.CreateClient(API_BASE_URL, LogonViewModel.LogonSession?.SessionToken ?? string.Empty);
            }
            else
            {
                result = CommonModules.RestApi.ClientAccess.CreateClient(API_BASE_URL);
            }
            return result;
#else
            var httpClient = new HttpClient
            {
                BaseAddress = new Uri(API_BASE_URL)
            };
            return httpClient;
#endif
        }
    }
}
