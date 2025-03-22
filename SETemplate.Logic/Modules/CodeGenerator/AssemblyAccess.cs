//@BaseCode
//MdStart
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
                var assembly = Assembly.GetExecutingAssembly();
                
                return assembly.GetTypes();
            }
        }
        /// <summary>
        /// Gets an array of Entity Types.
        /// </summary>
        /// <remarks>
        /// This property filters the <see cref="AllTypes"/> collection and returns only
        /// the types that have a non-null <see cref="Type.FullName"/> and contain the
        /// string ".Entities." in their full name.
        /// </remarks>
        /// <value>
        /// An array of Type objects representing entity types.
        /// </value>
        public static Type[] EntityTypes => [.. AllTypes.Where(t => string.IsNullOrEmpty(t.FullName) == false && t.FullName.Contains(".Entities."))];
    }
}
//MdEnd
