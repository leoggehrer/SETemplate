//@BaseCode
//MdStart
using System.Text.Json.Serialization;

namespace TemplateTools.ConApp.Models.ChatGPT
{
    /// <summary>
    /// Represents a message object.
    /// </summary>
    public partial class Message
    {
        /// <summary>
        /// Gets or sets the role.
        /// </summary>
        [JsonPropertyName("role")]
        public string Role { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the content of the property.
        /// </summary>
        [JsonPropertyName("content")]
        public string Content { get; set; } = string.Empty;
    }
}
//MdEnd
