//@BaseCode
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.Json;
using System.Threading.Tasks;

namespace SETemplate.MVVMApp.ViewModels
{
    public abstract partial class GenericItemsViewModel<TModel> : ViewModelBase
        where TModel : CommonModels.ModelObject, new()
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
        #endregion commands

        protected virtual async Task LoadModelsAsync()
        {
            try
            {
                using var httpClient = CreateHttpClient();
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
