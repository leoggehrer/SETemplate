//@BaseCode
//MdStart
namespace TemplateTools.Logic.Common
{
    [Flags]
    public enum ItemType : ulong
    {
        #region contracts
        AccessContract,         // controller access contract
        ServiceAccessContract,  // service access contract
        ServiceContract,        // service access contract
        ModelContract,          // model access contract
        #endregion contracts
        
        #region models
        AccessModel,
        ServiceModel,
        EditModel,
        AccessFilterModel,
        ServiceFilterModel,
        #endregion models
        
        #region properties
        Property,
        ModelProperty,
        FilterProperty,
        InterfaceProperty,
        ServiceModelProperty,
        #endregion properties
        
        #region controllers
        Controller,
        AccessController,
        ServiceController,
        #endregion controllers
        
        #region services
        DbContext,
        Service,
        AccessService,
        AddServices,
        #endregion services
        
        #region facades and factories
        Facade,
        
        Factory,
        FactoryControllerMethode,
        FactoryFacadeMethode,
        #endregion facades and factories
        
        #region views
        View,
        ViewProperty,
        ViewDisplayProperty,
        ViewEditProperty,
        ViewFilterProperty,
        ListView,
        ListViewProperty,
        PageList,
        PageListProperty,
        PageEditProperty,
        PageDetailsProperty,

        DetailsComponent,
        DetailsProperty,
        DetailsDialog,
        DetailsPage,
        EditComponent,
        EditProperty,
        EditDialog,
        EditPage,
        #endregion views
        
        #region angular
        TypeScriptEnum,
        TypeScriptModel,
        TypeScriptService,
        #endregion angular
        
        #region diagram
        EntityClassDiagram,
        #endregion diagram
        
        #region general
        Attribute,
        AllItems,
        File,
        Lambda,
        #endregion general
    }
}
//MdEnd