//@BaseCode
namespace TemplateTools.Logic.Generation
{
    using System.IO;
    using System.Reflection;
    using System.Text;
    using TemplateTools.Logic.Contracts;
    /// <summary>
    /// Represents a class for generating Angular code items.
    /// </summary>
    internal sealed partial class AngularGenerator : ItemGenerator
    {
        #region fields
        private ItemProperties? _itemProperties;
        #endregion fields

        #region properties
        /// <summary>
        /// Gets or sets the ItemProperties for the current instance.
        /// </summary>
        protected override ItemProperties ItemProperties => _itemProperties ??= new ItemProperties(SolutionProperties.SolutionName, StaticLiterals.CommonExtension);
        #endregion properties

        /// <summary>
        /// Gets or sets a value indicating whether to generate enums.
        /// </summary>
        public bool GenerateEnums { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether models should be generated.
        /// </summary>
        public bool GenerateModels { get; set; }
        ///<summary>
        /// Gets or sets a value indicating whether services should be generated.
        ///</summary>
        public bool GenerateServices { get; set; }

        #region AngularApp-Definitions
        public static string SourcePath => Path.Combine("src", "app");
        /// <summary>
        /// Gets the subfolder path for the generated enums.
        /// </summary>
        public static string EnumsSubFolder => Path.Combine(SourcePath);
        /// <summary>
        /// Gets the subfolder path for models under the core app.
        /// </summary>
        public static string ModelsSubFolder => Path.Combine(SourcePath, "models");
        /// <summary>
        ///     Gets the subfolder path for the services in the application's core.
        /// </summary>
        public static string ServicesSubFolder => Path.Combine(SourcePath, "services", "http");
        /// <summary>
        ///     Gets the subfolder path for the components in the application's core.
        /// </summary>
        public static string ComponentsSubFolder => Path.Combine(SourcePath, "components");
        /// <summary>
        ///     Gets the subfolder path for the pages in the application's core.
        /// </summary>
        public static string PagesSubFolder => Path.Combine(SourcePath, "pages");
        /// <summary>
        /// Gets or sets the source namespace.
        /// </summary>
        public static string SourceNameSpace => "src";
        /// <summary>
        /// Gets the namespace for the contracts.
        /// </summary>
        public static string ContractsNameSpace => $"{SourceNameSpace}.contracts";
        /// <summary>
        /// Creates the namespace for the contracts based on the provided <paramref name="type"/>.
        /// </summary>
        /// <param name="type">The type used to generate the subnamespace.</param>
        /// <returns>The fully qualified namespace for the contracts.</returns>
        public static string CreateContractsNameSpace(Type type)
        {
            return $"{ContractsNameSpace}.{ItemProperties.CreateSubNamespaceFromType(type)}".ToLower();
        }
        /// <summary>
        /// Creates a fully qualified TypeScript name for a given type.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> object representing the type.</param>
        /// <returns>A string representing the full TypeScript name of the type.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="type"/> is null.</exception>
        public static string CreateTypeScriptModelFullName(Type type)
        {
            type.CheckArgument(nameof(type));

            return $"{CreateContractsNameSpace(type)}.{ItemProperties.CreateTSModelName(type)}";
        }
        /// <summary>
        /// Creates a fully qualified TypeScript name for a given type.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> object representing the type.</param>
        /// <returns>A string representing the full TypeScript name of the type.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="type"/> is null.</exception>
        public static string CreateTypeScriptComponentFullName(Type type)
        {
            type.CheckArgument(nameof(type));

            return $"{CreateContractsNameSpace(type)}.{ItemProperties.CreateEntityName(type)}";
        }
        #endregion AngularApp-Definitions

        /// <summary>
        /// Initializes a new instance of the <see cref="AngularGenerator"/> class.
        /// </summary>
        /// <param name="solutionProperties">The solution properties.</param>
        public AngularGenerator(ISolutionProperties solutionProperties) : base(solutionProperties)
        {
            GenerateEnums = QuerySetting<bool>(Common.ItemType.TypeScriptEnum, "All", StaticLiterals.Generate, "True");
            GenerateModels = QuerySetting<bool>(Common.ItemType.TypeScriptModel, "All", StaticLiterals.Generate, "True");
            GenerateServices = QuerySetting<bool>(Common.ItemType.TypeScriptService, "All", StaticLiterals.Generate, "True");
        }

        /// <summary>
        /// Generates all the generated items, including enums, models, and services.
        /// </summary>
        /// <returns>An enumerable collection of the generated items.</returns>
        public IEnumerable<IGeneratedItem> GenerateAll()
        {
            var result = new List<IGeneratedItem>();

            result.AddRange(CreateEnums());
            result.AddRange(CreateModels());
            result.AddRange(CreateServices());
            result.AddRange(CreateComponents());
            return result;
        }
        /// <summary>
        /// Creates a collection of generated items representing enums.
        /// </summary>
        /// <returns>A collection of generated items representing enums.</returns>
        public IEnumerable<IGeneratedItem> CreateEnums()
        {
            var result = new List<IGeneratedItem>();
            var entityProject = EntityProject.Create(SolutionProperties);

            foreach (var type in entityProject.EnumTypes)
            {
                if (CanCreate(type)
                && QuerySetting<bool>(Common.ItemType.TypeScriptEnum, type, StaticLiterals.Generate, GenerateEnums.ToString()))
                {
                    result.Add(CreateEnumFromType(type));
                }
            }
            return result;
        }
        /// <summary>
        /// Creates a TypeScript enum from a specified System.Type object.
        /// </summary>
        /// <param name="type">The System.Type object to create the enum from.</param>
        /// <returns>An instance of the IGeneratedItem interface representing the created enum.</returns>
        public IGeneratedItem CreateEnumFromType(Type type)
        {
            var subPath = ConvertPathItem(ItemProperties.CreateSubPathFromType(type));
            var projectPath = Path.Combine(SolutionProperties.SolutionPath, SolutionProperties.AngularAppProjectName);
            var fileName = $"{ConvertFileItem(type.Name)}{StaticLiterals.TSFileExtension}";
            var result = new Models.GeneratedItem(Common.UnitType.AngularApp, Common.ItemType.TypeScriptEnum)
            {
                FullName = CreateTypeScriptModelFullName(type),
                FileExtension = StaticLiterals.TSFileExtension,
                SubFilePath = Path.Combine(EnumsSubFolder, subPath, fileName),
            };

            StartCreateEnum(type, result.Source);
            result.Add($"export enum {type.Name}" + " {");

            foreach (var item in Enum.GetNames(type))
            {
                var value = Enum.Parse(type, item);

                result.Add($"{item} = {(int)value},");
            }

            result.Add("}");

            result.Source.Insert(result.Source.Count - 1, StaticLiterals.CustomCodeBeginLabel);
            result.Source.InsertRange(result.Source.Count - 1, ReadCustomCode(projectPath, result));
            result.Source.Insert(result.Source.Count - 1, StaticLiterals.CustomCodeEndLabel);

            result.AddRange(result.Source.Eject().Distinct());
            result.FormatCSharpCode();
            FinishCreateEnum(type, result.Source);
            return result;
        }
        /// <summary>
        ///   Starts the creation of an enum based on the provided <paramref name="type"/> and <paramref name="lines"/>.
        /// </summary>
        /// <param name="type">The <see cref="System.Type"/> representing the enum type.</param>
        /// <param name="lines">The list of string values representing the enum values.</param>
        /// <remarks>
        ///    This method is used to initiate the creation of an enum by providing the enum <paramref name="type"/> and relevant <paramref name="lines"/>.
        ///    It should be implemented partially by other methods which handle the actual creation process.
        /// </remarks>
        /// <exception cref="System.ArgumentNullException">
        ///   Thrown if <paramref name="type"/> is null.
        ///   Thrown if <paramref name="lines"/> is null.
        /// </exception>
        partial void StartCreateEnum(Type type, List<string> lines);
        /// <summary>
        /// Concludes the process of creating an enumeration by performing additional operations on the generated code.
        /// </summary>
        /// <param name="type">The Type object representing the enumeration.</param>
        /// <param name="lines">A List of strings containing the generated code lines for the enumeration.</param>
        /// <remarks>
        /// This method is intended for internal use within the code generation process and should not be called directly.
        /// </remarks>
        /// <seealso cref="CreateEnum(Type)"/>
        /// <seealso cref="GenerateCode(Type)"/>f
        partial void FinishCreateEnum(Type type, List<string> lines);

        /// <summary>
        /// Creates models based on entity types in the entity project.
        /// </summary>
        /// <returns>An enumerable collection of generated items.</returns>
        public IEnumerable<IGeneratedItem> CreateModels()
        {
            var result = new List<IGeneratedItem>();
            var entityProject = EntityProject.Create(SolutionProperties);

            result.Add(CreateBaseModel());
            foreach (var type in entityProject.AllEntityTypes)
            {
                if (CanCreate(type)
                    && QuerySetting<bool>(Common.ItemType.TypeScriptModel, type, StaticLiterals.Generate, GenerateModels.ToString()))
                {
                    result.Add(CreateModelFromType(type, entityProject.EntityTypes));
                }
            }

            foreach (var type in entityProject.AllViewTypes)
            {
                if (CanCreate(type)
                    && QuerySetting<bool>(Common.ItemType.TypeScriptModel, type, StaticLiterals.Generate, GenerateModels.ToString()))
                {
                    result.Add(CreateModelFromType(type, entityProject.EntityTypes));
                }
            }
            return result;
        }

        /// <summary>
        /// Creates the base TypeScript model interface for key models.
        /// </summary>
        /// <remarks>
        /// Generates the <c>IKeyModel</c> interface and related type definitions for use in Angular models.
        /// The generated interface extends <c>IModel</c> and defines the key property as either <c>id</c> (number)
        /// or <c>guid</c> (string), depending on the <c>EXTERNALGUID_ON</c> compilation symbol.
        /// </remarks>
        /// <returns>
        /// An <see cref="IGeneratedItem"/> representing the generated base model TypeScript code.
        /// </returns>
        public IGeneratedItem CreateBaseModel()
        {
            var modelName = "IKeyModel";
            var fileName = $"{ConvertFileItem(modelName)}{StaticLiterals.TSFileExtension}";
            var declarationTypeName = string.Empty;
            var result = new Models.GeneratedItem(Common.UnitType.AngularApp, Common.ItemType.TypeScriptModel)
            {
                FullName = modelName,
                FileExtension = StaticLiterals.TSFileExtension,
                SubFilePath = Path.Combine(ModelsSubFolder, fileName),
            };

            result.Add("import { IModel } from \"@app-models/i-model\";");
            result.Add(string.Empty);

#if EXTERNALGUID_ON
            result.Add("export type IdType = string;");
            result.Add("export const IdDefault: IdType = '00000000-0000-0000-0000-000000000000';");
            result.Add(string.Empty);
            result.Add("export interface IKeyModel extends IModel {");
            result.Add("  guid: IdType;");
            result.Add("}");
#elif IDGUID_ON
            result.Add("export type IdType = string;");
            result.Add("export const IdDefault: IdType = '00000000-0000-0000-0000-000000000000';");
            result.Add(string.Empty);
            result.Add("export interface IKeyModel extends IModel {");
            result.Add("  id: IdType;");
            result.Add("}");
#else
            result.Add("export type IdType = number;");
            result.Add("export const IdDefault: IdType = 0;");
            result.Add(string.Empty);
            result.Add("export interface IKeyModel extends IModel {");
            result.Add("  id: IdType;");
            result.Add("}");
#endif
            return result;
        }
        /// <summary>
        /// Creates a TypeScript model from a given C# type.
        /// </summary>
        /// <param name="type">The C# type to create the model from.</param>
        /// <param name="types">Additional types used for property generation.</param>
        /// <returns>The generated TypeScript model.</returns>
        public IGeneratedItem CreateModelFromType(Type type, IEnumerable<Type> types)
        {
            static string GetBaseClassByType(Type type)
            {
                var result = "object";
                var found = false;
                var runType = type.BaseType;

                while (runType != null && found == false)
                {
                    if (StaticLiterals.AngularBaseClassMapping.TryGetValue(runType.Name, out string? value))
                    {
                        found = true;
                        result = value;
                    }
                    runType = runType.BaseType;
                }
                return result;
            }

            var subPath = ConvertPathItem(ItemProperties.CreateSubPathFromType(type));
            var projectPath = Path.Combine(SolutionProperties.SolutionPath, SolutionProperties.AngularAppProjectName);
            var modelName = ItemProperties.CreateTSModelName(type);
            var modelBaseType = GetBaseClassByType(type);
            var fileName = $"{ConvertFileItem(modelName)}{StaticLiterals.TSFileExtension}";
            var typeProperties = type.GetAllPropertyInfos();
            var generationProperties = typeProperties.Where(e => StaticLiterals.NoGenerationProperties.Any(p => p.Equals(e.Name)) == false) ?? [];
            var declarationTypeName = string.Empty;
            var result = new Models.GeneratedItem(Common.UnitType.AngularApp, Common.ItemType.TypeScriptModel)
            {
                FullName = CreateTypeScriptModelFullName(type),
                FileExtension = StaticLiterals.TSFileExtension,
                SubFilePath = Path.Combine(ModelsSubFolder, subPath, fileName),
            };

            StartCreateModel(type, result.Source);
            result.Add($"export interface {modelName} extends {modelBaseType}" + " {");

            foreach (var item in generationProperties)
            {
                if (declarationTypeName.Equals(item.DeclaringType!.Name) == false)
                {
                    declarationTypeName = item.DeclaringType.Name;
                }
                result.AddRange(CreateTypeScriptProperty(item, types));
            }

            result.Add("}");

            result.Source.Insert(result.Source.Count - 1, StaticLiterals.CustomCodeBeginLabel);
            result.Source.InsertRange(result.Source.Count - 1, ReadCustomCode(projectPath, result));
            result.Source.Insert(result.Source.Count - 1, StaticLiterals.CustomCodeEndLabel);

#pragma warning disable IDE0028 // Simplify collection initialization
            var imports = new List<string>();
#pragma warning restore IDE0028 // Simplify collection initialization

            imports.Add("import { " + $"{modelBaseType}" + " } from '@app-models/" + $"{ConvertFileItem(modelBaseType)}';");
            imports.AddRange(CreateTypeImports(type, types));
            imports.AddRange(CreateModelToModelImports(type, types));
            imports.Add(StaticLiterals.CustomImportBeginLabel);
            imports.AddRange(ReadCustomImports(projectPath, result));
            imports.Add(StaticLiterals.CustomImportEndLabel);

            InsertTypeImports(imports, result.Source);
            FinishCreateModel(type, result.Source);
            return result;
        }
        /// <summary>
        /// Starts the creation of a model.
        /// </summary>
        /// <param name="type">The type of the model.</param>
        /// <param name="lines">The list of strings used to create the model.</param>
        /// <remarks>
        /// This method is declared as 'partial' which means it can be implemented in separate files,
        /// allowing for modular code organization.
        /// </remarks>
        partial void StartCreateModel(Type type, List<string> lines);
        /// <summary>
        /// Finishes creating the model with the provided <paramref name="type"/> and <paramref name="lines"/>.
        /// </summary>
        /// <param name="type">The specified type used for creating the model.</param>
        /// <param name="lines">The list of strings used for creating the model.</param>
        /// <remarks>
        /// This method is partially implemented and needs to be completed in another class file.
        /// </remarks>
        partial void FinishCreateModel(Type type, List<string> lines);

        /// <summary>
        /// Creates a collection of generated items representing services.
        /// </summary>
        /// <returns>A collection of <see cref="IGeneratedItem"/> representing services.</returns>
        private List<Models.GeneratedItem> CreateServices()
        {
            var result = new List<Models.GeneratedItem>();
            var entityProject = EntityProject.Create(SolutionProperties);

            foreach (var type in entityProject.AllEntityTypes)
            {
                if (CanCreate(type) && QuerySetting<bool>(Common.ItemType.TypeScriptService, type, StaticLiterals.Generate, GenerateServices.ToString()))
                {
                    result.Add(CreateEntityServiceFromType(type, Common.UnitType.AngularApp, Common.ItemType.TypeScriptService));
                }
            }
            foreach (var type in entityProject.AllViewTypes)
            {
                if (CanCreate(type) && QuerySetting<bool>(Common.ItemType.TypeScriptService, type, StaticLiterals.Generate, GenerateServices.ToString()))
                {
                    result.Add(CreateViewServiceFromType(type, Common.UnitType.AngularApp, Common.ItemType.TypeScriptService));
                }
            }
            return result;
        }
        /// <summary>
        /// Creates a service from the specified type.
        /// </summary>
        /// <param name="type">The type from which the service is created.</param>
        /// <param name="unitType">The unit type.</param>
        /// <param name="itemType">The item type.</param>
        /// <returns>The generated item representing the service.</returns>
        private Models.GeneratedItem CreateEntityServiceFromType(Type type, Common.UnitType unitType, Common.ItemType itemType)
        {
            var subPath = ConvertPathItem(ItemProperties.CreateSubPathFromType(type));
            var projectPath = Path.Combine(SolutionProperties.SolutionPath, SolutionProperties.AngularAppProjectName);
            var entityName = ItemProperties.CreateEntityName(type);
            var modelName = ItemProperties.CreateTSModelName(type);
            var fileName = $"{ConvertFileItem($"{entityName}Service")}{StaticLiterals.TSFileExtension}";
            var result = new Models.GeneratedItem(unitType, itemType)
            {
                FullName = CreateTypeScriptModelFullName(type),
                FileExtension = StaticLiterals.TSFileExtension,
                SubFilePath = Path.Combine(ServicesSubFolder, subPath, fileName),
            };

            StartCreateService(type, result.Source);
            result.Add("import { IdType, IdDefault } from '@app-models/i-key-model';");
            result.Add("import { Injectable } from '@angular/core';");
            result.Add("import { HttpClient } from '@angular/common/http';");
            result.Add("import { ApiEntityBaseService } from '@app-services/api-entity-base.service';");
            result.Add("import { environment } from '@environment/environment';");
            result.Add(CreateImport("@app-models", modelName, subPath));

            result.Add(StaticLiterals.CustomImportBeginLabel);
            result.AddRange(ReadCustomImports(projectPath, result));
            result.Add(StaticLiterals.CustomImportEndLabel);

            result.Add("@Injectable({");
            result.Add("  providedIn: 'root',");
            result.Add("})");
            result.Add($"export class {entityName}Service extends ApiEntityBaseService<{modelName}>" + " {");
            result.Add("  constructor(public override http: HttpClient) {");
            result.Add($"    super(http, environment.API_BASE_URL + '/{entityName.CreatePluralWord().ToLower()}');");
            result.Add("  }");
            result.Add(string.Empty);
#if EXTERNALGUID_ON
            result.Add($"  public override getItemKey(item: {modelName}): IdType " + "{");
            result.Add($"    return item?.guid || IdDefault;");
            result.Add("  }");
#else
            result.Add($"  public override getItemKey(item: {modelName}): IdType " + "{");
            result.Add($"    return item?.id || IdDefault;");
            result.Add("  }");
#endif
            result.Add(string.Empty);
            result.Add("}");

            result.Source.Insert(result.Source.Count - 1, StaticLiterals.CustomCodeBeginLabel);
            result.Source.InsertRange(result.Source.Count - 1, ReadCustomCode(projectPath, result));
            result.Source.Insert(result.Source.Count - 1, StaticLiterals.CustomCodeEndLabel);
            FinishCreateService(type, result.Source);
            return result;
        }

        /// <summary>
        /// Creates a service from the specified type.
        /// </summary>
        /// <param name="type">The type from which the service is created.</param>
        /// <param name="unitType">The unit type.</param>
        /// <param name="itemType">The item type.</param>
        /// <returns>The generated item representing the service.</returns>
        private Models.GeneratedItem CreateViewServiceFromType(Type type, Common.UnitType unitType, Common.ItemType itemType)
        {
            var subPath = ConvertPathItem(ItemProperties.CreateSubPathFromType(type));
            var projectPath = Path.Combine(SolutionProperties.SolutionPath, SolutionProperties.AngularAppProjectName);
            var entityName = ItemProperties.CreateEntityName(type);
            var modelName = ItemProperties.CreateTSModelName(type);
            var fileName = $"{ConvertFileItem($"{entityName}Service")}{StaticLiterals.TSFileExtension}";
            var result = new Models.GeneratedItem(unitType, itemType)
            {
                FullName = CreateTypeScriptModelFullName(type),
                FileExtension = StaticLiterals.TSFileExtension,
                SubFilePath = Path.Combine(ServicesSubFolder, subPath, fileName),
            };

            StartCreateService(type, result.Source);
            result.Add("import { Injectable } from '@angular/core';");
            result.Add("import { HttpClient } from '@angular/common/http';");
            result.Add("import { ApiViewBaseService } from '@app-services/api-view-base.service';");
            result.Add("import { environment } from '@environment/environment';");
            result.Add(CreateImport("@app-models", modelName, subPath));

            result.Add(StaticLiterals.CustomImportBeginLabel);
            result.AddRange(ReadCustomImports(projectPath, result));
            result.Add(StaticLiterals.CustomImportEndLabel);

            result.Add("@Injectable({");
            result.Add("  providedIn: 'root',");
            result.Add("})");
            result.Add($"export class {entityName}Service extends ApiViewBaseService<{modelName}>" + " {");
            result.Add("  constructor(public override http: HttpClient) {");
            result.Add($"    super(http, environment.API_BASE_URL + '/{entityName.CreatePluralWord().ToLower()}');");
            result.Add("  }");
            result.Add("}");

            result.Source.Insert(result.Source.Count - 1, StaticLiterals.CustomCodeBeginLabel);
            result.Source.InsertRange(result.Source.Count - 1, ReadCustomCode(projectPath, result));
            result.Source.Insert(result.Source.Count - 1, StaticLiterals.CustomCodeEndLabel);
            FinishCreateService(type, result.Source);
            return result;
        }

        /// <summary>
        /// Starts the process of creating a service of the specified type, using the given list of lines.
        /// </summary>
        /// <param name="type">The type of service to be created.</param>
        /// <param name="lines">The list of lines to be used for creating the service.</param>
        partial void StartCreateService(Type type, List<string> lines);
        /// <summary>
        /// FinishCreateService is a method that finishes the creation of a service with the specified type and lines.
        /// </summary>
        /// <param name="type">The Type object representing the type of the service.</param>
        /// <param name="lines">A List of strings containing the lines of the service.</param>
        partial void FinishCreateService(Type type, List<string> lines);

        /// <summary>
        /// Creates a filter string for use in Angular list components based on the properties of the specified type.
        /// The filter string is used to filter list items in the UI, typically in the ngOnInit method.
        /// For string properties, it generates a filter using the 'ToLower().Contains(@0)' pattern.
        /// For numeric properties, it generates a filter using the '== @0' pattern.
        /// Only properties for which the filter property setting is enabled will be included.
        /// </summary>
        /// <param name="type">The type whose properties are used to generate the filter string.</param>
        /// <param name="unitType">The unit type used for querying settings.</param>
        /// <returns>A filter string combining all eligible properties, joined by 'OR'.</returns>
        private string CreateListFilterFromType(Type type, Common.UnitType unitType)
        {
            var sbToString = new StringBuilder();
            var viewProperties = GetViewProperties(type);

            foreach (var pi in viewProperties)
            {
                var canGenerate = QuerySetting<bool>(unitType, Common.ItemType.FilterProperty, $"{ItemProperties.CreateSubTypeFromEntity(type)}Filter.{pi.Name}", StaticLiterals.Generate, "True");

                if (canGenerate)
                {
                    if (pi.PropertyType == typeof(string))
                    {
                        if (sbToString.Length > 0)
                        {
                            sbToString.Append(" OR ");
                        }
                        sbToString.Append($"{ItemProperties.CreateTSPropertyName(pi)}.ToLower().Contains(@0)");
                    }
                    //else if (pi.PropertyType.IsNumericType() && pi.Name.EndsWith("Id", StringComparison.OrdinalIgnoreCase) == false)
                    //{
                    //    if (sbToString.Length > 0)
                    //    {
                    //        sbToString.Append(" OR ");
                    //    }
                    //    sbToString.Append($"{ItemProperties.CreateTSPropertyName(pi)} == @0");
                    //}
                    //else if (pi.PropertyType.IsEnum)
                    //{
                    //    if (sbToString.Length > 0)
                    //    {
                    //        sbToString.Append(" OR ");
                    //    }
                    //    sbToString.Append($"{ItemProperties.CreateTSPropertyName(pi)} == @0");
                    //}
                }
            }

            return sbToString.ToString();
        }
        private List<Models.GeneratedItem> CreateComponents()
        {
            var result = new List<Models.GeneratedItem>();
            var entityProject = EntityProject.Create(SolutionProperties);

            foreach (var type in entityProject.AllEntityTypes)
            {
                if (CanCreate(type) && QuerySetting<bool>(Common.ItemType.TypeScriptListComponent, type, StaticLiterals.Generate, GenerateServices.ToString()))
                {
                    result.Add(CreateEntityBaseListComponentFromType(type, Common.UnitType.AngularApp, Common.ItemType.TypeScriptListComponent));
                }
            }
            foreach (var type in entityProject.AllEntityTypes)
            {
                if (CanCreate(type) && QuerySetting<bool>(Common.ItemType.TypeScriptEditComponent, type, StaticLiterals.Generate, GenerateServices.ToString()))
                {
                    result.Add(CreateEntityBaseEditComponentFromType(type, Common.UnitType.AngularApp, Common.ItemType.TypeScriptEditComponent));
                }
            }
            foreach (var type in entityProject.AllViewTypes)
            {
                if (CanCreate(type) && QuerySetting<bool>(Common.ItemType.TypeScriptListComponent, type, StaticLiterals.Generate, GenerateServices.ToString()))
                {
                    result.Add(CreateViewBaseListComponentFromType(type, Common.UnitType.AngularApp, Common.ItemType.TypeScriptListComponent));
                }
            }

            foreach (var type in entityProject.AllEntityTypes)
            {
                if (CanCreate(type) && QuerySetting<bool>(Common.ItemType.TypeScriptPageListComponent, type, StaticLiterals.Generate, GenerateServices.ToString()))
                {
                    result.Add(CreateEntityListComponentFromType(type, Common.UnitType.AngularApp, Common.ItemType.TypeScriptPageListComponent));
                    result.Add(CreateEntityListHtmlComponentFromType(type, Common.UnitType.AngularApp, Common.ItemType.TypeScriptPageListComponent));
                    result.Add(CreateEntityListCssComponentFromType(type, Common.UnitType.AngularApp, Common.ItemType.TypeScriptPageListComponent));

                    result.Add(CreateEntityEditComponentFromType(type, Common.UnitType.AngularApp, Common.ItemType.TypeScriptEditComponent));
                    result.Add(CreateEntityEditHtmlComponentFromType(type, Common.UnitType.AngularApp, Common.ItemType.TypeScriptEditComponent));
                    result.Add(CreateEntityEditCssComponentFromType(type, Common.UnitType.AngularApp, Common.ItemType.TypeScriptEditComponent));
                }
            }

            foreach (var type in entityProject.AllViewTypes)
            {
                if (CanCreate(type) && QuerySetting<bool>(Common.ItemType.TypeScriptPageListComponent, type, StaticLiterals.Generate, GenerateServices.ToString()))
                {
                    result.Add(CreateViewListComponentFromType(type, Common.UnitType.AngularApp, Common.ItemType.TypeScriptPageListComponent));
                    result.Add(CreateViewListHtmlComponentFromType(type, Common.UnitType.AngularApp, Common.ItemType.TypeScriptPageListComponent));
                    result.Add(CreateViewListCssComponentFromType(type, Common.UnitType.AngularApp, Common.ItemType.TypeScriptPageListComponent));
                }
            }
            return result;
        }
        /// <summary>
        /// Creates a generated item representing an Angular entity list component from the specified type.
        /// </summary>
        /// <param name="type">The entity type for which the list component is generated.</param>
        /// <param name="unitType">The unit type for the generated item.</param>
        /// <param name="itemType">The item type for the generated item.</param>
        /// <returns>
        /// A <see cref="Models.GeneratedItem"/> containing the generated TypeScript code for the entity list component.
        /// </returns>
        private Models.GeneratedItem CreateEntityBaseListComponentFromType(Type type, Common.UnitType unitType, Common.ItemType itemType)
        {
            var subPath = ConvertPathItem(ItemProperties.CreateSubPathFromType(type));
            var projectPath = Path.Combine(SolutionProperties.SolutionPath, SolutionProperties.AngularAppProjectName);
            var entityName = ItemProperties.CreateEntityName(type);
            var modelName = ItemProperties.CreateTSModelName(type);
            var serviceName = $"{entityName}Service";
            var fileName = $"{ConvertFileItem($"{entityName}BaseList")}.component{StaticLiterals.TSFileExtension}";
            var result = new Models.GeneratedItem(unitType, itemType)
            {
                FullName = CreateTypeScriptModelFullName(type),
                FileExtension = StaticLiterals.TSFileExtension,
                SubFilePath = Path.Combine(ComponentsSubFolder, subPath, fileName),
            };

            StartCreateListComponent(type, result.Source);
            result.Add("import { Directive, OnInit } from '@angular/core';");
            result.Add("import { GenericEntityListComponent } from '@app/components/base/generic-entity-list.component';");
            result.Add(CreateImport("@app-models", modelName, subPath));
            result.Add(CreateImport("@app-services/http", serviceName, subPath));

            result.Add(StaticLiterals.CustomImportBeginLabel);
            result.AddRange(ReadCustomImports(projectPath, result));
            result.Add(StaticLiterals.CustomImportEndLabel);

            result.Add("@Directive()");
            result.Add($"export abstract class {entityName}BaseListComponent extends GenericEntityListComponent<{modelName}> implements OnInit" + " {");
            result.Add("  constructor(");
            result.Add($"              protected dataAccessService: {serviceName}");
            result.Add($"            )");
            result.Add("  {");
            result.Add("    super(dataAccessService);");
            result.Add("  }");

            result.Add("  ngOnInit(): void {");
            result.Add("    this.reloadData();");
            result.Add("  }");

            result.Add("}");

            result.Source.Insert(result.Source.Count - 1, StaticLiterals.CustomCodeBeginLabel);
            result.Source.InsertRange(result.Source.Count - 1, ReadCustomCode(projectPath, result));
            result.Source.Insert(result.Source.Count - 1, StaticLiterals.CustomCodeEndLabel);
            FinishCreateListComponent(type, result.Source);
            return result;
        }
        /// <summary>
        /// Creates a generated item representing an Angular entity edit component from the specified type.
        /// </summary>
        /// <param name="type">The entity type for which the edit component is generated.</param>
        /// <param name="unitType">The unit type for the generated item.</param>
        /// <param name="itemType">The item type for the generated item.</param>
        /// <returns>
        /// A <see cref="Models.GeneratedItem"/> containing the generated TypeScript code for the entity edit component.
        /// </returns>
        private Models.GeneratedItem CreateEntityBaseEditComponentFromType(Type type, Common.UnitType unitType, Common.ItemType itemType)
        {
            var subPath = ConvertPathItem(ItemProperties.CreateSubPathFromType(type));
            var projectPath = Path.Combine(SolutionProperties.SolutionPath, SolutionProperties.AngularAppProjectName);
            var entityName = ItemProperties.CreateEntityName(type);
            var modelName = ItemProperties.CreateTSModelName(type);
            var fileName = $"{ConvertFileItem($"{entityName}BaseEdit")}.component{StaticLiterals.TSFileExtension}";
            var result = new Models.GeneratedItem(unitType, itemType)
            {
                FullName = CreateTypeScriptModelFullName(type),
                FileExtension = StaticLiterals.TSFileExtension,
                SubFilePath = Path.Combine(ComponentsSubFolder, subPath, fileName),
            };

            StartCreateListComponent(type, result.Source);
            result.Add("import { Directive, OnInit } from '@angular/core';");
            result.Add("import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';");
            result.Add("import { IdType, IdDefault, IKeyModel } from '@app/models/i-key-model';");
            result.Add("import { GenericEditComponent } from '@app/components/base/generic-edit.component';");
            result.Add(CreateImport("@app-models", modelName, subPath));

            result.Add(StaticLiterals.CustomImportBeginLabel);
            result.AddRange(ReadCustomImports(projectPath, result));
            result.Add(StaticLiterals.CustomImportEndLabel);

            result.Add("@Directive()");
            result.Add($"export abstract class {entityName}BaseEditComponent extends GenericEditComponent<{modelName}> implements OnInit" + " {");
            result.Add("  constructor(public override activeModal: NgbActiveModal)");
            result.Add("  {");
            result.Add("    super(activeModal);");
            result.Add("  }");

            result.Add("  ngOnInit(): void {");
            result.Add("  }");
            result.Add(string.Empty);
#if EXTERNALGUID_ON
            result.Add($"  public override getItemKey(item: {modelName}): IdType " + "{");
            result.Add($"    return item?.guid || IdDefault;");
            result.Add("  }");
#else
            result.Add($"  public override getItemKey(item: {modelName}): IdType " + "{");
            result.Add($"    return item?.id || IdDefault;");
            result.Add("  }");
#endif
            result.Add(string.Empty);
            result.Add("  public override get title(): string {");
            result.Add($"    return '{entityName}' + super.title;");
            result.Add("  }");

            result.Add("}");

            result.Source.Insert(result.Source.Count - 1, StaticLiterals.CustomCodeBeginLabel);
            result.Source.InsertRange(result.Source.Count - 1, ReadCustomCode(projectPath, result));
            result.Source.Insert(result.Source.Count - 1, StaticLiterals.CustomCodeEndLabel);
            FinishCreateListComponent(type, result.Source);
            return result;
        }
        /// <summary>
        /// Creates a generated item representing an Angular view list component from the specified type.
        /// </summary>
        /// <param name="type">The view type for which the list component is generated.</param>
        /// <param name="unitType">The unit type for the generated item.</param>
        /// <param name="itemType">The item type for the generated item.</param>
        /// <returns>
        /// A <see cref="Models.GeneratedItem"/> containing the generated TypeScript code for the view list component.
        /// </returns>
        private Models.GeneratedItem CreateViewBaseListComponentFromType(Type type, Common.UnitType unitType, Common.ItemType itemType)
        {
            var subPath = ConvertPathItem(ItemProperties.CreateSubPathFromType(type));
            var projectPath = Path.Combine(SolutionProperties.SolutionPath, SolutionProperties.AngularAppProjectName);
            var entityName = ItemProperties.CreateEntityName(type);
            var modelName = ItemProperties.CreateTSModelName(type);
            var serviceName = $"{entityName}Service";
            var fileName = $"{ConvertFileItem($"{entityName}BaseList")}.component{StaticLiterals.TSFileExtension}";
            var result = new Models.GeneratedItem(unitType, itemType)
            {
                FullName = CreateTypeScriptModelFullName(type),
                FileExtension = StaticLiterals.TSFileExtension,
                SubFilePath = Path.Combine(ComponentsSubFolder, subPath, fileName),
            };

            StartCreateListComponent(type, result.Source);
            result.Add("import { Directive, OnInit } from '@angular/core';");
            result.Add("import { GenericViewListComponent } from '@app/components/base/generic-view-list.component';");
            result.Add(CreateImport("@app-models", modelName, subPath));
            result.Add(CreateImport("@app-services/http", serviceName, subPath));

            result.Add(StaticLiterals.CustomImportBeginLabel);
            result.AddRange(ReadCustomImports(projectPath, result));
            result.Add(StaticLiterals.CustomImportEndLabel);

            result.Add("@Directive()");
            result.Add($"export abstract class {entityName}BaseListComponent extends GenericViewListComponent<{modelName}> implements OnInit" + " {");
            result.Add("  constructor(");
            result.Add($"              protected dataAccessService: {serviceName}");
            result.Add("             )");
            result.Add("  {");
            result.Add("    super(dataAccessService);");
            result.Add("  }");

            result.Add("  ngOnInit(): void {");
            result.Add("    this.reloadData();");
            result.Add("  }");

            result.Add("}");

            result.Source.Insert(result.Source.Count - 1, StaticLiterals.CustomCodeBeginLabel);
            result.Source.InsertRange(result.Source.Count - 1, ReadCustomCode(projectPath, result));
            result.Source.Insert(result.Source.Count - 1, StaticLiterals.CustomCodeEndLabel);
            FinishCreateListComponent(type, result.Source);
            return result;
        }

        /// <summary>
        /// Creates a generated item representing an Angular entity page list component from the specified type.
        /// </summary>
        /// <param name="type">The entity type for which the page list component is generated.</param>
        /// <param name="unitType">The unit type for the generated item.</param>
        /// <param name="itemType">The item type for the generated item.</param>
        /// <returns>
        /// A <see cref="Models.GeneratedItem"/> containing the generated TypeScript code for the entity page list component.
        /// </returns>
        private Models.GeneratedItem CreateEntityListComponentFromType(Type type, Common.UnitType unitType, Common.ItemType itemType)
        {
            var subPath = ConvertPathItem(ItemProperties.CreateSubPathFromType(type));
            var projectPath = Path.Combine(SolutionProperties.SolutionPath, SolutionProperties.AngularAppProjectName);
            var entityName = ItemProperties.CreateEntityName(type);
            var modelName = ItemProperties.CreateTSModelName(type);
            var serviceName = $"{entityName}Service";
            var fileName = $"{ConvertFileItem($"{entityName}List")}.component{StaticLiterals.TSFileExtension}";
            var baseFileName = $"{ConvertFileItem($"{entityName}BaseList")}.component";
            var baseSubFilePath = ConvertFileItem(Path.Combine(subPath, baseFileName));
            var editFileName = $"{ConvertFileItem($"{entityName}Edit")}.component";
            var editSubFilePath = ConvertFileItem(Path.Combine(subPath, editFileName));

            var result = new Models.GeneratedItem(unitType, itemType)
            {
                HasDefaultLabel = false,
                SpecialLabel = CommonStaticLiterals.CustomCodeLabel,
                FullName = CreateTypeScriptComponentFullName(type),
                FileExtension = StaticLiterals.TSFileExtension,
                SubFilePath = Path.Combine(PagesSubFolder, subPath, fileName),
            };

            StartCreateListComponent(type, result.Source);
            result.Add("import { IdType, IdDefault } from '@app/models/i-key-model';");
            result.Add("import { Component } from '@angular/core';");
            result.Add("import { CommonModule } from '@angular/common';");
            result.Add("import { FormsModule } from '@angular/forms';");
            result.Add("import { RouterModule } from '@angular/router';");

            result.Add("import { TranslateModule } from '@ngx-translate/core';");
            result.Add(CreateImport("@app-models", modelName, subPath));
            result.Add("import { " + $"{entityName}BaseListComponent" + " }" + $"from '@app/components/{baseSubFilePath}';");

            result.Add("import { " + $"{entityName}EditComponent" + " }" + $"from '@app/components/{editSubFilePath}';");

#if ACCOUNT_ON
            result.Add("import { AuthService } from '@app/services/auth.service';");
            result.Add("import { Role } from '@app/models/account/role';");
#endif

            result.Add(CreateImport("@app-services/http", serviceName, subPath));

            result.Add(StaticLiterals.CustomImportBeginLabel);
            result.AddRange(ReadCustomImports(projectPath, result));
            result.Add(StaticLiterals.CustomImportEndLabel);

            result.Add("@Component({");
            result.Add($"  standalone: true,");
            result.Add($"  selector:'app-{ConvertFileItem(entityName)}-list',");
            result.Add($"  imports: [ CommonModule, FormsModule, TranslateModule, RouterModule ],");
            result.Add($"  templateUrl: './{ConvertFileItem(entityName)}-list.component.html',");
            result.Add($"  styleUrl: './{ConvertFileItem(entityName)}-list.component.css'");
            result.Add("})");
            result.Add($"export class {entityName}ListComponent extends {entityName}BaseListComponent" + " {");
#if ACCOUNT_OFF
            result.Add($"  constructor(protected override dataAccessService: {serviceName})");
#else
            result.Add("  constructor(");
            result.Add($"              protected override dataAccessService: {serviceName},");
            result.Add($"              private authService: AuthService");
            result.Add($"             )");
#endif
            result.Add("  {");
            result.Add("    super(dataAccessService);");
            result.Add("  }");

            result.Add("  override ngOnInit(): void {");
            result.Add($"    this._queryParams.filter = '{CreateListFilterFromType(type, unitType)}';");
            result.Add("    this.reloadData();");
            result.Add("  }");

#if EXTERNALGUID_ON
            result.Add($"  protected override getItemKey(item: {modelName}): IdType " + "{");
            result.Add($"    return item?.guid || IdDefault;");
            result.Add("  }");
#else
            result.Add($"  protected override getItemKey(item: {modelName}): IdType " + "{");
            result.Add($"    return item?.id || IdDefault;");
            result.Add("  }");
#endif

            result.Add("  override get pageTitle(): string {");
            result.Add($"    return '{entityName.CreatePluralWord()}';");
            result.Add("  }");

            result.Add("  override getEditComponent() {");
            result.Add($"    return {entityName}EditComponent;");
            result.Add("  }");

            result.Add("}");

            result.Source.Insert(result.Source.Count - 1, StaticLiterals.CustomCodeBeginLabel);
            result.Source.InsertRange(result.Source.Count - 1, ReadCustomCode(projectPath, result));
            result.Source.Insert(result.Source.Count - 1, StaticLiterals.CustomCodeEndLabel);
            FinishCreateListComponent(type, result.Source);
            return result;
        }
        /// <summary>
        /// Creates a generated item representing the HTML template for an Angular entity page list component from the specified type.
        /// </summary>
        /// <param name="type">The entity type for which the page list HTML component is generated.</param>
        /// <param name="unitType">The unit type for the generated item.</param>
        /// <param name="itemType">The item type for the generated item.</param>
        /// <returns>
        /// A <see cref="Models.GeneratedItem"/> containing the generated HTML code for the entity page list component.
        /// </returns>
        private static Models.GeneratedItem CreateEntityListHtmlComponentFromType(Type type, Common.UnitType unitType, Common.ItemType itemType)
        {
            var subPath = ConvertPathItem(ItemProperties.CreateSubPathFromType(type));
            var entityName = ItemProperties.CreateEntityName(type);
            var fileName = $"{ConvertFileItem($"{entityName}List")}.component{StaticLiterals.HtmlFileExtension}";

            var result = new Models.GeneratedItem(unitType, itemType)
            {
                HasDefaultLabel = false,
                SpecialLabel = string.Empty,
                FullName = CreateTypeScriptComponentFullName(type),
                FileExtension = StaticLiterals.HtmlFileExtension,
                SubFilePath = Path.Combine(PagesSubFolder, subPath, fileName),
            };

            result.Add("<p>List item works!</p>");
            return result;
        }
        /// <summary>
        /// Creates a generated item representing the CSS file for an Angular entity page list component from the specified type.
        /// </summary>
        /// <param name="type">The entity type for which the page list CSS component is generated.</param>
        /// <param name="unitType">The unit type for the generated item.</param>
        /// <param name="itemType">The item type for the generated item.</param>
        /// <returns>
        /// A <see cref="Models.GeneratedItem"/> containing the generated CSS code for the entity page list component.
        /// </returns>
        private static Models.GeneratedItem CreateEntityListCssComponentFromType(Type type, Common.UnitType unitType, Common.ItemType itemType)
        {
            var subPath = ConvertPathItem(ItemProperties.CreateSubPathFromType(type));
            var entityName = ItemProperties.CreateEntityName(type);
            var fileName = $"{ConvertFileItem($"{entityName}List")}.component{StaticLiterals.CssFileExtension}";

            var result = new Models.GeneratedItem(unitType, itemType)
            {
                HasDefaultLabel = false,
                SpecialLabel = string.Empty,
                FullName = CreateTypeScriptComponentFullName(type),
                FileExtension = StaticLiterals.CssFileExtension,
                SubFilePath = Path.Combine(PagesSubFolder, subPath, fileName),
            };

            return result;
        }

        /// <summary>
        /// Creates a generated item representing an Angular entity page edit component from the specified type.
        /// </summary>
        /// <param name="type">The entity type for which the page edit component is generated.</param>
        /// <param name="unitType">The unit type for the generated item.</param>
        /// <param name="itemType">The item type for the generated item.</param>
        /// <returns>
        /// A <see cref="Models.GeneratedItem"/> containing the generated TypeScript code for the entity page edit component.
        /// </returns>
        private Models.GeneratedItem CreateEntityEditComponentFromType(Type type, Common.UnitType unitType, Common.ItemType itemType)
        {
            var subPath = ConvertPathItem(ItemProperties.CreateSubPathFromType(type));
            var projectPath = Path.Combine(SolutionProperties.SolutionPath, SolutionProperties.AngularAppProjectName);
            var entityName = ItemProperties.CreateEntityName(type);
            var fileName = $"{ConvertFileItem($"{entityName}Edit")}.component{StaticLiterals.TSFileExtension}";
            var baseFileName = $"{ConvertFileItem($"{entityName}BaseEdit")}.component";
            var baseSubFilePath = ConvertFileItem(Path.Combine(subPath, baseFileName));

            var result = new Models.GeneratedItem(unitType, itemType)
            {
                HasDefaultLabel = false,
                SpecialLabel = CommonStaticLiterals.CustomCodeLabel,
                FullName = CreateTypeScriptComponentFullName(type),
                FileExtension = StaticLiterals.TSFileExtension,
                SubFilePath = Path.Combine(ComponentsSubFolder, subPath, fileName),
            };

            StartCreateListComponent(type, result.Source);
            result.Add("import { Component } from '@angular/core';");
            result.Add("import { CommonModule } from '@angular/common';");
            result.Add("import { FormsModule } from '@angular/forms';");
            result.Add("import { TranslateModule } from '@ngx-translate/core';");

            result.Add("import { " + $"{entityName}BaseEditComponent" + " }" + $"from '@app/components/{baseSubFilePath}';");

            result.Add(StaticLiterals.CustomImportBeginLabel);
            result.AddRange(ReadCustomImports(projectPath, result));
            result.Add(StaticLiterals.CustomImportEndLabel);

            result.Add("@Component({");
            result.Add($"  selector:'app-{ConvertFileItem(entityName)}-edit',");
            result.Add($"  imports: [ CommonModule, FormsModule, TranslateModule],");
            result.Add($"  templateUrl: './{ConvertFileItem(entityName)}-edit.component.html',");
            result.Add($"  styleUrl: './{ConvertFileItem(entityName)}-edit.component.css'");
            result.Add("})");
            result.Add($"export class {entityName}EditComponent extends {entityName}BaseEditComponent" + " {");

            result.Add("}");

            result.Source.Insert(result.Source.Count - 1, StaticLiterals.CustomCodeBeginLabel);
            result.Source.InsertRange(result.Source.Count - 1, ReadCustomCode(projectPath, result));
            result.Source.Insert(result.Source.Count - 1, StaticLiterals.CustomCodeEndLabel);
            FinishCreateListComponent(type, result.Source);
            return result;
        }
        /// <summary>
        /// Creates a generated item representing the HTML template for an Angular entity page edit component from the specified type.
        /// </summary>
        /// <param name="type">The entity type for which the page edit HTML component is generated.</param>
        /// <param name="unitType">The unit type for the generated item.</param>
        /// <param name="itemType">The item type for the generated item.</param>
        /// <returns>
        /// A <see cref="Models.GeneratedItem"/> containing the generated HTML code for the entity page edit component.
        /// </returns>
        private static Models.GeneratedItem CreateEntityEditHtmlComponentFromType(Type type, Common.UnitType unitType, Common.ItemType itemType)
        {
            var subPath = ConvertPathItem(ItemProperties.CreateSubPathFromType(type));
            var entityName = ItemProperties.CreateEntityName(type);
            var fileName = $"{ConvertFileItem($"{entityName}Edit")}.component{StaticLiterals.HtmlFileExtension}";

            var result = new Models.GeneratedItem(unitType, itemType)
            {
                HasDefaultLabel = false,
                SpecialLabel = string.Empty,
                FullName = CreateTypeScriptComponentFullName(type),
                FileExtension = StaticLiterals.HtmlFileExtension,
                SubFilePath = Path.Combine(ComponentsSubFolder, subPath, fileName),
            };

            result.Add("<p>Edit item works!</p>");
            return result;
        }
        /// <summary>
        /// Creates a generated item representing the CSS file for an Angular entity page edit component from the specified type.
        /// </summary>
        /// <param name="type">The entity type for which the page edit CSS component is generated.</param>
        /// <param name="unitType">The unit type for the generated item.</param>
        /// <param name="itemType">The item type for the generated item.</param>
        /// <returns>
        /// A <see cref="Models.GeneratedItem"/> containing the generated CSS code for the entity page edit component.
        /// </returns>
        private static Models.GeneratedItem CreateEntityEditCssComponentFromType(Type type, Common.UnitType unitType, Common.ItemType itemType)
        {
            var subPath = ConvertPathItem(ItemProperties.CreateSubPathFromType(type));
            var entityName = ItemProperties.CreateEntityName(type);
            var fileName = $"{ConvertFileItem($"{entityName}Edit")}.component{StaticLiterals.CssFileExtension}";

            var result = new Models.GeneratedItem(unitType, itemType)
            {
                HasDefaultLabel = false,
                SpecialLabel = string.Empty,
                FullName = CreateTypeScriptComponentFullName(type),
                FileExtension = StaticLiterals.CssFileExtension,
                SubFilePath = Path.Combine(ComponentsSubFolder, subPath, fileName),
            };

            return result;
        }

        /// <summary>
        /// Creates a generated item representing an Angular view page list component from the specified type.
        /// </summary>
        /// <param name="type">The view type for which the page list component is generated.</param>
        /// <param name="unitType">The unit type for the generated item.</param>
        /// <param name="itemType">The item type for the generated item.</param>
        /// <returns>
        /// A <see cref="Models.GeneratedItem"/> containing the generated TypeScript code for the view page list component.
        /// </returns>
        private Models.GeneratedItem CreateViewListComponentFromType(Type type, Common.UnitType unitType, Common.ItemType itemType)
        {
            var subPath = ConvertPathItem(ItemProperties.CreateSubPathFromType(type));
            var projectPath = Path.Combine(SolutionProperties.SolutionPath, SolutionProperties.AngularAppProjectName);
            var entityName = ItemProperties.CreateEntityName(type);
            var modelName = ItemProperties.CreateTSModelName(type);
            var serviceName = $"{entityName}Service";
            var fileName = $"{ConvertFileItem($"{entityName}List")}.component{StaticLiterals.TSFileExtension}";
            var baseFileName = $"{ConvertFileItem($"{entityName}BaseList")}.component";
            var baseSubFilePath = ConvertFileItem(Path.Combine(subPath, baseFileName));

            var result = new Models.GeneratedItem(unitType, itemType)
            {
                HasDefaultLabel = false,
                SpecialLabel = CommonStaticLiterals.CustomCodeLabel,
                FullName = CreateTypeScriptComponentFullName(type),
                FileExtension = StaticLiterals.TSFileExtension,
                SubFilePath = Path.Combine(PagesSubFolder, subPath, fileName),
            };

            StartCreateListComponent(type, result.Source);
            result.Add("import { Component } from '@angular/core';");
            result.Add("import { CommonModule } from '@angular/common';");
            result.Add("import { FormsModule } from '@angular/forms';");
            result.Add("import { TranslateModule } from '@ngx-translate/core';");

            result.Add("import { " + $"{entityName}BaseListComponent" + " }" + $"from '@app/components/{baseSubFilePath}';");

#if ACCOUNT_ON
            result.Add("import { AuthService } from '@app/services/auth.service';");
            result.Add("import { Role } from '@app/models/account/role';");
#endif

            result.Add(CreateImport("@app-services/http", serviceName, subPath));

            result.Add(StaticLiterals.CustomImportBeginLabel);
            result.AddRange(ReadCustomImports(projectPath, result));
            result.Add(StaticLiterals.CustomImportEndLabel);

            result.Add("@Component({");
            result.Add($"  selector:'app-{ConvertFileItem(entityName)}-list',");
            result.Add($"  imports: [ CommonModule, FormsModule, TranslateModule ],");
            result.Add($"  templateUrl: './{ConvertFileItem(entityName)}-list.component.html',");
            result.Add($"  styleUrl: './{ConvertFileItem(entityName)}-list.component.css'");
            result.Add("})");
            result.Add($"export class {entityName}ListComponent extends {entityName}BaseListComponent" + " {");
#if ACCOUNT_OFF
            result.Add($"  constructor(protected override dataAccessService: {serviceName})");
#else
            result.Add("  constructor(");
            result.Add($"              protected override dataAccessService: {serviceName},");
            result.Add($"              private authService: AuthService");
            result.Add($"             )");
#endif
            result.Add("  {");
            result.Add("    super(dataAccessService);");
            result.Add("  }");

            result.Add("  override ngOnInit(): void {");
            result.Add($"    this._queryParams.filter = '{CreateListFilterFromType(type, unitType)}';");
            result.Add("    this.reloadData();");
            result.Add("  }");

            result.Add("  override get pageTitle(): string {");
            result.Add($"    return '{entityName.CreatePluralWord()}';");
            result.Add("  }");

            result.Add("}");

            result.Source.Insert(result.Source.Count - 1, StaticLiterals.CustomCodeBeginLabel);
            result.Source.InsertRange(result.Source.Count - 1, ReadCustomCode(projectPath, result));
            result.Source.Insert(result.Source.Count - 1, StaticLiterals.CustomCodeEndLabel);
            FinishCreateListComponent(type, result.Source);
            return result;
        }
        /// <summary>
        /// Creates a generated item representing the HTML template for an Angular view page list component from the specified type.
        /// </summary>
        /// <param name="type">The view type for which the page list HTML component is generated.</param>
        /// <param name="unitType">The unit type for the generated item.</param>
        /// <param name="itemType">The item type for the generated item.</param>
        /// <returns>
        /// A <see cref="Models.GeneratedItem"/> containing the generated HTML code for the view page list component.
        /// </returns>
        private static Models.GeneratedItem CreateViewListHtmlComponentFromType(Type type, Common.UnitType unitType, Common.ItemType itemType)
        {
            var subPath = ConvertPathItem(ItemProperties.CreateSubPathFromType(type));
            var entityName = ItemProperties.CreateEntityName(type);
            var fileName = $"{ConvertFileItem($"{entityName}List")}.component{StaticLiterals.HtmlFileExtension}";

            var result = new Models.GeneratedItem(unitType, itemType)
            {
                HasDefaultLabel = false,
                SpecialLabel = string.Empty,
                FullName = CreateTypeScriptComponentFullName(type),
                FileExtension = StaticLiterals.HtmlFileExtension,
                SubFilePath = Path.Combine(PagesSubFolder, subPath, fileName),
            };

            result.Add("<p>List item works!</p>");
            return result;
        }
        /// <summary>
        /// Creates a generated item representing the CSS file for an Angular view page list component from the specified type.
        /// </summary>
        /// <param name="type">The view type for which the page list CSS component is generated.</param>
        /// <param name="unitType">The unit type for the generated item.</param>
        /// <param name="itemType">The item type for the generated item.</param>
        /// <returns>
        /// A <see cref="Models.GeneratedItem"/> containing the generated CSS code for the view page list component.
        /// </returns>
        private static Models.GeneratedItem CreateViewListCssComponentFromType(Type type, Common.UnitType unitType, Common.ItemType itemType)
        {
            var subPath = ConvertPathItem(ItemProperties.CreateSubPathFromType(type));
            var entityName = ItemProperties.CreateEntityName(type);
            var fileName = $"{ConvertFileItem($"{entityName}List")}.component{StaticLiterals.CssFileExtension}";

            var result = new Models.GeneratedItem(unitType, itemType)
            {
                HasDefaultLabel = false,
                SpecialLabel = string.Empty,
                FullName = CreateTypeScriptComponentFullName(type),
                FileExtension = StaticLiterals.CssFileExtension,
                SubFilePath = Path.Combine(PagesSubFolder, subPath, fileName),
            };

            return result;
        }

        /// <summary>
        /// Starts the process of creating a component of the specified type, using the given list of lines.
        /// </summary>
        /// <param name="type">The type of service to be created.</param>
        /// <param name="lines">The list of lines to be used for creating the service.</param>
        partial void StartCreateListComponent(Type type, List<string> lines);
        /// <summary>
        /// FinishCreateService is a method that finishes the creation of a component with the specified type and lines.
        /// </summary>
        /// <param name="type">The Type object representing the type of the service.</param>
        /// <param name="lines">A List of strings containing the lines of the service.</param>
        partial void FinishCreateListComponent(Type type, List<string> lines);

        #region Helpers
        /// <summary>
        /// Reads the custom imports from a source file and returns them as a sequence of strings.
        /// </summary>
        /// <param name="sourcePath">The path to the source file directory.</param>
        /// <param name="generatedItem">The generated item.</param>
        /// <returns>A sequence of strings representing the custom imports.</returns>
        public static IEnumerable<string> ReadCustomImports(string sourcePath, Models.GeneratedItem generatedItem)
        {
            var result = new List<string>();
            var sourceFilePath = Path.Combine(sourcePath, generatedItem.SubFilePath);
            var customFilePath = FileHandler.CreateCustomFilePath(sourceFilePath);

            if (File.Exists(sourceFilePath))
            {
                result.AddRange(FileHandler.ReadCustomImports(sourceFilePath));
            }
            else
            {
                result.AddRange(FileHandler.ReadCustomImports(customFilePath));
            }
            return result.Where(l => string.IsNullOrEmpty(l.Trim()) == false);
        }
        /// <summary>
        /// Reads custom code from a source file and a generated item.
        /// </summary>
        /// <param name="sourcePath">The path to the source file.</param>
        /// <param name="generatedItem">The generated item representing the file.</param>
        /// <returns>An enumerable collection of strings containing the custom code.</returns>
        public static IEnumerable<string> ReadCustomCode(string sourcePath, Models.GeneratedItem generatedItem)
        {
            var result = new List<string>();
            var sourceFilePath = Path.Combine(sourcePath, generatedItem.SubFilePath);
            var customFilePath = FileHandler.CreateCustomFilePath(sourceFilePath);

            if (File.Exists(sourceFilePath))
            {
                result.AddRange(FileHandler.ReadCustomCode(sourceFilePath));
            }
            else
            {
                result.AddRange(FileHandler.ReadCustomCode(customFilePath));
            }
            return result.Where(l => string.IsNullOrEmpty(l.Trim()) == false);
        }
        /// <summary>
        /// Converts a file item into a normalized format.
        /// </summary>
        /// <param name="fileItem">The file item to be converted.</param>
        /// <returns>A string representing the normalized file item.</returns>
        public static string ConvertFileItem(string fileItem)
        {
            var result = new StringBuilder();

            foreach (var item in fileItem)
            {
                if (result.Length == 0)
                {
                    result.Append(char.ToLower(item));
                }
                else if (item == '\\')
                {
                    result.Append('/');
                }
                else if (char.IsUpper(item))
                {
                    if (result[^1] != '-' && result[^1] != '/' && result[^1] != '\\')
                    {
                        result.Append('-');
                    }
                    result.Append(char.ToLower(item));
                }
                else
                {
                    result.Append(char.ToLower(item));
                }
            }
            return result.ToString();
        }
        /// <summary>
        /// Converts a file path item to a normalized format by replacing backslashes with forward slashes
        /// and converting all characters to lower case.
        /// </summary>
        /// <param name="fileItem">The file path item to convert.</param>
        /// <returns>A normalized string representing the file path item.</returns>
        public static string ConvertPathItem(string fileItem)
        {
            var result = new StringBuilder();

            foreach (var item in fileItem)
            {
                if (result.Length == 0)
                {
                    result.Append(char.ToLower(item));
                }
                else if (item == '\\')
                {
                    result.Append('/');
                }
                else
                {
                    result.Append(char.ToLower(item));
                }
            }
            return result.ToString();
        }
        /// <summary>
        /// Creates an import statement for a given alias, typeName, and subPath.
        /// </summary>
        /// <param name="alias">The alias to be used for the import statement.</param>
        /// <param name="importName">The type name to be imported.</param>
        /// <param name="subPath">The sub path where the file is located.</param>
        /// <returns>The import statement string.</returns>
        public static string CreateImport(string alias, string importName, string subPath)
        {
            var result = "import { " + importName + " } from ";

            if (subPath.IsNullOrEmpty())
            {
                result += $"'{alias}/{ConvertFileItem(importName)}';";
            }
            else
            {
                result += $"'{alias}/{ConvertPathItem(subPath)}/{ConvertFileItem(importName)}';";
            }
            return result;
        }
        /// <summary>
        /// Inserts a collection of import statements into a list of lines.
        /// </summary>
        /// <param name="imports">The collection of import statements to be inserted.</param>
        /// <param name="lines">The list of lines to insert the import statements into.</param>
        /// <remarks>
        /// The import statements will be inserted at the beginning of the list in the reverse order
        /// of the original collection, with duplicate statements removed.
        /// </remarks>
        public static void InsertTypeImports(IEnumerable<string> imports, List<string> lines)
        {
            foreach (var item in imports.Reverse().Distinct())
            {
                lines.Insert(0, item);
            }
        }

        /// <summary>
        /// Creates a list of type imports based on the provided type and a collection of types.
        /// </summary>
        /// <param name="type">The type for which type imports are being created.</param>
        /// <param name="types">The collection of types used to create type imports.</param>
        /// <returns>A distinct collection of type imports.</returns>
        public static IEnumerable<string> CreateTypeImports(Type type, IEnumerable<Type> types)
        {
            var result = new List<string>();
            var typeProperties = type.GetAllPropertyInfos();
            var entityName = ItemProperties.CreateEntityName(type);

            foreach (var propertyInfo in typeProperties)
            {
                if (propertyInfo.PropertyType.IsEnum)
                {
                    var typeName = $"{propertyInfo.PropertyType.Name}";

                    if (typeName.Equals(entityName) == false)
                    {
                        var subPath = ConvertPathItem(ItemProperties.CreateItemSubPathFromType(propertyInfo.PropertyType));

                        result.Add(CreateImport("@app-enums", typeName, subPath));
                    }
                }
                else if (propertyInfo.PropertyType.IsGenericType)
                {
                    var subType = propertyInfo.PropertyType.GetGenericArguments().First();
                    var modelType = types.FirstOrDefault(e => e.FullName == subType.FullName);

                    if (modelType != null && modelType.IsClass)
                    {
                        var modelName = ItemProperties.CreateTSModelName(modelType);

                        if (modelName.Equals(entityName) == false)
                        {
                            var subPath = ConvertPathItem(ItemProperties.CreateSubPathFromType(modelType));

                            result.Add(CreateImport("@app-models", modelName, subPath));
                        }
                    }
                }
                else if (propertyInfo.PropertyType.IsClass)
                {
                    var modelType = types.FirstOrDefault(e => e.FullName == propertyInfo.PropertyType.FullName);

                    if (modelType != null && modelType.IsClass)
                    {
                        var modelName = ItemProperties.CreateTSModelName(modelType);

                        if (modelName.Equals(entityName) == false)
                        {
                            var subPath = ConvertPathItem(ItemProperties.CreateSubPathFromType(modelType));

                            result.Add(CreateImport("@app-models", modelName, subPath));
                        }
                    }
                }
            }
            return result.Distinct();
        }
        /// <summary>
        /// Creates TypeScript properties based on the given PropertyInfo and types.
        /// </summary>
        /// <param name="propertyInfo">The PropertyInfo object.</param>
        /// <param name="types">The collection of types.</param>
        /// <returns>An IEnumerable of strings containing the TypeScript properties.</returns>
        public static IEnumerable<string> CreateTypeScriptProperty(PropertyInfo propertyInfo, IEnumerable<Type> types)
        {
            var result = new List<string>();
            var tsPropertyName = ItemProperties.CreateTSPropertyName(propertyInfo);
            var navigationType = types.FirstOrDefault(t => t.FullName!.Equals(propertyInfo.PropertyType.FullName));
            var indentSpaceSave = StringExtensions.IndentSpace;

            StringExtensions.IndentSpace = "  ";
            if (navigationType != null)
            {
                result.Add($"{tsPropertyName.SetIndent()}: {ItemProperties.CreateTSModelName(navigationType)} | null;");
            }
            else if (propertyInfo.PropertyType.IsEnum)
            {
                var enumName = $"{propertyInfo.PropertyType.Name.SetIndent()}";

                result.Add($"{tsPropertyName.SetIndent()}: {enumName};");
            }
            else if (propertyInfo.PropertyType == typeof(DateTime?))
            {
                result.Add($"{tsPropertyName.SetIndent()}: Date | null;");
            }
            else if (propertyInfo.PropertyType == typeof(DateTime))
            {
                result.Add($"{tsPropertyName.SetIndent()}: Date;");
            }
            else if (propertyInfo.PropertyType == typeof(DateOnly?))
            {
                result.Add($"{tsPropertyName.SetIndent()}: Date | null;");
            }
            else if (propertyInfo.PropertyType == typeof(DateOnly))
            {
                result.Add($"{tsPropertyName.SetIndent()}: Date;");
            }
            else if (propertyInfo.PropertyType == typeof(TimeSpan?))
            {
                result.Add($"{tsPropertyName.SetIndent()}: string | null;");
            }
            else if (propertyInfo.PropertyType == typeof(TimeSpan))
            {
                result.Add($"{tsPropertyName.SetIndent()}: string;");
            }
            else if (propertyInfo.PropertyType == typeof(string))
            {
                if (propertyInfo.PropertyType.IsNullableType())
                    result.Add($"{tsPropertyName.SetIndent()}: string | null;");
                else
                    result.Add($"{tsPropertyName.SetIndent()}: string;");
            }
            else if (propertyInfo.PropertyType == typeof(Guid?))
            {
                result.Add($"{tsPropertyName.SetIndent()}: string | null;");
            }
            else if (propertyInfo.PropertyType == typeof(Guid))
            {
                result.Add($"{tsPropertyName.SetIndent()}: string;");
            }
            else if (propertyInfo.PropertyType == typeof(bool?))
            {
                result.Add($"{tsPropertyName.SetIndent()}: boolean | null;");
            }
            else if (propertyInfo.PropertyType == typeof(bool))
            {
                result.Add($"{tsPropertyName.SetIndent()}: boolean;");
            }
            else if (propertyInfo.PropertyType.IsNumericType())
            {
                if (propertyInfo.PropertyType.IsNullableValueType())
                    result.Add($"{tsPropertyName.SetIndent()}: number | null;");
                else
                    result.Add($"{tsPropertyName.SetIndent()}: number;");
            }
            else if (propertyInfo.PropertyType.IsGenericType)
            {
                Type subType = propertyInfo.PropertyType.GetGenericArguments().First();

                if (subType.IsInterface)
                {
                    result.Add($"{tsPropertyName.SetIndent()}: {subType.Name[1..]}[];");
                }
                else if (subType == typeof(Guid?))
                {
                    result.Add($"{tsPropertyName.SetIndent()}: string | null;");
                }
                else if (subType == typeof(Guid))
                {
                    result.Add($"{tsPropertyName.SetIndent()}: string;");
                }
                else
                {
                    result.Add($"{tsPropertyName.SetIndent()}: {ItemProperties.CreateTSModelName(subType)}[];");
                }
            }
            else if (propertyInfo.PropertyType.IsInterface)
            {
                result.Add($"{tsPropertyName.SetIndent()}: {propertyInfo.PropertyType.Name[1..]};");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"Unknown property type: {propertyInfo.PropertyType.FullName}");
            }
            StringExtensions.IndentSpace = indentSpaceSave;
            return result;
        }
        /// <summary>
        /// Creates model to model imports for a given type and a collection of types.
        /// </summary>
        /// <param name="type">The type for which model to model imports are created.</param>
        /// <param name="types">The collection of types to search for model to model imports.</param>
        /// <returns>An IEnumerable collection of strings representing the model to model imports.</returns>
        private static IEnumerable<string> CreateModelToModelImports(Type type, IEnumerable<Type> types)
        {
            var result = new List<string>();
            var modelName = ItemProperties.CreateTSModelName(type);

            foreach (var pi in type.GetProperties())
            {
                var other = types.FirstOrDefault(t => t == pi.PropertyType);

                if (other != null && other != type)
                {
                    var refTypeName = ItemProperties.CreateTSModelName(other);
                    var subPath = ConvertPathItem(ItemProperties.CreateSubPathFromType(other));

                    result.Add(CreateImport("@app-models", refTypeName, subPath));
                }
            }
            return result.Distinct();
        }

        /// <summary>
        /// Queries a setting value of type <typeparamref name="T"/> from a specific item type, using the specified value name and default value.
        /// </summary>
        /// <typeparam name="T">The type of the setting value to be queried.</typeparam>
        /// <param name="itemType">The itemType of the setting.</param>
        /// <param name="type">The type of the setting.</param>
        /// <param name="valueName">The name of the setting value to be queried.</param>
        /// <param name="defaultValue">The default value to be used if the queried setting value is not found or cannot be converted to type <typeparamref name="T"/>.</param>
        /// <returns>The queried setting value of type <typeparamref name="T"/> or the default value if the queried setting value is not found or cannot be converted to type <typeparamref name="T"/>.</returns>
        private T QuerySetting<T>(Common.ItemType itemType, Type type, string valueName, string defaultValue)
        {
            T result;

            try
            {
                result = (T)Convert.ChangeType(QueryGenerationSettingValue(Common.UnitType.AngularApp, itemType, ItemProperties.CreateSubTypeFromEntity(type), valueName, defaultValue), typeof(T));
            }
            catch (Exception ex)
            {
                result = (T)Convert.ChangeType(defaultValue, typeof(T));
                System.Diagnostics.Debug.WriteLine($"Error in {System.Reflection.MethodBase.GetCurrentMethod()!.Name}: {ex.Message}");
            }
            return result;
        }
        /// <summary>
        ///   Queries a setting value and converts it to the specified type.
        /// </summary>
        /// <typeparam name="T">The type to which the setting value will be converted.</typeparam>
        /// <param name="itemType">The type of item for which the setting is being queried.</param>
        /// <param name="itemName">The name of the item for which the setting is being queried.</param>
        /// <param name="valueName">The name of the setting being queried.</param>
        /// <param name="defaultValue">The default value to be used if the setting value cannot be queried or converted.</param>
        /// <returns>
        ///   The queried setting value converted to the specified type, or the default value if an error occurs.
        /// </returns>
        /// <remarks>
        ///   If querying or converting the setting value throws an exception, the default value will be used
        ///   and an error message will be written to the debug output.
        /// </remarks>
        private T QuerySetting<T>(Common.ItemType itemType, string itemName, string valueName, string defaultValue)
        {
            T result;

            try
            {
                result = (T)Convert.ChangeType(QueryGenerationSettingValue(Common.UnitType.AngularApp, itemType, itemName, valueName, defaultValue), typeof(T));
            }
            catch (Exception ex)
            {
                result = (T)Convert.ChangeType(defaultValue, typeof(T));
                System.Diagnostics.Debug.WriteLine($"Error in {System.Reflection.MethodBase.GetCurrentMethod()!.Name}: {ex.Message}");
            }
            return result;
        }
        #endregion Helpers
    }
}
