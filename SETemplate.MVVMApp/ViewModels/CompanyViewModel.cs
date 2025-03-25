//@Ignore
#if GENERATEDCODE_ON

namespace SETemplate.MVVMApp.ViewModels
{
    public class CompanyViewModel : GenericItemViewModel<Models.Company>
    {
        public string Name
        {
            get { return Model.Name; }
            set
            {
                Model.Name = value;
                OnPropertyChanged();
            }
        }

        public string? Address
        {
            get { return Model.Address; }
            set { Model.Address = value; }
        }

        public string? Description
        {
            get { return Model.Description; }
            set { Model.Description = value; }
        }

    }
}
#endif