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
    /// <summary>
    /// A generic ViewModel for managing a collection of items of type <typeparamref name="TModel"/>.
    /// Provides functionality for filtering, adding, editing, and loading items.
    /// </summary>
    /// <typeparam name="TModel">The type of the model object.</typeparam>
    public abstract partial class GenericItemsViewModel<TModel> : ViewModelBase
        where TModel : CommonModels.ModelObject, new()
    {
        #region fields
        /// <summary>
        /// The current filter string used to filter the items.
        /// </summary>
        private string _filter = string.Empty;

        /// <summary>
        /// The currently selected item in the collection.
        /// </summary>
        private TModel? selectedItem;

        /// <summary>
        /// The complete list of models loaded from the data source.
        /// </summary>
        private readonly List<TModel> _models = [];

        /// <summary>
        /// The filtered list of models based on the current filter.
        /// </summary>
        private readonly List<TModel> _filteredModels = [];
        #endregion fields

        #region properties
        /// <summary>
        /// Gets the API request URI for the current model type.
        /// </summary>
        public virtual string RequestUri => $"{typeof(TModel).Name.CreatePluralWord()}";

        /// <summary>
        /// Gets or sets the filter string used to filter the items.
        /// Updates the filtered list when set.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the currently selected item in the collection.
        /// </summary>
        public virtual TModel? SelectedItem
        {
            get => selectedItem;
            set
            {
                selectedItem = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets the filtered collection of models as an observable collection.
        /// </summary>
        public virtual ObservableCollection<TModel> Models
        {
            get
            {
                return new ObservableCollection<TModel>(_filteredModels);
            }
        }
        #endregion properties

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericItemsViewModel{TModel}"/> class.
        /// Automatically loads the models asynchronously.
        /// </summary>
        public GenericItemsViewModel()
        {
            _ = LoadModelsAsync();
        }

        /// <summary>
        /// Creates a new window for adding or editing an item.
        /// Must be implemented by derived classes.
        /// </summary>
        /// <returns>A new instance of a <see cref="Window"/>.</returns>
        protected virtual Window CreateWindow()
        {
            throw new NotImplementedException("CreateWindow must be implemented in derived classes.");
        }

        /// <summary>
        /// Creates a new ViewModel for adding or editing an item.
        /// Must be implemented by derived classes.
        /// </summary>
        /// <returns>A new instance of <see cref="GenericItemViewModel{TModel}"/>.</returns>
        protected virtual GenericItemViewModel<TModel> CreateViewModel()
        {
            throw new NotImplementedException("CreateViewModel must be implemented in derived classes.");
        }

        #region commands
        /// <summary>
        /// Command to load the models asynchronously.
        /// </summary>
        [RelayCommand]
        public virtual async Task LoadModels()
        {
            await LoadModelsAsync();
        }

        /// <summary>
        /// Command to add a new item. Opens a dialog window for item creation.
        /// </summary>
        [RelayCommand]
        public virtual async Task AddItem()
        {
            var viewModelWindow = CreateWindow();
            var viewModel = CreateViewModel();

            viewModel.CloseAction = viewModelWindow.Close;
            viewModelWindow.DataContext = viewModel;

            // Set the current main window as the parent
            var mainWindow = (Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow;
            if (mainWindow != null)
            {
                await viewModelWindow.ShowDialog(mainWindow);
                _ = LoadModelsAsync();
            }
        }

        /// <summary>
        /// Command to edit an existing item. Opens a dialog window for item editing.
        /// </summary>
        /// <param name="model">The model to be edited.</param>
        [RelayCommand]
        public virtual async Task EditItem(TModel model)
        {
            var viewModelWindow = CreateWindow();
            var viewModel = CreateViewModel();

            viewModel.CloseAction = viewModelWindow.Close;
            viewModelWindow.DataContext = viewModel;
            viewModel.Model = model;

            // Set the current main window as the parent
            var mainWindow = (Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow;

            if (mainWindow != null)
            {
                await viewModelWindow.ShowDialog(mainWindow);
                _ = LoadModelsAsync();
            }
        }
        #endregion commands

        /// <summary>
        /// Loads the models asynchronously from the data source.
        /// Applies the current filter after loading.
        /// </summary>
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
