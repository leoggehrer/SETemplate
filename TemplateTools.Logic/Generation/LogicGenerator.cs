//@BaseCode
//MdStart
namespace TemplateTools.Logic.Generation
{
    /*
    * Contracts:
    *  - AccessContract
    *  - ServcieContract
    *  Models:
    *  - AccessModel
    *  Controllers:
    *  - Controller
    *  - Service
    *  DataContext:
    *  - ProjectDbContextGeneration
    *  Facades:
    *  - Facades
    */
    using System.Reflection;
    using TemplateTools.Logic.Contracts;
    using TemplateTools.Logic.Models;

    /// <summary>
    /// Represents a logic generator that is responsible for generating logic-related code.
    /// </summary>
    internal sealed partial class LogicGenerator : ModelGenerator
    {
        #region fields
        private ItemProperties? _itemProperties;
        #endregion fields

        #region properties
        /// <summary>
        /// Gets or sets the ItemProperties for the current instance.
        /// </summary>
        /// <value>
        /// The ItemProperties for the current instance.
        /// </value>
        protected override ItemProperties ItemProperties => _itemProperties ??= new ItemProperties(SolutionProperties.SolutionName, StaticLiterals.LogicExtension);

        /// <summary>
        /// Gets or sets a value indicating whether to generate the database context.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the database context should be generated; otherwise, <c>false</c>.
        /// </value>
        public bool GenerateDbContext { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether all access models should be generated.
        /// </summary>
        /// <value>
        ///   <c>true</c> if all access models should be generated; otherwise, <c>false</c>.
        /// </value>
        public bool GenerateAllAccessModels { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether all model contracts should be generated.
        /// </summary>
        /// <value>
        ///   <c>true</c> if all model contracts should be generated; otherwise, <c>false</c>.
        /// </value>
        public bool GenerateAllModelContracts { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether all access contracts should be generated.
        /// </summary>
        /// <value>
        /// <c>true</c> if all access contracts should be generated; otherwise, <c>false</c>.
        /// </value>
        public bool GenerateAllAccessContracts { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether all the controllers should be generated.
        /// </summary>
        /// <value>
        ///   <c>true</c> if all the controllers should be generated; otherwise, <c>false</c>.
        /// </value>
        public bool GenerateAllControllers { get; set; }

        /// <summary>
        /// Determines whether all service contracts should be generated.
        /// </summary>
        public bool GenerateAllServiceContracts { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether to generate all services.
        /// </summary>
        /// <value>
        ///   <c>true</c> if all services should be generated; otherwise, <c>false</c>.
        /// </value>
        public bool GenerateAllServices { get; set; }
        /// <summary>
        /// Gets or sets the base address used for making HTTP requests.
        /// </summary>
        /// <value>
        /// The base address as a string.
        /// </value>
        public string BaseAddress { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether all facades should be generated.
        /// </summary>
        /// <value><c>true</c> if all facades should be generated; otherwise, <c>false</c>.</value>
        private bool GenerateAllFacades { get; set; }
        #endregion properties

        #region constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="LogicGenerator"/> class.
        /// </summary>
        /// <param name="solutionProperties">The solution properties.</param>
        public LogicGenerator(ISolutionProperties solutionProperties) : base(solutionProperties)
        {
            var generateAll = QuerySetting<string>(Common.ItemType.AllItems, StaticLiterals.AllItems, StaticLiterals.Generate, "True");

            GenerateDbContext = QuerySetting<bool>(Common.ItemType.DbContext, "All", StaticLiterals.Generate, generateAll);
            GenerateAllAccessModels = QuerySetting<bool>(Common.ItemType.AccessModel, "All", StaticLiterals.Generate, generateAll);

            GenerateAllModelContracts = QuerySetting<bool>(Common.ItemType.ModelContract, "All", StaticLiterals.Generate, generateAll);

            GenerateAllAccessContracts = QuerySetting<bool>(Common.ItemType.ServiceAccessContract, "All", StaticLiterals.Generate, generateAll);
            GenerateAllControllers = QuerySetting<bool>(Common.ItemType.Controller, "All", StaticLiterals.Generate, generateAll);

            GenerateAllServiceContracts = QuerySetting<bool>(Common.ItemType.ServiceContract, "All", StaticLiterals.Generate, generateAll);
            GenerateAllServices = QuerySetting<bool>(Common.ItemType.Service, "All", StaticLiterals.Generate, generateAll);
            BaseAddress = QuerySetting<string>(Common.ItemType.Service, "All", nameof(BaseAddress), "https://localhost:7085/api");

            GenerateAllFacades = QuerySetting<bool>(Common.ItemType.Facade, "All", StaticLiterals.Generate, generateAll);
        }
        #endregion constructors

        #region overrides
        /// <summary>
        /// Creates a delegate auto property.
        /// </summary>
        /// <param name="propertyInfo">The property information.</param>
        /// <param name="delegateObjectName">The delegate object name.</param>
        /// <param name="delegatePropertyInfo">The delegate property information.</param>
        /// <returns>An IEnumerable of strings representing the generated code for the delegate auto property.</returns>
        public override IEnumerable<string> CreateDelegateAutoProperty(PropertyInfo propertyInfo, string delegateObjectName, PropertyInfo delegatePropertyInfo)
        {
            var result = new List<string>();

            if (ItemProperties.IsListType(propertyInfo.PropertyType))
            {
                var entityType = propertyInfo.PropertyType.GenericTypeArguments[0].FullName;
                var modelType = ItemProperties.ConvertEntityToModelType(propertyInfo.PropertyType.GenericTypeArguments[0].FullName!);
                var fieldName = CreateFieldName(propertyInfo, "_");
                var internalPropertyName = $"Internal{propertyInfo.Name}";

                result.Add(string.Empty);
                result.Add($"private CommonModules.Collection.DelegateList<{entityType}, {modelType}>? {fieldName};");
                result.Add($"internal CommonModules.Collection.DelegateList<{entityType}, {modelType}>? {internalPropertyName}");
                result.Add("{");
                result.Add($"get => {fieldName};");
                result.Add($"set => {fieldName} = value;");
                result.Add("}");
                result.AddRange(CreateComment(propertyInfo));
                CreatePropertyAttributes(propertyInfo, result);
                result.Add($"public System.Collections.Generic.IList<{modelType}> {propertyInfo.Name}");
                result.Add("{");
                result.Add($"get => {fieldName} ??= new CommonModules.Collection.DelegateList<{entityType}, {modelType}>({delegateObjectName}.{delegatePropertyInfo.Name}, e => {modelType}.Create(e));");
                result.Add("}");
            }
            else
            {
                result.AddRange(base.CreateDelegateAutoProperty(propertyInfo, delegateObjectName, delegatePropertyInfo));
            }
            return result;
        }
        /// <summary>
        /// Creates a delegate that automates the process of getting a property value.
        /// </summary>
        /// <param name="propertyInfo">The PropertyInfo object that represents the property for which the delegate is created.</param>
        /// <param name="delegateObjectName">The name of the object on which the delegate property belongs.</param>
        /// <param name="delegatePropertyInfo">The PropertyInfo object that represents the delegate property.</param>
        /// <returns>An IEnumerable<string> containing the generated code for the delegate.</returns>
        public override IEnumerable<string> CreateDelegateAutoGet(PropertyInfo propertyInfo, string delegateObjectName, PropertyInfo delegatePropertyInfo)
        {
            var result = new List<string>();
            var propertyType = GetPropertyType(propertyInfo);
            var delegateProperty = $"{delegateObjectName}.{delegatePropertyInfo.Name}";
            var visibility = propertyInfo.GetGetMethod(true)!.IsPublic ? string.Empty : "internal ";

            if (ItemProperties.IsModelType(propertyType))
            {
                if (ItemProperties.IsArrayType(propertyInfo.PropertyType))
                {
                    var modelType = ItemProperties.ConvertEntityToModelType(propertyInfo.PropertyType.GetElementType()!.FullName!);

                    result.Add($"{visibility}get => {delegateProperty}.Select(e => {modelType}.Create(e)).ToArray();");
                }
                else if (ItemProperties.IsListType(propertyInfo.PropertyType))
                {
                    var modelType = ItemProperties.ConvertEntityToModelType(propertyInfo.PropertyType.GenericTypeArguments[0].FullName!);

                    result.Add($"{visibility}get => {delegateProperty}.Select(e => {modelType}.Create(e)).ToList();");
                }
                else
                {
                    result.Add($"{visibility}get => {delegateProperty} != null ? {propertyType.Replace("?", string.Empty)}.Create({delegateProperty}) : null;");
                }
            }
            else
            {
                result.Add($"{visibility}get => {delegateObjectName}.{delegatePropertyInfo.Name};");
            }
            return result;
        }
        /// <summary>
        /// Creates a delegate auto-set for a specified property.
        /// </summary>
        /// <param name="propertyInfo">The PropertyInfo object representing the property.</param>
        /// <param name="delegateObjectName">The name of the delegate object.</param>
        /// <param name="delegatePropertyInfo">The PropertyInfo object representing the delegate property.</param>
        /// <returns>An IEnumerable collection of strings containing the delegate auto-set.</returns>
        //result.Add($"{visibility}set => {delegateProperty} = value.Select(e => e.{delegateObjectName}).ToList();");
        public override IEnumerable<string> CreateDelegateAutoSet(PropertyInfo propertyInfo, string delegateObjectName, PropertyInfo delegatePropertyInfo)
        {
            var result = new List<string>();
            var propertyType = GetPropertyType(propertyInfo);
            var delegateProperty = $"{delegateObjectName}.{delegatePropertyInfo.Name}";
            var visibility = propertyInfo.GetSetMethod(true)!.IsPublic ? string.Empty : "internal ";

            if (ItemProperties.IsModelType(propertyType))
            {
                if (ItemProperties.IsArrayType(propertyInfo.PropertyType))
                {
                    result.Add($"{visibility}set => {delegateProperty} = value.Select(e => e.{delegateObjectName}).ToArray();");
                }
                else if (ItemProperties.IsListType(propertyInfo.PropertyType))
                {
                    //result.Add($"{visibility}set => {delegateProperty} = value.Select(e => e.{delegateObjectName}).ToList();");
                }
                else
                {
                    result.Add($"{visibility}set => {delegateProperty} = value?.{delegateObjectName};");
                }
            }
            else
            {
                result.Add($"{visibility}set => {delegateObjectName}.{delegatePropertyInfo.Name} = value;");
            }
            return result;
        }
        #endregion overrides

        #region generations
        /// <summary>
        /// Generates all the required items for the application.
        /// </summary>
        /// <returns>An enumerable list of generated items.</returns>
        public IEnumerable<IGeneratedItem> GenerateAll()
        {
            var result = new List<IGeneratedItem>
            {
                CreateDbContext()
            };

            result.AddRange(CreateAccessModels());
            result.AddRange(CreateModelContracts());
            result.AddRange(CreateAccessContracts());
            result.AddRange(CreateControllers());
            result.AddRange(CreateServices());
            result.AddRange(CreateFacades());

            //result.Add(CreateFactory());
            return result;
        }

        /// <summary>
        ///   Determines whether the specified type should generate default values.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns>
        ///   <c>true</c> if the specified type is not a generation entity; otherwise, <c>false</c>.
        /// </returns>
        private static bool GetGenerateDefault(Type type)
        {
            return !EntityProject.IsNotAGenerationEntity(type);
        }
        /// <summary>
        /// Determines the visibility default of a given type.
        /// </summary>
        /// <param name="type">The type to determine visibility default for.</param>
        /// <returns>Returns the visibility default, "public" if the type is public, otherwise "internal".</returns>
        private static string GetVisiblityDefault(Type type)
        {
            return type.IsPublic ? "public" : "internal";
        }

        /// <summary>
        /// Creates a database context for the project.
        /// </summary>
        /// <returns>The generated item representing the database context.</returns>
        private GeneratedItem CreateDbContext()
        {
            var entityProject = EntityProject.Create(SolutionProperties);
            var dataContextNamespace = $"{ItemProperties.ProjectNamespace}.DataContext";
            var result = new GeneratedItem(Common.UnitType.Logic, Common.ItemType.DbContext)
            {
                FullName = $"{dataContextNamespace}.ProjectDbContext",
                FileExtension = StaticLiterals.CSharpFileExtension,
                SubFilePath = $"DataContext{Path.DirectorySeparatorChar}ProjectDbContextGeneration{StaticLiterals.CSharpFileExtension}",
            };
            result.AddRange(CreateComment());
            result.Add($"partial class ProjectDbContext");
            result.Add("{");

            foreach (var type in entityProject.EntityTypes)
            {
                var defaultValue = (GenerateDbContext && GetGenerateDefault(type)).ToString();

                if (QuerySetting<bool>(Common.ItemType.DbContext, type, StaticLiterals.Generate, defaultValue))
                {
                    var entityType = ItemProperties.GetModuleSubType(type);

                    result.AddRange(CreateComment(type));
                    result.Add($"public DbSet<{entityType}> {type.Name}Set" + "{ get; set; }");
                }
            }

            result.Add(string.Empty);
            result.AddRange(CreateComment());
            result.Add($"partial void GetGeneratorDbSet<E>(ref DbSet<E>? dbSet, ref bool handled) where E : Entities.{StaticLiterals.EntityObjectName}");
            result.Add("{");

            bool first = false;

            foreach (var type in entityProject.EntityTypes)
            {
                var defaultValue = (GenerateDbContext && GetGenerateDefault(type)).ToString();

                if (QuerySetting<bool>(Common.ItemType.DbContext, type, StaticLiterals.Generate, defaultValue))
                {
                    var entityType = ItemProperties.GetModuleSubType(type);

                    result.Add($"{(first ? "else " : string.Empty)}if (typeof(E) == typeof({entityType}))");
                    result.Add("{");
                    result.Add($"dbSet = {type.Name}Set as DbSet<E>;");
                    result.Add("handled = true;");
                    result.Add("}");
                    first = true;
                }
            }
            result.Add("}");

            result.Add("}");
            result.EnvelopeWithANamespace(dataContextNamespace);
            result.FormatCSharpCode();
            return result;
        }

        /// <summary>
        /// Creates access models for each entity type in the EntityProject.
        /// </summary>
        /// <returns>
        /// An IEnumerable of IGeneratedItem objects representing the access models.
        /// </returns>
        private List<GeneratedItem> CreateAccessModels()
        {
            var result = new List<GeneratedItem>();
            var entityProject = EntityProject.Create(SolutionProperties);

            foreach (var type in entityProject.EntityTypes)
            {
                var defaultValue = (GenerateAllAccessModels && GetGenerateDefault(type)).ToString();

                if (CanCreate(type) && QuerySetting<bool>(Common.ItemType.AccessModel, type, StaticLiterals.Generate, defaultValue))
                {
                    result.Add(CreateDelegateModelFromType(type, Common.UnitType.Logic, Common.ItemType.AccessModel));
                    result.Add(CreateDelegateModelNavigationsFromType(type, Common.UnitType.Logic, Common.ItemType.AccessModel));
                    result.Add(CreateModelInheritance(type, Common.UnitType.Logic, Common.ItemType.AccessModel));
                }
            }
            return result;
        }

        /// <summary>
        /// Creates model contracts.
        /// </summary>
        /// <returns>An enumerable collection of generated items.</returns>
        // Create the entity project from the solution properties
        // Loop through each entity type in the entity project
        // Get the default value for generating the model contracts
        // Check if the model contract can be created and meets the specified conditions
        // Return the generated items
        private List<GeneratedItem> CreateModelContracts()
        {
            var result = new List<GeneratedItem>();
            var entityProject = EntityProject.Create(SolutionProperties);

            foreach (var type in entityProject.EntityTypes)
            {
                var defaultValue = (GenerateAllModelContracts && GetGenerateDefault(type)).ToString();

                if (CanCreate(type) && QuerySetting<bool>(Common.ItemType.ModelContract, type, StaticLiterals.Generate, defaultValue))
                {
                    result.Add(CreateModelContract(type, Common.UnitType.Logic, Common.ItemType.ModelContract));
                    result.Add(CreateModelNavigationsContract(type, Common.UnitType.Logic, Common.ItemType.ModelContract));
                }
            }
            return result;
        }
        /// <summary>
        /// Creates a model contract for the specified type, unit type, and item type.
        /// </summary>
        /// <param name="type">The type for which to create the model contract.</param>
        /// <param name="unitType">The unit type used for the generated item.</param>
        /// <param name="itemType">The item type used for the generated item.</param>
        /// <returns>The generated model contract as an <see cref="IGeneratedItem"/>.</returns>
        private GeneratedItem CreateModelContract(Type type, Common.UnitType unitType, Common.ItemType itemType)
        {
            var itemName = ItemProperties.CreateModelContractName(type);
            var fileName = $"{itemName}{StaticLiterals.CSharpFileExtension}";
            var typeProperties = type.GetAllPropertyInfos();
            var generateProperties = typeProperties.Where(e => StaticLiterals.NoGenerationProperties.Any(p => p.Equals(e.Name)) == false) ?? [];
            var result = new GeneratedItem(unitType, itemType)
            {
                FullName = ItemProperties.CreateFullLogicModelContractType(type),
                FileExtension = StaticLiterals.CSharpFileExtension,
                SubFilePath = ItemProperties.CreateSubFilePath(type, fileName, StaticLiterals.ContractsFolder),
            };
            result.AddRange(CreateComment(type));
            result.Add($"public partial interface {itemName}");
            result.Add("{");
            foreach (var propertyItem in generateProperties.Where(pi => ItemProperties.IsEntityType(pi.PropertyType) == false
            && ItemProperties.IsEntityListType(pi.PropertyType) == false))
            {
                if (QuerySetting<bool>(unitType, Common.ItemType.InterfaceProperty, propertyItem.DeclaringName(), StaticLiterals.Generate, "True"))
                {
                    var getAccessor = string.Empty;
                    var setAccessor = string.Empty;
                    var propertyType = GetPropertyType(propertyItem);

                    if (QuerySetting<bool>(unitType, Common.ItemType.InterfaceProperty, propertyItem.DeclaringName(), StaticLiterals.GetAccessor, "True"))
                    {
                        getAccessor = "get;";
                    }
                    if (QuerySetting<bool>(unitType, Common.ItemType.InterfaceProperty, propertyItem.DeclaringName(), StaticLiterals.SetAccessor, "True"))
                    {
                        setAccessor = "set;";
                    }
                    result.Add($"{propertyType} {propertyItem.Name}" + " { " + $"{getAccessor} {setAccessor}" + " } ");
                }
            }
            result.Add("}");
            result.EnvelopeWithANamespace(ItemProperties.CreateFullLogicNamespace(type, StaticLiterals.ContractsFolder));
            result.FormatCSharpCode();
            return result;
        }
        /// <summary>
        /// Creates a model navigation contract based on the provided type, unit type, and item type.
        /// </summary>
        /// <param name="type">The type of the model.</param>
        /// <param name="unitType">The unit type of the model.</param>
        /// <param name="itemType">The item type of the model.</param>
        /// <returns>The generated model navigation contract.</returns>
        private GeneratedItem CreateModelNavigationsContract(Type type, Common.UnitType unitType, Common.ItemType itemType)
        {
            var itemName = $"{ItemProperties.CreateModelNavigationContractName(type)}";
            var fileName = $"{itemName}{StaticLiterals.CSharpFileExtension}";
            var typeProperties = type.GetAllPropertyInfos();
            var generateProperties = typeProperties.Where(e => StaticLiterals.NoGenerationProperties.Any(p => p.Equals(e.Name)) == false) ?? [];
            var result = new GeneratedItem(unitType, itemType)
            {
                FullName = ItemProperties.CreateFullLogicModelNavigationContractType(type),
                FileExtension = StaticLiterals.CSharpFileExtension,
                SubFilePath = ItemProperties.CreateSubFilePath(type, fileName, StaticLiterals.ContractsFolder),
            };
            result.AddRange(CreateComment(type));
            result.Add($"public partial interface {itemName}");
            result.Add("{");
            foreach (var propertyItem in generateProperties.Where(pi => ItemProperties.IsEntityType(pi.PropertyType)
            || ItemProperties.IsEntityListType(pi.PropertyType)))
            {
                if (QuerySetting<bool>(unitType, Common.ItemType.InterfaceProperty, propertyItem.DeclaringName(), StaticLiterals.Generate, "True"))
                {
                    var propertyType = GetPropertyType(propertyItem);

                    if (ItemProperties.IsModelType(propertyType))
                    {
                        if (ItemProperties.IsArrayType(propertyItem.PropertyType))
                        {
                            var modelType = ItemProperties.ConvertEntityToModelType(propertyItem.PropertyType.GetElementType()!.FullName!);

                            result.Add($"{modelType}[] {propertyItem.Name}" + " { get; }");
                        }
                        else if (ItemProperties.IsListType(propertyItem.PropertyType))
                        {
                            var modelType = ItemProperties.ConvertEntityToModelType(propertyItem.PropertyType.GenericTypeArguments[0].FullName!);

                            result.Add($"System.Collections.Generic.IList<{modelType}> {propertyItem.Name}" + " { get; }");
                        }
                        else
                        {
                            var modelType = ItemProperties.ConvertEntityToModelType(type.FullName!);

                            result.Add($"{modelType} {propertyItem.Name}" + " { get; }");
                        }
                    }
                }
            }
            result.Add("}");
            result.EnvelopeWithANamespace(ItemProperties.CreateFullLogicNamespace(type, StaticLiterals.ContractsFolder));
            result.FormatCSharpCode();
            return result;
        }

        /// <summary>
        /// Creates access contracts and service contracts for entity types and service types.
        /// </summary>
        /// <returns>An enumerable list of generated items.</returns>
        private List<GeneratedItem> CreateAccessContracts()
        {
            var result = new List<GeneratedItem>();
            var entityProject = EntityProject.Create(SolutionProperties);

            foreach (var type in entityProject.EntityTypes)
            {
                var defaultValue = (GenerateAllAccessContracts && GetGenerateDefault(type)).ToString();

                if (CanCreate(type)
                && QuerySetting<bool>(Common.ItemType.AccessContract, type, StaticLiterals.Generate, defaultValue))
                {
                    result.Add(CreateAccessContract(type, Common.UnitType.Logic, Common.ItemType.AccessContract));
                }
            }

            foreach (var type in entityProject.ServiceTypes)
            {
                var defaultValue = (GenerateAllServiceContracts && GetGenerateDefault(type)).ToString();

                if (CanCreate(type)
                && QuerySetting<bool>(Common.ItemType.ServiceContract, type, StaticLiterals.Generate, defaultValue))
                {
                    result.Add(CreateServiceContract(type, Common.UnitType.Logic, Common.ItemType.ServiceContract));
                }
            }
            return result;
        }
        /// <summary>
        /// Creates an access contract based on the provided <paramref name="type"/>, <paramref name="unitType"/>, and <paramref name="itemType"/>.
        /// </summary>
        /// <param name="type">The type of access contract to create.</param>
        /// <param name="unitType">The unit type for the generated item.</param>
        /// <param name="itemType">The item type for the generated item.</param>
        /// <returns>The created <see cref="IGeneratedItem"/>.</returns>
        private GeneratedItem CreateAccessContract(Type type, Common.UnitType unitType, Common.ItemType itemType)
        {
            var itemName = ItemProperties.CreateAccessContractName(type);
            var fileName = $"{itemName}{StaticLiterals.CSharpFileExtension}";
            var outModelType = ItemProperties.CreateModelSubType(type);
            var result = new Models.GeneratedItem(unitType, itemType)
            {
                FullName = ItemProperties.CreateFullLogicAccessContractType(type),
                FileExtension = StaticLiterals.CSharpFileExtension,
                SubFilePath = ItemProperties.CreateSubFilePath(type, fileName, StaticLiterals.ContractsFolder),
            };
            result.Add($"using TOutModel = {outModelType};");
            result.AddRange(CreateComment(type));
            result.Add($"public partial interface {itemName} : CommonContracts.IDataAccess<TOutModel>");
            result.Add("{");
            result.Add("}");
            result.EnvelopeWithANamespace(ItemProperties.CreateFullLogicNamespace(type, StaticLiterals.ContractsFolder));
            result.FormatCSharpCode();
            return result;
        }
        /// <summary>
        /// Creates a service contract based on the specified type, unit type, and item type.
        /// </summary>
        /// <param name="type">The type of the service contract.</param>
        /// <param name="unitType">The unit type of the service contract.</param>
        /// <param name="itemType">The item type of the service contract.</param>
        /// <returns>An instance of <see cref="IGeneratedItem"/> representing the created service contract.</returns>
        private GeneratedItem CreateServiceContract(Type type, Common.UnitType unitType, Common.ItemType itemType)
        {
            var itemName = ItemProperties.CreateServiceContractName(type);
            var fileName = $"{itemName}{StaticLiterals.CSharpFileExtension}";
            var modelType = ItemProperties.CreateModelSubType(type);
            var result = new Models.GeneratedItem(unitType, itemType)
            {
                FullName = ItemProperties.CreateFullLogicServiceContractType(type),
                FileExtension = StaticLiterals.CSharpFileExtension,
                SubFilePath = ItemProperties.CreateSubFilePath(type, fileName, StaticLiterals.ContractsFolder),
            };
            result.Add($"using TOutModel = {modelType};");
            result.AddRange(CreateComment(type));
            result.Add($"public partial interface {itemName} : CommonContracts.IBaseAccess<TOutModel>");
            result.Add("{");
            result.Add("}");
            result.EnvelopeWithANamespace(ItemProperties.CreateFullLogicNamespace(type, StaticLiterals.ContractsFolder));
            result.FormatCSharpCode();
            return result;
        }

        /// <summary>
        /// Creates controllers for all entity types in the entity project.
        /// </summary>
        /// <returns>An enumerable collection of generated items representing the created controllers.</returns>
        private List<IGeneratedItem> CreateControllers()
        {
            var result = new List<IGeneratedItem>();
            var entityProject = EntityProject.Create(SolutionProperties);

            foreach (var type in entityProject.EntityTypes)
            {
                var defaultValue = (GenerateAllAccessContracts && GenerateAllControllers && GetGenerateDefault(type)).ToString();

                if (CanCreate(type)
                && QuerySetting<bool>(Common.ItemType.Controller, type, StaticLiterals.Generate, defaultValue))
                {
                    result.Add(CreateControllerFromType(type, Common.UnitType.Logic, Common.ItemType.Controller));
                }
            }
            return result;
        }
        /// <summary>
        /// Creates a controller item from the specified type, unit type, and item type.
        /// </summary>
        /// <param name="type">The type of the controller.</param>
        /// <param name="unitType">The unit type of the controller.</param>
        /// <param name="itemType">The item type of the controller.</param>
        /// <returns>The generated controller item.</returns>
        private GeneratedItem CreateControllerFromType(Type type, Common.UnitType unitType, Common.ItemType itemType)
        {
            var visibility = QuerySetting<string>(itemType, type, StaticLiterals.Visibility, GetVisiblityDefault(type));
            var attributes = QuerySetting<string>(itemType, type, StaticLiterals.Attribute, string.Empty);
            var controllerGenericType = QuerySetting<string>(itemType, "All", StaticLiterals.ControllerGenericType, "EntitiesController");
            var entityType = ItemProperties.GetModuleSubType(type);
            var outModelType = ItemProperties.CreateModelSubType(type);
            var controllerName = ItemProperties.CreateControllerClassName(type);
            var contractSubType = ItemProperties.CreateAccessContractSubType(type);
            var result = new GeneratedItem(unitType, itemType)
            {
                FullName = ItemProperties.CreateFullLogicControllerType(type),
                FileExtension = StaticLiterals.CSharpFileExtension,
                SubFilePath = ItemProperties.CreateControllersSubPathFromType(type, string.Empty, StaticLiterals.CSharpFileExtension),
            };

            controllerGenericType = QuerySetting<string>(itemType, type, StaticLiterals.ControllerGenericType, controllerGenericType);
            result.Add($"using TEntity = {entityType};");
            result.Add($"using TOutModel = {outModelType};");
            result.AddRange(CreateComment(type));
            CreateControllerAttributes(type, result.Source);
            result.Add($"{(attributes.HasContent() ? $"[{attributes}]" : string.Empty)}");
            result.Add($"{visibility} sealed partial class {controllerName} : {controllerGenericType}<TEntity, TOutModel>, {contractSubType}");
            result.Add("{");
            result.AddRange(CreatePartialStaticConstrutor(controllerName));
            result.AddRange(CreatePartialConstrutor("public", controllerName));
            result.AddRange(CreatePartialConstrutor("public", controllerName, "ControllerObject other", "base(other)", null, false));

            result.AddRange(CreateComment(type));
            result.Add($"internal override TOutModel ToModel(TEntity entity)");
            result.Add("{");
            result.Add("var handled = false;");
            result.Add("TOutModel? result = default;");
            result.Add(string.Empty);
            result.Add("BeforeToOutModel(entity, ref result, ref handled);");
            result.Add("if (handled == false || result == default)");
            result.Add("{");
            result.Add("result = new TOutModel(entity);");
            result.Add("}");
            result.Add("AfterToOutModel(entity, result);");
            result.Add("return result;");
            result.Add("}");
            result.Add("partial void BeforeToOutModel(TEntity entity, ref TOutModel? result, ref bool handled);");
            result.Add("partial void AfterToOutModel(TEntity entity, TOutModel result);");

            result.Add("}");
            result.EnvelopeWithANamespace(ItemProperties.CreateFullLogicNamespace(type, StaticLiterals.ControllersFolder));
            result.FormatCSharpCode();
            return result;
        }

        /// <summary>
        ///     Creates services based on the specified conditions and returns them as an enumerable of generated items.
        /// </summary>
        /// <returns>
        ///     An enumerable of <see cref="IGeneratedItem"/> representing the generated services.
        /// </returns>
        private List<GeneratedItem> CreateServices()
        {
            var result = new List<GeneratedItem>();
            var entityProject = EntityProject.Create(SolutionProperties);

            if (GenerateAllServices)
            {
                foreach (var type in entityProject.ServiceTypes)
                {
                    var defaultValue = (GenerateAllServiceContracts && GenerateAllServices && GetGenerateDefault(type)).ToString();

                    if (CanCreate(type)
                    && QuerySetting<bool>(Common.ItemType.Service, type, StaticLiterals.Generate, defaultValue))
                    {
                        result.Add(CreateServiceFromType(type, Common.UnitType.Logic, Common.ItemType.Service));
                    }
                }
            }
            return result;
        }
        /// <summary>
        /// Creates a service from the specified type, unit type, and item type.
        /// </summary>
        /// <param name="type">The type of the service.</param>
        /// <param name="unitType">The unit type of the service.</param>
        /// <param name="itemType">The item type of the service.</param>
        /// <returns>The generated service item.</returns>
        private GeneratedItem CreateServiceFromType(Type type, Common.UnitType unitType, Common.ItemType itemType)
        {
            var visibility = QuerySetting<string>(itemType, type, StaticLiterals.Visibility, type.IsPublic ? "public" : "internal");
            var attributes = QuerySetting<string>(itemType, type, StaticLiterals.Attribute, string.Empty);
            var baseAddress = QuerySetting<string>(itemType, type, nameof(BaseAddress), BaseAddress);
            var requestUri = QuerySetting<string>(itemType, type, "RequestUri", type.Name.CreatePluralWord());
            var serviceGenericType = QuerySetting<string>(itemType, "All", StaticLiterals.ServiceGenericType, "CommonServices.GenericService");
            var serviceType = ItemProperties.CreateModelSubType(type);
            var serviceName = ItemProperties.CreateServiceClassName(type);
            var fileName = $"{serviceName}{StaticLiterals.CSharpFileExtension}";
            var contractSubType = ItemProperties.CreateServiceContractSubType(type);
            var result = new GeneratedItem(unitType, itemType)
            {
                FullName = ItemProperties.CreateFullLogicServiceType(type),
                FileExtension = StaticLiterals.CSharpFileExtension,
                SubFilePath = ItemProperties.CreateSubFilePath(type, fileName, StaticLiterals.ServicesFolder),
            };

            serviceGenericType = QuerySetting<string>(itemType, type, StaticLiterals.ServiceGenericType, serviceGenericType);
            result.Add($"using TModel = {serviceType};");
            result.AddRange(CreateComment(type));
            CreateControllerAttributes(type, result.Source);
            result.Add($"{(attributes.HasContent() ? $"[{attributes}]" : string.Empty)}");
            result.Add($"{visibility} sealed partial class {serviceName} : {serviceGenericType}<TModel>, {contractSubType}");
            result.Add("{");
            result.Add($"private static string ServiceBaseAddress = \"{baseAddress}\";");
            result.Add($"private static string ServiceRequestUri = \"{requestUri}\";");
            result.AddRange(CreatePartialStaticConstrutor(serviceName));
            result.AddRange(CreatePartialConstrutor("public", serviceName, null, "base(ServiceBaseAddress, ServiceRequestUri)"));
            result.Add("}");
            result.EnvelopeWithANamespace(ItemProperties.CreateFullLogicNamespace(type, StaticLiterals.ServicesFolder));
            result.FormatCSharpCode();
            return result;
        }

        /// <summary>
        /// Creates facades for the specified entity types.
        /// </summary>
        /// <returns>
        /// An <see cref="IEnumerable{T}"/> of <see cref="IGeneratedItem"/> representing the generated facades.
        /// </returns>
        private List<GeneratedItem> CreateFacades()
        {
            var result = new List<GeneratedItem>();
            var entityProject = EntityProject.Create(SolutionProperties);

            if (GenerateAllFacades)
            {
                foreach (var type in entityProject.EntityTypes)
                {
                    var defaultValue = (GenerateAllAccessContracts && GenerateAllControllers && GenerateAllFacades && GetGenerateDefault(type)).ToString();

                    if (CanCreate(type)
                    && QuerySetting<bool>(Common.ItemType.Facade, type, StaticLiterals.Generate, defaultValue))
                    {
                        result.Add(CreateFacadeFromType(type, Common.UnitType.Logic, Common.ItemType.Facade));
                    }
                }
            }

            return result;
        }
        /// <summary>
        /// Creates a facade object from the given type, unit type, and item type.
        /// </summary>
        /// <param name="type">The type used to create the facade.</param>
        /// <param name="unitType">The unit type.</param>
        /// <param name="itemType">The item type.</param>
        /// <returns>An interface object representing the generated facade.</returns>
        private GeneratedItem CreateFacadeFromType(Type type, Common.UnitType unitType, Common.ItemType itemType)
        {
            var facadeGenericType = QuerySetting<string>(itemType, "All", StaticLiterals.FacadeGenericType, "ControllerFacade");
            var outModelType = $"{ItemProperties.CreateModelSubType(type)}";
            var controllerType = ItemProperties.CreateControllerType(type);
            var facadeName = ItemProperties.CreateFacadeClassName(type);
            var fileName = $"{facadeName}{StaticLiterals.CSharpFileExtension}";
            var contractSubType = ItemProperties.CreateFacadeContractSubType(type);
            var result = new GeneratedItem(unitType, itemType)
            {
                FullName = ItemProperties.CreateFullLogicFacadeType(type),
                FileExtension = StaticLiterals.CSharpFileExtension,
                SubFilePath = ItemProperties.CreateSubFilePath(type, fileName, StaticLiterals.FacadesFolder),
            };

            facadeGenericType = QuerySetting<string>(itemType, type, StaticLiterals.FacadeGenericType, facadeGenericType);
            result.Add($"using TOutModel = {outModelType};");
            result.Add($"using TAccessContract = {contractSubType};");
            result.AddRange(CreateComment(type));
            result.Add($"public sealed partial class {facadeName} : {facadeGenericType}<TOutModel, TAccessContract>, TAccessContract");
            result.Add("{");
            result.AddRange(CreatePartialStaticConstrutor(facadeName));
            result.AddRange(CreatePartialConstrutor("public", facadeName, null, $"base(new {controllerType}())"));
            result.AddRange(CreatePartialConstrutor("public", facadeName, "FacadeObject facadeObject", $"base(new {controllerType}(facadeObject.ControllerObject))", withPartials: false));

            result.Add("}");
            result.EnvelopeWithANamespace(ItemProperties.CreateFullLogicNamespace(type, StaticLiterals.FacadesFolder));
            result.FormatCSharpCode();
            return result;
        }

        /// <summary>
        /// Queries a setting value and converts it to the specified type.
        /// </summary>
        /// <typeparam name="T">The type to which the setting value will be converted.</typeparam>
        /// <param name="itemType">The common item type.</param>
        /// <param name="type">The type of the setting value.</param>
        /// <param name="valueName">The name of the setting value.</param>
        /// <param name="defaultValue">The default value to use if the setting value cannot be queried or converted.</param>
        /// <returns>
        /// The queried setting value converted to the specified type. If the setting value cannot be queried or converted,
        /// the default value will be returned.
        /// </returns>
        /// <remarks>
        /// If an exception occurs during the query or conversion process, the default value will be returned
        /// and the error message will be written to the debug output.
        /// </remarks>
        private T QuerySetting<T>(Common.ItemType itemType, Type type, string valueName, string defaultValue)
        {
            T result;

            try
            {
                result = (T)Convert.ChangeType(QueryGenerationSettingValue(Common.UnitType.Logic, itemType, ItemProperties.CreateSubTypeFromEntity(type), valueName, defaultValue), typeof(T));
            }
            catch (Exception ex)
            {
                result = (T)Convert.ChangeType(defaultValue, typeof(T));
                System.Diagnostics.Debug.WriteLine($"Error in {MethodBase.GetCurrentMethod()!.Name}: {ex.Message}");
            }
            return result;
        }
        /// <summary>
        /// Executes a query to retrieve a setting value and returns the result as the specified type.
        /// </summary>
        /// <typeparam name="T">The type to which the setting value should be converted.</typeparam>
        /// <param name="itemType">The type of item to query for the setting value.</param>
        /// <param name="itemName">The name of the item to query for the setting value.</param>
        /// <param name="valueName">The name of the value to query for.</param>
        /// <param name="defaultValue">The default value to return if the query fails or the value cannot be converted.</param>
        /// <returns>The setting value as the specified type.</returns>
        private T QuerySetting<T>(Common.ItemType itemType, string itemName, string valueName, string defaultValue)
        {
            T result;

            try
            {
                result = (T)Convert.ChangeType(QueryGenerationSettingValue(Common.UnitType.Logic, itemType, itemName, valueName, defaultValue), typeof(T));
            }
            catch (Exception ex)
            {
                result = (T)Convert.ChangeType(defaultValue, typeof(T));
                System.Diagnostics.Debug.WriteLine($"Error in {MethodBase.GetCurrentMethod()!.Name}: {ex.Message}");
            }
            return result;
        }
        #endregion generations

        #region partial methods
        /// <summary>
        /// Creates attributes for a controller.
        /// </summary>
        /// <param name="type">The type of the controller.</param>
        /// <param name="codeLines">The list of code lines.</param>
        partial void CreateControllerAttributes(Type type, List<string> codeLines);
        #endregion partial methods
    }
}
//MdEnd
