//@BaseCode
#if EXTERNALGUID_ON
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.Input;
using SETemplate.MVVMApp.Views;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SETemplate.MVVMApp.ViewModels
{
    partial class GenericItemsViewModel<TModel>
    {
        /// <summary>
        /// Deletes the specified item after user confirmation.
        /// </summary>
        /// <param name="model">The model to be deleted.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        [RelayCommand]
        public virtual async Task DeleteItem(TModel model)
        {
            var messageDialog = new MessageDialog("Delete", $"Do you want to delete the '{model}'?", MessageType.Question);
            var mainWindow = (Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow;

            // Set the current main window as the parent
            await messageDialog.ShowDialog(mainWindow!);

            if (messageDialog.Result == MessageResult.Yes)
            {
                using var httpClient = CreateHttpClient();

                var response = await httpClient.DeleteAsync($"{RequestUri}/{model.Guid}");

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

        /// <summary>
        /// Applies a filter to the models and updates the UI accordingly.
        /// </summary>
        /// <param name="filter">The filter string to apply.</param>
        protected virtual async void ApplyFilter(string filter)
        {
            // Ensure UI update
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                var selectedItem = SelectedItem;

                _filteredModels.Clear();
                foreach (var model in _models)
                {
                    if (model != default && model.ToString()!.Contains(filter, StringComparison.OrdinalIgnoreCase))
                    {
                        _filteredModels.Add(model);
                    }
                }
                OnPropertyChanged(nameof(Models));
                if (selectedItem != null)
                {
                    SelectedItem = _filteredModels.FirstOrDefault(e => e.Guid == selectedItem.Guid);
                }
            });
        }
    }
}
#endif
