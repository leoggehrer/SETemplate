//@BaseCode
//MdStart
namespace TemplateTools.Logic.Contracts
{
    /// <summary>
    /// Represents a set of solution properties that describe various aspects of a software solution.
    /// </summary>
    public interface ISolutionProperties
    {
        #region Project extensions
        /// <summary>
        /// Gets the logic extension of the property.
        /// </summary>
        /// <value>
        /// The logic extension of the property.
        /// </value>
        string LogicExtension { get; }
        /// <summary>
        /// Gets the WebApiExtension property.
        /// </summary>
        /// <value>
        /// A string representing the WebApiExtension.
        /// </value>
        string WebApiExtension { get; }
        /// <summary>
        /// Gets the ASP.NET MVC extension for the property.
        /// </summary>
        /// <value>The ASP.NET MVC extension.</value>
        string AspMvcExtension { get; }
        /// <summary>
        /// Gets the Angular extension associated with the property.
        /// </summary>
        /// <remarks>
        /// This property is used to store the file extension used by Angular for the property.
        /// </remarks>
        /// <value>
        /// The Angular extension as a string.
        /// </value>
        string AngularExtension { get; }
        /// <summary>
        /// Gets the MVVMExtension property value.
        /// </summary>
        /// <value>
        /// The value of the MVVMExtension property.
        /// </value>
        string MVVMExtension { get; }
        /// <summary>
        /// Gets the client Blazor extension.
        /// </summary>
        /// <value>
        /// The client Blazor extension.
        /// </value>
        string ClientBlazorExtension { get; }
        #endregion Project extensions
        
        #region properties
        ///<summary>
        /// Gets the path of the solution.
        ///</summary>
        ///<value>
        /// A string representing the path of the solution.
        ///</value>
        string SolutionPath { get; }
        ///
        /// Gets the name of the solution.
        ///
        /// The name of the solution.
        string SolutionName { get; }
        ///<summary>
        ///Gets the file path of the solution.
        ///</summary>
        ///<value>
        ///A string representing the file path of the solution.
        ///</value>
        string SolutionFilePath { get; }
        /// <summary>
        /// Gets or sets the compile path for the string.
        /// </summary>
        /// <value>
        /// The compile path.
        /// </value>
        string? CompilePath { get; set; }
        /// <summary>
        /// Gets or sets an array of types representing logic assemblies.
        /// </summary>
        /// <value>
        /// An array of types representing logic assemblies.
        /// </value>
        Type[]? LogicAssemblyTypes { get; set; }
        /// <summary>
        /// Gets the file path of the compiled logic assembly.
        /// </summary>
        /// <remarks>
        /// This property returns the file path of the compiled logic assembly.
        /// A null value indicates that the logic assembly has not been compiled or is not available.
        /// </remarks>
        /// <value>
        /// The file path of the compiled logic assembly. A null value indicates that the logic assembly
        /// has not been compiled or is not available.
        /// </value>
        string? CompileLogicAssemblyFilePath { get; }
        
        /// <summary>
        /// Gets the collection of template project names.
        /// </summary>
        /// <remarks>
        /// This property returns an enumerable collection of strings that represent the names of the template projects.
        /// </remarks>
        IEnumerable<string> TemplateProjectNames { get; }
        /// <summary>
        /// Gets the collection of all template project names.
        /// </summary>
        /// <value>
        /// An <see cref="IEnumerable{T}"/> of type <see cref="string"/> representing all the template project names.
        /// </value>
        IEnumerable<string> AllTemplateProjectNames { get; }
        /// <summary>
        /// Gets the collection of paths to template projects.
        /// </summary>
        /// <value>
        /// The collection of paths to template projects.
        /// </value>
        IEnumerable<string> TemplateProjectPaths { get; }
        
        /// <summary>
        /// Gets the file path of the logic C# project.
        /// </summary>
        /// <value>The file path of the logic C# project.</value>
        string LogicCSProjectFilePath { get; }
        /// <summary>
        /// Gets the file path of the logic assembly.
        /// </summary>
        /// <value>The logic assembly file path.</value>
        string LogicAssemblyFilePath { get; }
        /// <summary>
        /// Gets the logic project name.
        /// </summary>
        /// <value>
        /// The logic project name as a string.
        /// </value>
        string LogicProjectName { get; }
        ///<summary>
        /// Gets the logic sub path.
        ///</summary>
        ///<returns>
        /// The logic sub path as a string.
        ///</returns>
        string LogicSubPath { get; }
        /// <summary>
        /// Gets the subpath for logic controllers.
        /// </summary>
        /// <remarks>
        /// This property returns the subpath for logic controllers.
        /// </remarks>
        /// <value>The subpath for logic controllers.</value>
        string LogicControllersSubPath { get; }
        /// <summary>
        /// Gets the sub path of the LogicDataContext.
        /// </summary>
        /// <value>
        /// The sub path of the LogicDataContext.
        /// </value>
        string LogicDataContextSubPath { get; }
        /// <summary>
        /// Gets the sub path for the logic entities.
        /// </summary>
        /// <remarks>
        /// This property represents the sub path that is used for locating the logic entities in the file system.
        /// </remarks>
        /// <value>
        /// The sub path for the logic entities.
        /// </value>
        string LogicEntitiesSubPath { get; }
        
        /// <summary>
        /// Gets the name of the Web Api project.
        /// </summary>
        /// <value>
        /// The name of the Web Api project.
        /// </value>
        string WebApiProjectName { get; }
        /// <summary>
        /// Gets the path of the WebApi.
        /// </summary>
        /// <value>
        /// The subpath of the WebApi.
        /// </value>
        string WebApiSubPath { get; }
        /// <summary>
        /// Gets the subpath of the web API controllers.
        /// </summary>
        /// <value>The subpath of the web API controllers.</value>
        string WebApiControllersSubPath { get; }
        
        /// <summary>
        /// Gets the name of the ASP.NET MVC application project.
        /// </summary>
        /// <value>
        /// The name of the ASP.NET MVC application project.
        /// </value>
        string AspMvcAppProjectName { get; }
        /// <summary>
        /// Gets the subpath of the ASP.NET MVC application.
        /// </summary>
        /// <value>
        /// The subpath of the ASP.NET MVC application.
        /// </value>
        string AspMvcAppSubPath { get; }
        /// <summary>
        /// Gets the subpath of ASP.NET MVC controllers.
        /// </summary>
        /// <value>
        /// The subpath of ASP.NET MVC controllers as a string.
        /// </value>
        string AspMvcControllersSubPath { get; }
        
        /// <summary>
        /// Gets the name of the MVVM application project.
        /// </summary>
        /// <returns>
        /// A string representing the name of the MVVM application project.
        /// </returns>
        string MVVMAppProjectName { get; }
        /// <summary>
        /// Gets the subpath of the MVVM application.
        /// </summary>
        /// <value>
        /// The subpath of the MVVM application.
        /// </value>
        string MVVMAppSubPath { get; }
        
        /// <summary>
        /// Gets the name of the client Blazor project.
        /// </summary>
        /// <value>
        /// The name of the client Blazor project.
        /// </value>
        string ClientBlazorProjectName { get; }
        /// <summary>
        /// Gets the subpath of the Blazor client.
        /// </summary>
        /// <remarks>
        /// The subpath is a part of the URL path that is used to access the Blazor client.
        /// </remarks>
        /// <value>
        /// The subpath of the Blazor client.
        /// </value>
        string ClientBlazorSubPath { get; }
        
        /// <summary>
        /// Gets the name of the Angular app project.
        /// </summary>
        /// <value>The name of the Angular app project.</value>
        string AngularAppProjectName { get; }
        #endregion properties
        
        #region methods
        /// <summary>
        /// Checks if the given file path corresponds to a template project file.
        /// </summary>
        /// <param name="filePath">The path of the file to be checked.</param>
        /// <returns>True if the file is a template project file; otherwise, false.</returns>
        bool IsTemplateProjectFile(string filePath);
        /// <summary>
        /// Retrieves the name of the project from the given project path.
        /// </summary>
        /// <param name="projectPath">The full path of the project.</param>
        /// <returns>The name of the project.</returns>
        string GetProjectNameFromPath(string projectPath);
        /// <summary>
        /// Retrieves the project name from the given file path.
        /// </summary>
        /// <param name="filePath">The path of the file.</param>
        /// <returns>The project name extracted from the file path.</returns>
        string GetProjectNameFromFile(string filePath);
        #endregion methods
    }
}
//MdEnd
