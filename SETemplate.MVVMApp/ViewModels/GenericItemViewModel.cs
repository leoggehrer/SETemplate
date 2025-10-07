//@BaseCode
using CommunityToolkit.Mvvm.Input;
using System;
using System.Windows.Input;

namespace SETemplate.MVVMApp.ViewModels
{
    public abstract partial class GenericItemViewModel<TModel> : ViewModelBase
        where TModel : CommonModels.ModelObject, new()
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
    }
}
