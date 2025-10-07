//@BaseCode
using Avalonia.Controls;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Input;

namespace SETemplate.MVVMApp.ViewModels
{
    public partial class MessageDialogViewModel : ViewModelBase
    {
        public Window? Parent { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;

        public MessageType Type { get; set; } = MessageType.Info;
        public MessageResult Result { get; set; } = MessageResult.Ok;

        public bool IsInfoType => Type == MessageType.Info;
        public bool IsQuestionType => Type ==   MessageType.Question;
        public bool IsErrorType => Type == MessageType.Error;
        public bool IsInfoOrErrorType => IsInfoType || IsErrorType;

        public ICommand CloseCommand { get; }
        public ICommand YesCommand { get; }
        public ICommand NoCommand { get; }

        public MessageDialogViewModel()
        {
            YesCommand = new RelayCommand(ActivateYesCommand);
            NoCommand = new RelayCommand(ActivateNoCommand);
            CloseCommand = new RelayCommand(Close);
        }

        private void ActivateYesCommand()
        {
            Result = MessageResult.Yes;
            Parent?.Close();
        }
        private void ActivateNoCommand()
        {
            Result = MessageResult.No;
            Parent?.Close();
        }

        private void Close()
        {
            Result = MessageResult.Ok;
            Parent?.Close();
        }
    }
}
