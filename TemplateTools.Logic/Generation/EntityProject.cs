//@BaseCode
//MdStart
namespace TemplateTools.Logic.Generation
{
    using System.Reflection;
    using TemplateTools.Logic.Contracts;
    /// <summary>
    /// Represents an internal sealed partial class for working with entity projects.
    /// </summary>
    internal sealed partial class EntityProject
    {
        /// <summary>
        /// Gets or sets the solution properties.
        /// </summary>
        /// <value>
        /// The solution properties.
        /// </value>
        public ISolutionProperties SolutionProperties { get; private set; }
        /// <summary>
        /// Gets the name of the project by combining the solution name and logic extension.
        /// </summary>
        /// <value>The name of the project.</value>
        public string ProjectName => $"{SolutionProperties.SolutionName}{SolutionProperties.LogicExtension}";
        /// <summary>
        /// Gets the full path of the project, combining the solution path and project name.
        /// </summary>
        /// <value>The full path of the project.</value>
        public string ProjectPath => Path.Combine(SolutionProperties.SolutionPath, ProjectName);
        
        /// <summary>
        /// Initializes a new instance of the EntityProject class.
        /// </summary>
        /// <param name="solutionProperties">The solution properties.</param>
        private EntityProject(ISolutionProperties solutionProperties)
        {
            SolutionProperties = solutionProperties;
        }
        /// <summary>
        /// Creates a new instance of the EntityProject class using the provided solution properties.
        /// </summary>
        /// <param name="solutionProperties">The solution properties to be used in creating the EntityProject instance.</param>
        /// <returns>A new instance of the EntityProject class.</returns>
        public static EntityProject Create(ISolutionProperties solutionProperties)
        {
            return new(solutionProperties);
        }
        
        private IEnumerable<Type>? assemblyTypes;
        ///<summary>
        /// Gets or sets the types in the assembly.
        ///</summary>
        ///<value>
        /// An IEnumerable of Type representing the collection of types in the assembly.
        ///</value>
        ///<remarks>
        /// This property is used to retrieve the types present in the assembly.
        /// The assembly types are obtained either from SolutionProperties.LogicAssemblyTypes if already assigned,
        /// or by loading the assembly from SolutionProperties.CompileLogicAssemblyFilePath
        /// or SolutionProperties.LogicAssemblyFilePath if the former is not available.
        /// If the assembly fails to load or if any exceptions occur while retrieving the types,
        /// the property returns an empty array.
        ///</remarks>
        public IEnumerable<Type> AssemblyTypes
        {
            get
            {
                assemblyTypes = assemblyTypes ??= SolutionProperties.LogicAssemblyTypes;
                if (assemblyTypes == null)
                {
                    if (SolutionProperties.CompileLogicAssemblyFilePath.HasContent() && File.Exists(SolutionProperties.CompileLogicAssemblyFilePath))
                    {
                        var assembly = Assembly.LoadFile(SolutionProperties.CompileLogicAssemblyFilePath!);
                        
                        if (assembly != null)
                        {
                            try
                            {
                                assemblyTypes = assembly.GetTypes();
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Error in {nameof(assemblyTypes)}: {ex.Message}");
                            }
                        }
                    }
                    if (assemblyTypes == null && SolutionProperties.LogicAssemblyFilePath.HasContent() && File.Exists(SolutionProperties.LogicAssemblyFilePath))
                    {
                        var assembly = Assembly.LoadFile(SolutionProperties.LogicAssemblyFilePath);
                        
                        if (assembly != null)
                        {
                            try
                            {
                                assemblyTypes = assembly.GetTypes();
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Error in {nameof(assemblyTypes)}: {ex.Message}");
                            }
                        }
                    }
                }
                return assemblyTypes ?? [];
            }
        }
        
        /// <summary>
        /// Retrieves the collection of all enum types within the assembly.
        /// </summary>
        /// <value>
        /// An IEnumerable of Type objects that represent the enum types.
        /// </value>
        public IEnumerable<Type> EnumTypes => AssemblyTypes.Where(t => t.IsEnum && t.IsNested == false);
        /// <summary>
        /// Gets the interface types found within the assembly.
        /// </summary>
        /// <value>
        /// An enumerable collection of Type objects representing the interface types.
        /// </value>
        public IEnumerable<Type> InterfaceTypes => AssemblyTypes.Where(t => t.IsInterface);
        /// <summary>
        /// Gets the collection of entity types, excluding certain types, within the assembly.
        /// </summary>
        /// <remarks>
        /// This property returns a collection of types that meet the following criteria:
        /// - The type is a class and not abstract
        /// - The type is not nested
        /// - The type belongs to a namespace that is not null and contains the specified entities folder
        /// - The type's full name does not contain the specified account folder
        /// - The type's full name does not contain the specified logging folder
        /// - The type's name is not equal to the specified entity object name
        /// - The type's name is not equal to the specified version entity name
        /// </remarks>
        public IEnumerable<Type> EntityTypes => AssemblyTypes.Where(t => t.IsClass
                                                                      && t.IsAbstract == false
                                                                      && t.IsNested == false
                                                                      && t.Namespace != null
                                                                      && t.Namespace!.Contains($".{StaticLiterals.EntitiesFolder}")
        
                                                                      && t.FullName!.Contains($"{StaticLiterals.EntitiesFolder}.{StaticLiterals.AccountFolder}.") == false
                                                                      && t.FullName!.Contains($"{StaticLiterals.EntitiesFolder}.{StaticLiterals.LoggingFolder}.") == false
                                                                      && t.FullName!.Contains($"{StaticLiterals.EntitiesFolder}.{StaticLiterals.LoggingFolder}.") == false
        
                                                                      && t.Name.Equals(StaticLiterals.EntityObjectName) == false);
        /// <summary>
        /// Determines if the specified <paramref name="type"/> is an account entity.
        /// </summary>
        /// <param name="type">The type to be checked.</param>
        /// <returns>
        /// <c>true</c> if the specified <paramref name="type"/> is an account entity; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsAccountEntity(Type type)
        {
            var result = type.FullName!.EndsWith($".{StaticLiterals.Account}.Identity")
            || type.FullName!.EndsWith($".{StaticLiterals.Account}.IdentityXRole")
            || type.FullName!.EndsWith($".{StaticLiterals.Account}.LoginSession")
            || type.FullName!.EndsWith($".{StaticLiterals.Account}.Role")
            || type.FullName!.EndsWith($".{StaticLiterals.Account}.SecureIdentity")
            || type.FullName!.EndsWith($".{StaticLiterals.Account}.User");
            
            return result;
        }
        /// <summary>
        /// Determines whether the specified type is an access entity.
        /// </summary>
        /// <param name="type">The type to be checked.</param>
        /// <returns>
        ///   <c>true</c> if the specified type is an access entity; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsAccessEntity(Type type)
        {
            return type.FullName!.EndsWith($".{StaticLiterals.Access}.AccessRule");
        }
        /// <summary>
        /// Determines whether the specified type is a logging entity.
        /// </summary>
        /// <param name="type">
        /// The type to check.
        /// </param>
        /// <returns>
        /// <c>true</c> if the specified type is a logging entity; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsLoggingEntity(Type type)
        {
            return type.FullName!.EndsWith($".{StaticLiterals.Logging}.ActionLog");
        }
        /// <summary>
        /// Checks if the specified type is a revision entity.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns>
        ///   <c>true</c> if the specified type is a revision entity; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsRevisionEntity(Type type)
        {
            return type.FullName!.EndsWith($".{StaticLiterals.Revision}.History");
        }
        ///<summary>
        ///Checks if the given type is not a Generation entity.
        ///</summary>
        ///<param name="type">The type to check.</param>
        ///<returns>True if the type is not a Generation entity, false otherwise.</returns>
        public static bool IsNotAGenerationEntity(Type type)
        {
            return IsAccountEntity(type) || IsAccessEntity(type) || IsLoggingEntity(type) || IsRevisionEntity(type);
        }
    }
}
//MdEnd
