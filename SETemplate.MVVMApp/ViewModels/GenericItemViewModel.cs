//@BaseCode
using CommunityToolkit.Mvvm.Input;
using System;
using System.Net.Http;
using System.Text.Json;
using System.Text;
using System.Windows.Input;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia;
using SETemplate.MVVMApp.Views;

namespace SETemplate.MVVMApp.ViewModels
{
    public abstract partial class GenericItemViewModel<TModel> : ViewModelBase
        where TModel : Models.ModelObject, new()
    {
        #region fields
        private TModel model = new();
        #endregion fields

        #region properties
        public virtual string RequestUri => $"{typeof(TModel).Name.CreatePluralWord()}";
        public Action? CloseAction { get; set; }
        public TModel Model
        {
            get => model;
            set => model = value ?? new();
        }
        #endregion properties

        #region commands
        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }
        #endregion commands
        public GenericItemViewModel()
        {
            CancelCommand = new RelayCommand(() => Close());
            SaveCommand = new RelayCommand(() => Save());
        }
        protected virtual void Close()
        {
            CloseAction?.Invoke();
        }
        protected virtual async void Save()
        {
            bool canClose = false;
            using var httpClient = new HttpClient { BaseAddress = new Uri(API_BASE_URL) };

            try
            {
                if (Model.Id == 0)
                {
                    var response = httpClient.PostAsync(RequestUri, new StringContent(JsonSerializer.Serialize(Model), Encoding.UTF8, "application/json")).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        canClose = true;
                    }
                    else
                    {
                        var messageDialog = new MessageDialog("Fehler", "Beim Speichern ist ein Fehler aufgetreten!", MessageType.Error);
                        var mainWindow = (Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow;

                        await messageDialog.ShowDialog(mainWindow!);
                        Console.WriteLine($"Fehler beim Abrufen von {RequestUri}. Status: {response.StatusCode}");
                    }
                }
                else
                {
                    var response = httpClient.PutAsync($"{RequestUri}/{Model.Id}", new StringContent(JsonSerializer.Serialize(Model), Encoding.UTF8, "application/json")).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        canClose = true;
                    }
                    else
                    {
                        Console.WriteLine($"Fehler beim Abrufen von {RequestUri}. Status: {response.StatusCode}");
                    }
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
        }
    }
}
