//@BaseCode
namespace SETemplate.Logic
{
    /// <summary>
    /// Provides a collection of static literals.
    /// </summary>
    public static partial class StaticLiterals
    {
        /// <summary>
        /// Gets the value representing the role "SysAdmin".
        /// </summary>
        /// <value>The value representing the role "SysAdmin".</value>
        public static string RoleSysAdmin => "SysAdmin";
        /// <summary>
        /// Represents the role name for the application administrator.
        /// </summary>
        /// <value>The role name "AppAdmin".</value>
        public static string RoleAppAdmin => "AppAdmin";
        /// <summary>
        /// Gets or sets the name of the connection string key for the environment.
        /// </summary>
        /// <value>
        /// The name of the connection string key for the environment.
        /// </value>
        public static string EnvironmentConnectionStringKey => "ASPNETCORE_CONNECTIONSTRING";
        /// <summary>
        /// Gets the key for the connection string in the app settings.
        /// </summary>
        /// <value>
        /// The key for the connection string in the app settings.
        /// </value>
        public static string AppSettingsConnectionStringKey => "ConnectionStrings:DefaultConnection";
        
        /// <summary>
        /// Gets the maximum page size.
        /// </summary>
        /// <value>The maximum page size.</value>
        public static int MaxPageSize => Common.StaticLiterals.MaxPageSize;
    }
}
