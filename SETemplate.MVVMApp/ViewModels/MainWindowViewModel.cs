using Avalonia.Controls.ApplicationLifetimes;
using Avalonia;
using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;

namespace SETemplate.MVVMApp.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        public string Greeting { get; } = "Welcome to Avalonia!";

        [RelayCommand]
        public static async Task Logon()
        {
            var window = new Views.LogonWindow();
            var mainWindow = (Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow;

            await window.ShowDialog(mainWindow!);
        }
    }
}
