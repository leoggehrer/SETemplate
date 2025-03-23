//@BaseCode
//MdStart
namespace TemplateTools.Logic.Generation
{
    using System.Reflection;
    using TemplateTools.Logic.Contracts;
    using TemplateTools.Logic.Extensions;
    using TemplateTools.Logic.Models;

    /// <summary>
    /// Represents a common generator that is responsible for generating logic-related code.
    /// </summary>
    internal sealed partial class CommonGenerator : ModelGenerator
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
        protected override ItemProperties ItemProperties => _itemProperties ??= new ItemProperties(SolutionProperties.SolutionName, StaticLiterals.CommonExtension);

        /// <summary>
        /// Gets or sets a value indicating whether all model contracts should be generated.
        /// </summary>
        /// <value>
        ///   <c>true</c> if all model contracts should be generated; otherwise, <c>false</c>.
        /// </value>
        public bool GenerateAllModelContracts { get; set; }
        #endregion properties

        #region constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="LogicGenerator"/> class.
        /// </summary>
        /// <param name="solutionProperties">The solution properties.</param>
        public CommonGenerator(ISolutionProperties solutionProperties) : base(solutionProperties)
        {
            var generateAll = QuerySetting<string>(Common.ItemType.AllItems, StaticLiterals.AllItems, StaticLiterals.Generate, "True");

            GenerateAllModelContracts = QuerySetting<bool>(Common.ItemType.EntityContract, StaticLiterals.AllItems, StaticLiterals.Generate, generateAll);
        }
        #endregion constructors

        #region generations
        /// <summary>
        /// Generates all the required items for the common project.
        /// </summary>
        /// <returns>An enumerable list of generated items.</returns>
        public IEnumerable<IGeneratedItem> GenerateAll()
        {
            var result = new List<IGeneratedItem>();

            result.AddRange(CreateEntityContracts());

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
        /// Creates entity contracts.
        /// </summary>
        /// <returns>An enumerable collection of generated items.</returns>
        private List<GeneratedItem> CreateEntityContracts()
        {
            var result = new List<GeneratedItem>();
            var entityProject = EntityProject.Create(SolutionProperties);

            foreach (var type in entityProject.EntityTypes)
            {
                var defaultValue = (GenerateAllModelContracts && GetGenerateDefault(type)).ToString();

                if (CanCreate(type) && QuerySetting<bool>(Common.ItemType.EntityContract, type, StaticLiterals.Generate, defaultValue))
                {
                    result.Add(CreateEntityContract(type, Common.UnitType.Common, Common.ItemType.EntityContract));
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
        private GeneratedItem CreateEntityContract(Type type, Common.UnitType unitType, Common.ItemType itemType)
        {
            var baseType = type.BaseType;
            var inherit = baseType != null ? (baseType.Name.Equals("EntityObject") ? $" : IIdentifiable" : $" : {ItemProperties.CreateModelContractName(baseType)}") : string.Empty;
            var itemName = ItemProperties.CreateContractName(type);
            var fileName = $"{itemName}{StaticLiterals.CSharpFileExtension}";
            var typeProperties = type.GetAllPropertyInfos();
            var generateProperties = typeProperties.Where(e => StaticLiterals.NoGenerationProperties.Any(p => p.Equals(e.Name)) == false) ?? [];
            var result = new GeneratedItem(unitType, itemType)
            {
                FullName = ItemProperties.CreateFullCommonModelContractType(type),
                FileExtension = StaticLiterals.CSharpFileExtension,
                SubFilePath = ItemProperties.CreateSubFilePath(type, fileName, StaticLiterals.ContractsFolder),
            };

            result.AddRange(CreateComment(type));
            result.Add($"public partial interface {itemName}{inherit}");
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
            // Added copy properties method
            result.AddRange(CreateCopyProperties(string.Empty, type, itemName, pi => pi.IsNavigationProperties() == false));

            result.Add("}");
            result.EnvelopeWithANamespace(ItemProperties.CreateFullCommonNamespace(type, StaticLiterals.ContractsFolder));
            result.FormatCSharpCode();
            return result;
        }
        #endregion generations

        #region query settings
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
                result = (T)Convert.ChangeType(QueryGenerationSettingValue(Common.UnitType.Common, itemType, itemName, valueName, defaultValue), typeof(T));
            }
            catch (Exception ex)
            {
                result = (T)Convert.ChangeType(defaultValue, typeof(T));
                System.Diagnostics.Debug.WriteLine($"Error in {MethodBase.GetCurrentMethod()!.Name}: {ex.Message}");
            }
            return result;
        }
        #endregion query settings
    }
}
//MdEnd
