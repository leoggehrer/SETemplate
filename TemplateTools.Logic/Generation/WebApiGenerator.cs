//@BaseCode
//MdStart
using TemplateTools.Logic.Common;
using TemplateTools.Logic.Contracts;
using TemplateTools.Logic.Extensions;
using TemplateTools.Logic.Models;

namespace TemplateTools.Logic.Generation
{
    /// <summary>
    /// Represents a generator for generating Web API related items.
    /// </summary>
    /// <remarks>
    /// This generator is responsible for generating models, controllers, and adding services for Web API.
    /// </remarks>
    internal sealed partial class WebApiGenerator : ModelGenerator
    {
        private ItemProperties? _itemProperties;
        /// <summary>
        /// Gets the item properties from the base class. If not yet instantiated, it will create a new instance using the solution name and web API extension as parameters.
        /// </summary>
        /// <remarks>
        /// This property overrides the base class implementation.
        /// </remarks>
        /// <value>
        /// The item properties object.
        /// </value>
        protected override ItemProperties ItemProperties => _itemProperties ??= new ItemProperties(SolutionProperties.SolutionName, StaticLiterals.WebApiExtension);
        /// <summary>
        /// Gets or sets a value indicating whether to generate models.
        /// </summary>
        public bool GenerateModels { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether controllers should be generated.
        /// </summary>
        /// <value>
        ///   <c>true</c> if controllers should be generated; otherwise, <c>false</c>.
        /// </value>
        public bool GenerateControllers { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether the add services should be generated.
        /// </summary>
        /// <value>
        ///   <c>true</c> if add services should be generated; otherwise, <c>false</c>.
        /// </value>
        public bool GenerateAddServices { get; set; }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="WebApiGenerator"/> class.
        /// </summary>
        /// <param name="solutionProperties">The solution properties.</param>
        /// <remarks>
        /// This constructor is used to create a new instance of the <see cref="WebApiGenerator"/> class with the specified solution properties.
        /// </remarks>
        public WebApiGenerator(ISolutionProperties solutionProperties) : base(solutionProperties)
        {
            GenerateModels = QuerySetting<bool>(ItemType.WebApiModel, "All", StaticLiterals.Generate, "True");
            GenerateControllers = QuerySetting<bool>(ItemType.Controller, "All", StaticLiterals.Generate, "True");
            GenerateAddServices = QuerySetting<bool>(ItemType.AddServices, "All", StaticLiterals.Generate, "True");
        }
        
        /// <summary>
        /// Generates all the required items such as models, controllers, and services.
        /// </summary>
        /// <returns>A collection of generated items.</returns>
        public IEnumerable<IGeneratedItem> GenerateAll()
        {
            var result = new List<IGeneratedItem>();
            
            result.AddRange(CreateModels());
            result.AddRange(CreateControllers());
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
                if (CanCreate(type) && QuerySetting<bool>(ItemType.WebApiModel, type, StaticLiterals.Generate, GenerateModels.ToString()))
                {
                    result.Add(CreateModelFromType(type, UnitType.WebApi, ItemType.WebApiModel));
                    result.Add(CreateModelInheritance(type, UnitType.WebApi, ItemType.WebApiModel));
                    result.Add(CreateEditModelFromType(type, UnitType.WebApi, ItemType.WebApiEditModel));
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
        /// Creates controllers for entity types.
        /// </summary>
        /// <returns>An enumerable collection of generated items.</returns>
        private List<IGeneratedItem> CreateControllers()
        {
            var result = new List<IGeneratedItem>();
            var entityProject = EntityProject.Create(SolutionProperties);
            
            foreach (var type in entityProject.EntityTypes)
            {
                if (CanCreate(type) && QuerySetting<bool>(ItemType.Controller, type, StaticLiterals.Generate, GenerateControllers.ToString()))
                {
                    result.Add(CreateControllerFromType(type, UnitType.WebApi, ItemType.Controller));
                }
            }
            return result;
        }
        /// <summary>
        /// Creates a controller from the specified type.
        /// </summary>
        /// <param name="type">The type of the controller.</param>
        /// <param name="unitType">The unit type.</param>
        /// <param name="itemType">The item type.</param>
        /// <returns>An instance of the IGeneratedItem interface representing the created controller.</returns>
        private GeneratedItem CreateControllerFromType(Type type, UnitType unitType, ItemType itemType)
        {
            var visibility = "public";
            var logicProject = $"{ItemProperties.SolutionName}{StaticLiterals.LogicExtension}";
            var genericType = $"Controllers.GenericController";
            var modelType = ItemProperties.CreateModelType(type);
            var entityType = $"{logicProject}.{ItemProperties.GetModuleSubType(type)}";
            var controllerName = ItemProperties.CreateControllerClassName(type);
            var contractType = ItemProperties.CreateFullCommonModelContractType(type);
            var result = new GeneratedItem(unitType, itemType)
            {
                FullName = $"{ItemProperties.CreateControllerType(type)}",
                FileExtension = StaticLiterals.CSharpFileExtension,
                SubFilePath = ItemProperties.CreateControllersSubPathFromType(type, string.Empty, StaticLiterals.CSharpFileExtension),
            };
            result.Add($"using TModel = {modelType};");
            result.Add($"using TEntity = {entityType};");
            result.Add($"using TContract = {contractType};");
            result.AddRange(CreateComment(type));
            CreateControllerAttributes(type, unitType, itemType, result.Source);
            result.Add($"{visibility} sealed partial class {controllerName}(Contracts.IContextAccessor contextAccessor) : {genericType}<TModel, TEntity, TContract>(contextAccessor)");
            result.Add("{");
            result.AddRange(CreatePartialStaticConstrutor(controllerName));
//            result.AddRange(CreatePartialConstrutor("public", controllerName, $"{contractType} other", "base(other)", null, true));
            
            result.AddRange(CreateComment(type));
            result.Add($"protected override TModel ToModel(TEntity entity)");
            result.Add("{");
            result.Add($"var handled = false;");
            result.Add($"var result = new TModel();");
            result.Add("BeforeToModel(entity, ref result, ref handled);");
            result.Add("if (handled == false)");
            result.Add("{");
            result.Add($"(result as TContract).CopyProperties(entity);");
            result.Add("}");
            result.Add("AfterToModel(result);");
            result.Add($"return result;");
            result.Add("}");

            result.Add($"partial void BeforeToModel(TEntity entity, ref TModel outModel, ref bool handled);");
            result.Add($"partial void AfterToModel(TModel model);");

            result.AddRange(CreateComment(type));
            result.Add($"protected override TEntity ToEntity(TModel model, TEntity? entity)");
            result.Add("{");
            result.Add($"var handled = false;");
            result.Add($"var result = entity ?? new TEntity();");
            result.Add("BeforeToEntity(model, ref result, ref handled);");
            result.Add("if (handled == false)");
            result.Add("{");
            result.Add($"(result as TContract).CopyProperties(model);");
            result.Add("}");
            result.Add("AfterToEntity(result);");
            result.Add($"return result;");
            result.Add("}");

            result.Add($"partial void BeforeToEntity(TModel model, ref TEntity outEntity, ref bool handled);");
            result.Add($"partial void AfterToEntity(TEntity entity);");
            
            result.Add("}");
            result.EnvelopeWithANamespace(ItemProperties.CreateControllerNamespace(type));
            result.FormatCSharpCode();
            return result;
        }

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
                result = (T)Convert.ChangeType(QueryGenerationSettingValue(UnitType.WebApi, itemType, ItemProperties.CreateSubTypeFromEntity(type), valueName, defaultValue), typeof(T));
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
                result = (T)Convert.ChangeType(QueryGenerationSettingValue(UnitType.WebApi, itemType, itemName, valueName, defaultValue), typeof(T));
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
