//@BaseCode
using CommunityToolkit.Mvvm.ComponentModel;
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
    }
}
