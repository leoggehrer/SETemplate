//@BaseCode
namespace SETemplate.Logic.Models
{
    /// <summary>
    /// Query parameters for data retrieval.
    /// </summary>
    public partial class QueryParams
    {
        /// <summary>
        /// Gets or sets the related entities to include in the query.
        /// </summary>
        public string[] Includes { get; set; } = [];
        /// <summary>
        /// Gets or sets the filter expression.
        /// </summary>
        public string Filter { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the values for the filter expression.
        /// </summary>
        public string[] Values { get; set; } = [];
        /// <summary>
        /// Gets or sets the property name to sort the query results by.
        /// </summary>
        public string SortBy { get; set; } = string.Empty;
    }
}
