//@BaseCode

namespace TemplateTools.ConApp.Models
{
    /// <summary>
    /// Represents a payload with a type, data, and description.
    /// </summary>
    public partial class Payload
    {
        /// <summary>
        /// Gets or sets the type of the payload.
        /// </summary>
        public string? Type { get; set; }

        /// <summary>
        /// Gets or sets the data of the payload.
        /// </summary>
        public string? Data { get; set; }

        /// <summary>
        /// Gets or sets the description of the payload.
        /// </summary>
        public string? Description { get; set; }
    }
}
