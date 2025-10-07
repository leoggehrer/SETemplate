//@BaseCode
using System.Reflection;

namespace SETemplate.Logic.Modules.CodeGenerator
{
    /// <summary>
    /// Provides access to assembly and type information.
    /// </summary>
    public static class AssemblyAccess
    {
        /// <summary>
        /// Gets an array of all types in the executing assembly.
        /// </summary>
        /// <returns>An array of Type objects representing all types in the executing assembly.</returns>
        public static Type[] AllTypes
        {
            get
            {
                var allTypes = AppDomain.CurrentDomain
                                        .GetAssemblies()
                                        .Where(a => a.FullName!.StartsWith($"{nameof(SETemplate)}{Common.StaticLiterals.CommonExtension}")
                                                 || a.FullName!.StartsWith($"{nameof(SETemplate)}{Common.StaticLiterals.LogicExtension}"))
                                        .SelectMany(assembly =>
                                        {
                                            try
                                            {
                                                return assembly.GetTypes();
                                            }
                                            catch (ReflectionTypeLoadException ex)
                                            {
                                                return ex.Types.Where(t => t != null);
                                            }
                                        });

                return allTypes!.ToArray()!;
                //var assembly = Assembly.GetExecutingAssembly();

                //return assembly.GetTypes();
            }
        }
        /// <summary>
        /// Gets an array of Entity Types.
        /// </summary>
        public static Type[] EntityTypes => [.. AllTypes.Where(t => t.IsClass
                                                                 && t.IsAbstract == false
                                                                 && t.IsNested == false
                                                                 && string.IsNullOrEmpty(t.FullName) == false 
                                                                 && t.FullName.Contains(".Entities."))];

    }
}

