//@BaseCode
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace SETemplate.MVVMApp.Views
{
    public partial class MessageDialog : Window
    {
        public MessageDialog()
        {
            InitializeComponent();
        }
        public MessageDialog(string title, string message, ViewModels.MessageType type)
        {
            InitializeComponent();
            DataContext = new ViewModels.MessageDialogViewModel { Title = title, Message = message, Parent = this, Type = type };
        }

        public ViewModels.MessageResult Result => (DataContext as ViewModels.MessageDialogViewModel)?.Result ?? ViewModels.MessageResult.Ok;
        public async void ShowMessage(Window parent)
        {
            await ShowDialog(parent);
        }
    }
}

