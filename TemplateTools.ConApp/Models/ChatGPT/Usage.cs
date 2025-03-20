//@BaseCode
//MdStart
namespace TemplateTools.ConApp.Models.ChatGPT
{
    /// <summary>
    /// Represents the usage of tokens.
    /// </summary>
    public partial class Usage
    {
        /// Gets or sets the number of prompt tokens.
        public int Prompt_tokens { get; set; }
        /// <summary>
        /// Gets or sets the number of completion tokens.
        /// </summary>
        /// <value>
        /// The number of completion tokens.
        /// </value>
        public int Completion_tokens { get; set; }
        /// Gets or sets the total number of tokens.
        public int Total_tokens { get; set; }
    }
}
//MdEnd
