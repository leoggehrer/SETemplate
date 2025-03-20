//@BaseCode
//MdStart
namespace TemplateTools.ConApp.Models.ChatGPT
{
    /// <summary>
    /// Represents a choice.
    /// </summary>
    public partial class Choice
    {
        /// <summary>
        /// Gets or sets the index value.
        /// </summary>
        public int Index { get; set; }
        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        public Message? Message { get; set; }
        /// <summary>
        /// Gets or sets the reason for finishing.
        /// </summary>
        /// <value>A string representing the reason for finishing.</value>
        /// <remarks>The default value is an empty string.</remarks>
        public string Finish_reason { get; set; } = string.Empty;
    }
}
//MdEnd
