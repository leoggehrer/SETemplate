//@BaseCode
//MdStart
namespace TemplateTools.Logic.Common
{
    [Flags]
    public enum ItemType : ulong
    {
        #region contracts
        ContextContract,
        EntityContract,
        EntitySetContract,
        #endregion contracts

        #region entities
        EntitySet,
        #endregion entities

        #region models
        WebApiModel,
        WebApiEditModel,
        MVVMAppModel,
        MVVMAppEditModel,
        MVVVMAppViewModel,
        #endregion models
        
        #region properties
        Property,
        ModelProperty,
        FilterProperty,
        InterfaceProperty,
        #endregion properties
        
        #region controllers
        Controller,
        ContextAccessor,
        #endregion controllers
        
        #region services
        DbContext,
        Service,
        #endregion services
        
        #region facades and factories
        Facade,
        
        Factory,
        FactoryControllerMethode,
        FactoryFacadeMethode,
        #endregion facades and factories
        
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
        Lambda,
        #endregion general
    }
}
//MdEnd