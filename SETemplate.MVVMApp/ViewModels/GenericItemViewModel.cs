//@BaseCode
using CommunityToolkit.Mvvm.Input;
using System;
using System.Windows.Input;

namespace SETemplate.MVVMApp.ViewModels
{
    public abstract partial class GenericItemViewModel<TDataModel> : ViewModelBase
        where TDataModel : CommonModels.ModelObject, new()
    {
        #region fields
        private TDataModel dataModel = new();
        #endregion fields

        #region properties
        public virtual string RequestUri => $"{typeof(TDataModel).Name.CreatePluralWord()}";
        public Action? CloseAction { get; set; }
        public TDataModel DataModel
        {
            get => dataModel;
            set => dataModel = value ?? new();
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
    }
}
