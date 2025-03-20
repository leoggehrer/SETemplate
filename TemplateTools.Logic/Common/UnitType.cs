//@BaseCode
//MdStart
namespace TemplateTools.Logic.Common
{
    [Flags]
    public enum UnitType : long
    {
        All,
        General,
        
        CommonBase,
        Logic,
        WebApi,
        
        AspMvc,
        AngularApp,
        MVVMApp,
        ClientBlazorApp,
        ConApp,
        
        TemplateCodeGenerator,
        TemplateTool,
    }
}
//MdEnd