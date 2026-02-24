//@BaseCode
using CommunityToolkit.Mvvm.Input;
using System;
using System.Net.Http;
using System.Text.Json;
using System.Text;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia;
using SETemplate.MVVMApp.Views;
using System.Threading.Tasks;

namespace SETemplate.MVVMApp.ViewModels
{
    public partial class LogonViewModel : ViewModelBase
    {
        #region fields
        private string _email = string.Empty;
        private string _password = string.Empty;
        #endregion fields

        #region properties
        public static Models.Account.LogonSession? LogonSession { get; private set; }
        public Action? CloseAction { get; set; }

        public string Email
        {
            get { return _email; }
            set
            {
                _email = value;
                OnPropertyChanged();
            }
        }

        public string Password
        {
            get { return _password; }
            set
            {
                _password = value;
                OnPropertyChanged();
            }
        }
        #endregion properties

        [RelayCommand]
        public virtual void Cancel()
        {
            CloseAction?.Invoke();
        }
        [RelayCommand]
        public virtual async Task Logon()
        {
#if ACCOUNT_ON && GENERATEDCODE_ON
            bool canClose = false;
            try
            {
                var requestUri = "Accounts/Logon";
                using var httpClient = CreateHttpClient();
                var jsonModel = new StringContent(JsonSerializer.Serialize(new { Email, Password }), Encoding.UTF8, "application/json");
                var response = httpClient.PostAsync(requestUri, jsonModel).Result;

                if (response.IsSuccessStatusCode)
                {
                    canClose = true;
                    LogonSession = JsonSerializer.Deserialize<Models.Account.LogonSession>(response.Content.ReadAsStringAsync().Result, _jsonSerializerOptions);
                }
                else
                {
                    var messageDialog = new MessageDialog("Fehler beim Anmelden", "Benutzername und Kennwort stimmen nicht überein!", MessageType.Error);
                    var mainWindow = (Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow;

                    await messageDialog.ShowDialog(mainWindow!);
                    System.Diagnostics.Debug.WriteLine($"Fehler beim Abrufen von {requestUri}. Status: {response.StatusCode}");
                }
            }
            catch (Exception)
            {

                throw;
            }

            if (canClose)
            {
                CloseAction?.Invoke();
            }
#else 
            await Task.Delay(1000);
#endif
        }
    }
}
