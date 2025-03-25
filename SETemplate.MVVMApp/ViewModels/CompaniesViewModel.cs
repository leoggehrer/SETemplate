//@Ignore
#if GENERATEDCODE_ON
using Avalonia.Controls;
using SETemplate.MVVMApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SETemplate.MVVMApp.ViewModels
{
    public class CompaniesViewModel : GenericItemsViewModel<Models.Company>
    {
        protected override GenericItemViewModel<Company> CreateViewModel()
        {
            return new CompanyViewModel();
        }

        protected override Window CreateWindow()
        {
            throw new NotImplementedException();
        }
    }
}
#endif