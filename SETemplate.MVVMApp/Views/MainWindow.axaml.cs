using Avalonia.Controls;
using System;

namespace SETemplate.MVVMApp.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    protected override async void OnOpened(EventArgs e)
    {
        base.OnOpened(e);
        await System.Threading.Tasks.Task.Delay(1000); // Simulate loading time
#if ACCOUNT_ON
        if (ViewModels.LogonViewModel.LogonSession?.SessionToken == default)
        {
            var logonWindow = new LogonWindow();

            await logonWindow.ShowDialog(this);
        }

        if ( ViewModels.LogonViewModel.LogonSession?.SessionToken == default)
        {
            Close();
        }
#endif
    }
}
