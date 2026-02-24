//@BaseCode

namespace TemplateTools.ConApp.Models
{
    /// <summary>
    /// Represents a request to the MCP (Managed Control Plane) API.
    /// </summary>
    public partial class McpRequest
    {
        /// <summary>
        /// Gets or sets the source URL for the request.
        /// </summary>
        public string SourceUrl { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the target parameters for the request.
        /// </summary>
        public string TargetParams { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the payload of the request.
        /// </summary>
        public Payload? Payload { get; set; }
    }
}
