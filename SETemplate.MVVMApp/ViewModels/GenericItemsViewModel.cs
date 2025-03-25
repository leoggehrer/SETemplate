using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.Input;
using SETemplate.MVVMApp.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace SETemplate.MVVMApp.ViewModels
{
    public abstract partial class GenericItemsViewModel<TModel> : ViewModelBase
        where TModel : Models.ModelObject, new()
    {
        #region fields
        private string _filter = string.Empty;
        private TModel? selectedItem;
        private readonly List<TModel> _models = [];
        #endregion fields

        #region properties
        public virtual string RequestUri => $"{typeof(TModel).Name.CreatePluralWord()}";
        public virtual string Filter
        {
            get
            {
                return _filter;
            }
            set
            {
                _filter = value;
                ApplyFilter(value);
                OnPropertyChanged();
            }
        }
        public virtual TModel? SelectedItem
        {
            get => selectedItem;
            set
            {
                selectedItem = value;
                OnPropertyChanged();
            }
        }
        public virtual ObservableCollection<TModel> Models
        {
            get
            {
                var result = new ObservableCollection<TModel>();

                foreach (var model in _models)
                {
                    result.Add(model);
                }
                return result;
            }
        }
        #endregion properties

        public GenericItemsViewModel()
        {
            _ = LoadModelsAsync();
        }

        protected abstract Window CreateWindow();
        protected abstract GenericItemViewModel<TModel> CreateViewModel();

        #region commands
        [RelayCommand]
        public virtual async Task LoadModels()
        {
            await LoadModelsAsync();
        }
        [RelayCommand]
        public virtual async Task AddItem()
        {
            var viewModelWindow = CreateWindow();
            var viewModel = CreateViewModel();
            
            viewModel.CloseAction = viewModelWindow.Close;
            viewModelWindow.DataContext = viewModel;
            // Aktuelles Hauptfenster als Parent setzen
            var mainWindow = (Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow;
            if (mainWindow != null)
            {
                await viewModelWindow.ShowDialog(mainWindow);
                _ = LoadModelsAsync();
            }
        }
        [RelayCommand]
        public virtual async Task EditItem(TModel model)
        {
            var viewModelWindow = CreateWindow();
            var viewModel = CreateViewModel();

            viewModel.CloseAction = viewModelWindow.Close;
            viewModelWindow.DataContext = viewModel;
            // Aktuelles Hauptfenster als Parent setzen
            var mainWindow = (Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow;

            if (mainWindow != null)
            {
                await viewModelWindow.ShowDialog(mainWindow);
                _ = LoadModelsAsync();
            }
        }
        [RelayCommand]
        public virtual async Task DeleteItem(TModel model)
        {
            var messageDialog = new MessageDialog("Delete", $"Do you want to delete the '{model}'?", MessageType.Question);
            var mainWindow = (Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow;

            // Aktuelles Hauptfenster als Parent setzen
            await messageDialog.ShowDialog(mainWindow!);

            if (messageDialog.Result == MessageResult.Yes)
            {
                using var httpClient = new HttpClient { BaseAddress = new Uri(API_BASE_URL) };


                var response = await httpClient.DeleteAsync($"{RequestUri}/{model.Id}");

                if (response.IsSuccessStatusCode == false)
                {
                    messageDialog = new MessageDialog("Error", "An error occurred during deletion!", MessageType.Error);
                    await messageDialog.ShowDialog(mainWindow!);
                }
                else
                {
                    _ = LoadModelsAsync();
                }
            }
        }
        #endregion commands

        protected virtual async void ApplyFilter(string filter)
        {
            // UI-Update sicherstellen
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                var selectedItem = SelectedItem;

                Models.Clear();
                foreach (var model in _models)
                {
                    if (model != default && model.ToString()!.Contains(filter, StringComparison.OrdinalIgnoreCase))
                    {
                        Models.Add(model);
                    }
                }
                OnPropertyChanged(nameof(Models));
                if (selectedItem != null)
                {
                    SelectedItem = Models.FirstOrDefault(e => e.Id == selectedItem.Id);
                }
            });
        }
        protected virtual async Task LoadModelsAsync()
        {
            try
            {
                using var httpClient = new HttpClient { BaseAddress = new Uri(API_BASE_URL) };
                var response = await httpClient.GetStringAsync(RequestUri);
                var models = JsonSerializer.Deserialize<List<TModel>>(response, _jsonSerializerOptions);

                if (models != null)
                {
                    _models.Clear();
                    foreach (var model in models)
                    {
                        _models.Add(model);
                    }
                    ApplyFilter(Filter);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading models: {ex.Message}");
            }
        }
    }
}
