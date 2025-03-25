//@BaseCode
//MdStart
using TemplateTools.Logic.Common;
using TemplateTools.Logic.Contracts;
using TemplateTools.Logic.Models;

namespace TemplateTools.Logic.Generation
{
    /// <summary>
    /// Represents a generator for generating Web API related items.
    /// </summary>
    /// <remarks>
    /// This generator is responsible for generating models, controllers, and adding services for Web API.
    /// </remarks>
    internal sealed partial class MVVMGenerator : ModelGenerator
    {
        #region fields
        private ItemProperties? _itemProperties;
        #endregion fields

        #region properties
        /// <summary>
        /// Gets the item properties from the base class. If not yet instantiated, it will create a new instance using the solution name and web API extension as parameters.
        /// </summary>
        protected override ItemProperties ItemProperties => _itemProperties ??= new ItemProperties(SolutionProperties.SolutionName, StaticLiterals.MVVMAppExtension);
        /// <summary>
        /// Gets or sets a value indicating whether to generate models.
        /// </summary>
        public bool GenerateModels { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether controllers should be generated.
        /// </summary>
        public bool GenerateViewModels { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether context accesssor should be generated.
        /// </summary>
        public bool GenerateContextAccessor { get; set; }
        #endregion properties

        /// <summary>
        /// Initializes a new instance of the <see cref="MVVMAppGenerator"/> class.
        /// </summary>
        /// <param name="solutionProperties">The solution properties.</param>
        /// <remarks>
        /// This constructor is used to create a new instance of the <see cref="MVVMAppGenerator"/> class with the specified solution properties.
        /// </remarks>
        public MVVMGenerator(ISolutionProperties solutionProperties) : base(solutionProperties)
        {
            GenerateModels = QuerySetting<bool>(ItemType.MVVMAppModel, StaticLiterals.AllItems, StaticLiterals.Generate, "True");
            GenerateViewModels = QuerySetting<bool>(ItemType.MVVVMAppViewModel, StaticLiterals.AllItems, StaticLiterals.Generate, "True");
            GenerateContextAccessor = QuerySetting<bool>(ItemType.ContextAccessor, StaticLiterals.AllItems, StaticLiterals.Generate, "True");
        }

        #region generation
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
        /// Generates all the required items such as models, controllers, and services.
        /// </summary>
        /// <returns>A collection of generated items.</returns>
        public IEnumerable<IGeneratedItem> GenerateAll()
        {
            var result = new List<IGeneratedItem>();
            
            result.AddRange(CreateModels());
            result.AddRange(CreateMVVMViewModels());
            //result.Add(CreateContextAccessor(UnitType.MVVMApp, ItemType.ContextAccessor));
            return result;
        }
        /// <summary>
        /// Creates models based on the entity types in the entity project.
        /// </summary>
        /// <returns>An enumerable collection of generated items.</returns>
        public IEnumerable<IGeneratedItem> CreateModels()
        {
            var result = new List<IGeneratedItem>();
            var entityProject = EntityProject.Create(SolutionProperties);
            
            foreach (var type in entityProject.EntityTypes)
            {
                if (CanCreate(type) && QuerySetting<bool>(ItemType.MVVMAppModel, type, StaticLiterals.Generate, GenerateModels.ToString()))
                {
                    result.Add(CreateModelFromType(type, UnitType.MVVMApp, ItemType.MVVMAppModel));
                    result.Add(CreateModelInheritance(type, UnitType.MVVMApp, ItemType.MVVMAppModel));
                    result.Add(CreateEditModelFromType(type, UnitType.MVVMApp, ItemType.MVVMAppEditModel));
                }
            }
            return result;
        }

        /// <summary>
        /// Creates and returns a generated item for editing a given type.
        /// </summary>
        /// <param name="type">The type to create the edit model from.</param>
        /// <param name="unitType">The unit type of the generated item.</param>
        /// <param name="itemType">The item type of the generated item.</param>
        /// <returns>The generated item for editing the given type.</returns>
        private GeneratedItem CreateEditModelFromType(Type type, UnitType unitType, ItemType itemType)
        {
            var modelName = ItemProperties.CreateEditModelName(type);
            var typeProperties = type.GetAllPropertyInfos();
            var filteredProperties = typeProperties.Where(e => StaticLiterals.VersionProperties.Any(p => p.Equals(e.Name)) == false
            && ItemProperties.IsListType(e.PropertyType) == false);
            var result = new GeneratedItem(unitType, itemType)
            {
                FullName = CreateModelFullName(type),
                FileExtension = StaticLiterals.CSharpFileExtension,
                SubFilePath = ItemProperties.CreateModelSubPath(type, "Edit", StaticLiterals.CSharpFileExtension),
            };
            
            result.AddRange(CreateComment(type));
            CreateModelAttributes(type, unitType, itemType, result.Source);
            result.Add($"public partial class {modelName}");
            result.Add("{");
            result.AddRange(CreatePartialStaticConstrutor(modelName));
            result.AddRange(CreatePartialConstrutor("public", modelName));
            
            foreach (var propertyInfo in filteredProperties.Where(pi => pi.CanWrite))
            {
                result.AddRange(CreateComment(propertyInfo));
                CreateModelPropertyAttributes(propertyInfo, unitType, result.Source);
                result.AddRange(CreateProperty(type, propertyInfo));
            }
            result.Add("}");
            result.EnvelopeWithANamespace(ItemProperties.CreateModelNamespace(type), "using System;");
            result.FormatCSharpCode();
            return result;
        }
        
        /// <summary>
        /// Creates view models for entity types.
        /// </summary>
        /// <returns>An enumerable collection of generated items.</returns>
        private List<IGeneratedItem> CreateMVVMViewModels()
        {
            var result = new List<IGeneratedItem>();
            var entityProject = EntityProject.Create(SolutionProperties);
            
            foreach (var type in entityProject.EntityTypes)
            {
                if (CanCreate(type) && QuerySetting<bool>(ItemType.Controller, type, StaticLiterals.Generate, GenerateViewModels.ToString()))
                {
                    result.Add(CreateMVVMViewModelFromType(type, UnitType.MVVMApp, ItemType.MVVVMAppViewModel));
                }
            }
            return result;
        }
        /// <summary>
        /// Creates a view model from the specified type.
        /// </summary>
        /// <param name="type">The type of the controller.</param>
        /// <param name="unitType">The unit type.</param>
        /// <param name="itemType">The item type.</param>
        /// <returns>An instance of the IGeneratedItem interface representing the created controller.</returns>
        private GeneratedItem CreateMVVMViewModelFromType(Type type, UnitType unitType, ItemType itemType)
        {
            var modelType = ItemProperties.CreateModelSubType(type);
            var viewModelName = ItemProperties.CreateViewModelName(type);
            var typeProperties = type.GetAllPropertyInfos();
            var genericItemViewModel = QuerySetting<string>(itemType, StaticLiterals.AllItems, StaticLiterals.ItemViewModelGenericType, StaticLiterals.GenericItemViewModel);
            var viewModelsSubNamespace = ItemProperties.CreateSubNamespaceFromEntity(type, StaticLiterals.ViewModelsFolder);
            var viewModelsNamespace = $"{ItemProperties.ProjectNamespace}.{viewModelsSubNamespace}";
            var subFilepath = Path.Combine(StaticLiterals.ViewModelsFolder, ItemProperties.CreateSubFilePath(type, $"{viewModelName}{StaticLiterals.CSharpFileExtension}"));
            var filteredProperties = typeProperties.Where(e => StaticLiterals.VersionProperties.Any(p => p.Equals(e.Name)) == false
                                                            && ItemProperties.IsListType(e.PropertyType) == false);
            var result = new GeneratedItem(unitType, itemType)
            {
                FullName = CreateModelFullName(type),
                FileExtension = StaticLiterals.CSharpFileExtension,
                SubFilePath = subFilepath,
            };

            genericItemViewModel = QuerySetting<string>(itemType, type, StaticLiterals.ItemViewModelGenericType, genericItemViewModel);
            result.AddRange(CreateComment(type));
            CreateModelAttributes(type, unitType, itemType, result.Source);
            result.Add($"public partial class {viewModelName} : {genericItemViewModel}<{modelType}>");
            result.Add("{");
            result.AddRange(CreatePartialStaticConstrutor(viewModelName));
            result.AddRange(CreatePartialConstrutor("public", viewModelName));

            //foreach (var propertyInfo in filteredProperties.Where(pi => pi.CanWrite))
            //{
            //    result.AddRange(CreateComment(propertyInfo));
            //    CreateModelPropertyAttributes(propertyInfo, unitType, result.Source);
            //    result.AddRange(CreateProperty(type, propertyInfo));
            //}
            result.Add("}");
            result.EnvelopeWithANamespace(viewModelsNamespace, "using System;");
            result.FormatCSharpCode();
            return result;
        }

        /// <summary>
        /// Creates a context accessor for the specified unit type and item type.
        /// </summary>
        /// <param name="unitType">The unit type for the context accessor.</param>
        /// <param name="itemType">The item type for the context accessor.</param>
        /// <returns>A generated item representing the context accessor.</returns>
        private GeneratedItem CreateContextAccessor(UnitType unitType, ItemType itemType)
        {
            var entityProject = EntityProject.Create(SolutionProperties);
            var itemName = StaticLiterals.ContextAccessor;
            var controllerNamespace = $"{ItemProperties.ProjectNamespace}.{StaticLiterals.ControllersFolder}";
            var contractNamespace = $"{ItemProperties.ProjectNamespace}.{StaticLiterals.ContractsFolder}";
            var subPath = $"{StaticLiterals.ControllersFolder}";
            var fileName = $"{itemName}{StaticLiterals.GenerationPostFix}{StaticLiterals.CSharpFileExtension}";
            var result = new GeneratedItem(unitType, itemType)
            {
                FullName = $"{contractNamespace}.{itemName}",
                FileExtension = StaticLiterals.CSharpFileExtension,
                SubFilePath = Path.Combine(subPath, fileName),
            };
            result.AddRange(CreateComment());
            result.Add($"partial class {itemName}");
            result.Add("{");

            result.AddRange(CreateComment());
            result.Add("partial void GetEntitySetHandler<TEntity>(ref Logic.Contracts.IEntitySet<TEntity>? entitySet, ref bool handled) where TEntity : Logic.Entities.EntityObject, new()");
            result.Add("{");

            if (GenerateContextAccessor)
            {
                foreach (var type in entityProject.EntityTypes)
                {
                    result.Add($"if (typeof(TEntity) == typeof({type.FullName}))");
                    result.Add("{");
                    result.Add($"entitySet = GetContext().{ItemProperties.CreateEntitySetName(type)} as Logic.Contracts.IEntitySet<TEntity>;");
                    result.Add("handled = true;");
                    result.Add("}");
                }
            }

            result.Add("}");

            result.Add("}");
            result.EnvelopeWithANamespace(controllerNamespace);
            result.FormatCSharpCode();
            return result;
        }
        #endregion generation

        #region query configuration
        /// <summary>
        /// Queries the setting value based on the specified parameters.
        /// </summary>
        /// <typeparam name="T">The type of the setting value.</typeparam>
        /// <param name="itemType">The type of the item.</param>
        /// <param name="type">The type of the setting.</param>
        /// <param name="valueName">The name of the setting value.</param>
        /// <param name="defaultValue">The default value to use if the query fails.</param>
        /// <returns>The queried setting value.</returns>
        private T QuerySetting<T>(ItemType itemType, Type type, string valueName, string defaultValue)
        {
            T result;
            
            try
            {
                result = (T)Convert.ChangeType(QueryGenerationSettingValue(UnitType.MVVMApp, itemType, ItemProperties.CreateSubTypeFromEntity(type), valueName, defaultValue), typeof(T));
            }
            catch (Exception ex)
            {
                result = (T)Convert.ChangeType(defaultValue, typeof(T));
                System.Diagnostics.Debug.WriteLine($"Error in {System.Reflection.MethodBase.GetCurrentMethod()!.Name}: {ex.Message}");
            }
            return result;
        }
        /// <summary>
        /// Executes a query to retrieve a setting value and returns the value of type T.
        /// </summary>
        /// <typeparam name="T">The type of the setting value to be returned.</typeparam>
        /// <param name="itemType">The type of the item.</param>
        /// <param name="itemName">The name of the item.</param>
        /// <param name="valueName">The name of the value.</param>
        /// <param name="defaultValue">The default value to be returned in case of an exception.</param>
        /// <returns>The setting value of type T.</returns>
        private T QuerySetting<T>(ItemType itemType, string itemName, string valueName, string defaultValue)
        {
            T result;
            
            try
            {
                result = (T)Convert.ChangeType(QueryGenerationSettingValue(UnitType.MVVMApp, itemType, itemName, valueName, defaultValue), typeof(T));
            }
            catch (Exception ex)
            {
                result = (T)Convert.ChangeType(defaultValue, typeof(T));
                System.Diagnostics.Debug.WriteLine($"Error in {System.Reflection.MethodBase.GetCurrentMethod()!.Name}: {ex.Message}");
            }
            return result;
        }
        #endregion query configuration

        #region Partial methods
        /// <summary>
        /// Creates model attributes for a given type, unit type, and source.
        /// </summary>
        /// <param name="type">The type for which the model attributes are being created.</param>
        /// <param name="unitType">The unit type for the model attributes.</param>
        /// <param name="itemType">The item type.</param>
        /// <param name="source">The source list for the model attributes.</param>
        partial void CreateModelAttributes(Type type, UnitType unitType, ItemType itemType, List<string> source);
        /// <summary>
        /// Creates the attributes for the controller based on the specified type, unit type, and code lines.
        /// </summary>
        /// <param name="type">The type of the controller.</param>
        /// <param name="unitType">The unit type for the controller.</param>
        /// <param name="itemType">The item type.</param>
        /// <param name="codeLines">The list of code lines for the controller.</param>
        partial void CreateControllerAttributes(Type type, UnitType unitType, ItemType itemType, List<string> codeLines);
        #endregion Partial methods
    }
}
//MdEnd
