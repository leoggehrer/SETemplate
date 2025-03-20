//@BaseCode
//MdStart
namespace TemplateTools.ConApp.Models.ChatGPT
{
    /// <summary>
    /// Represents a response.
    /// </summary>
    public partial class Response
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public string Id { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the value indicating the creation status.
        /// </summary>
        /// <remarks>
        /// This property represents the timestamp indicating when the object was created.
        /// </remarks>
        public int Created { get; set; }
        /// <summary>
        /// Gets or sets the model.
        /// </summary>
        /// <value>
        /// The model.
        /// </value>
        public string Model { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the choices available for selection.
        /// </summary>
        /// <value>An array of Choice objects.</value>
        public Choice[] Choices { get; set; } = [];
        /// <summary>
        /// Gets or sets the usage of the property.
        /// </summary>
        /// <value>
        /// The usage of the property.
        /// </value>
        public Usage? Usage { get; set; }
        
        /// <summary>
        /// Returns a string representation of the object.
        /// </summary>
        /// <returns>The string representation of the object.</returns>
        public override string ToString()
        {
            var result = string.Empty;
            
            if (Choices.Length != 0)
            {
                result = Choices.First().Message!.Content;
            }
            return result;
        }
    }
}
//MdEnd
