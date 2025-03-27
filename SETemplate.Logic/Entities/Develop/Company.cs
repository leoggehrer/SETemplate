//@Ignore

//@Ignore
using SETemplate.Logic.Entities.Develop.BaseData;

namespace SETemplate.Logic.Entities.Develop
{
    /// <summary>
    /// Represents a company entity.
    /// </summary>
    [Table("Companies")]
    [Index(nameof(Name), IsUnique = true)]
    public partial class Company : EntityObject
    {
        #region properties
        /// <summary>
        /// Gets or sets the name of the company.
        /// </summary>
        [Required]
        [MaxLength(256)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the address of the company.
        /// </summary>
        [MaxLength(1024)]
        public string? Address { get; set; }

        /// <summary>
        /// Gets or sets the description of the company.
        /// </summary>
        [MaxLength(2048)]
        public string? Description { get; set; }
        #endregion properties

        #region navigation properties
        /// <summary>
        /// Gets or sets the list of customers associated with the company.
        /// </summary>
        public List<Customer>? Customers { get; set; } = [];
        /// <summary>
        /// Gets or sets the list of employees associated with the company.
        /// </summary>
        public List<Employee>? Employees { get; set; } = [];
        #endregion navigation properties

        #region methods
        /// <summary>
        /// Returns a string representation of the company.
        /// </summary>
        /// <returns>A string that represents the company.</returns>
        public override string ToString()
        {
            return $"Company: {Name}";
        }
        #endregion methods
    }
}
