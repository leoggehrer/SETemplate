//@BaseCode
#if IDINT_ON
global using IdType = System.Int32;
#elif IDLONG_ON
    global using IdType = System.Int64;
#elif IDGUID_ON
    global using IdType = System.Guid;
#else
global using IdType = System.Int32;
#endif
global using Common = SETemplate.Common;
global using CommonModules = SETemplate.Common.Modules;
global using SETemplate.Common.Extensions;
global using CommonStaticLiterals = SETemplate.Common.StaticLiterals;
global using TemplatePath = SETemplate.Common.Modules.Template.TemplatePath;
