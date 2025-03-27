//@Ignore

//@Ignore
using SETemplate.Logic.Entities.Develop;

namespace SETemplate.Logic.Entities.Develop.BaseData
{
    /// <summary>
    /// Represents an employee entity.
    /// </summary>
    [Table("Employees")]
    [Index(nameof(Email), IsUnique = true)]
    public partial class Employee : EntityObject
    {
        #region properties
        /// <summary>
        /// Gets or sets the company ID.
        /// </summary>
        public int CompanyId { get; set; }

        /// <summary>
        /// Gets or sets the first name of the employee.
        /// </summary>
        public string FirstName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the last name of the employee.
        /// </summary>
        public string LastName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the email of the employee.
        /// </summary>
        public string Email { get; set; } = string.Empty;
        #endregion properties

        #region navigation properties
        /// <summary>
        /// Gets or sets the associated company.
        /// </summary>
        public Company? Company { get; set; }
        #endregion navigation properties

        #region methods
        /// <summary>
        /// Returns a string representation of the employee.
        /// </summary>
        /// <returns>A string representation of the employee.</returns>
        public override string ToString()
        {
            return $"{FirstName} {LastName}";
        }
        #endregion methods
    }
}
