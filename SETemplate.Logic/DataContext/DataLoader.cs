//@Ignore
using Azure.Identity;
using SETemplate.Logic.Entities.Develop;
using SETemplate.Logic.Entities.Develop.BaseData;

namespace SETemplate.Logic.DataContext
{
    /// <summary>
    /// Provides methods to load data from CSV files.
    /// </summary>
    public static class DataLoader
    {
        #region methods

        /// <summary>
        /// Loads companies from a CSV file.
        /// </summary>
        /// <param name="path">The path to the CSV file.</param>
        /// <returns>A list of companies.</returns>
        public static List<Company> LoadCompaniesFromCsv(string path)
        {
            var result = new List<Company>();

            result.AddRange(File.ReadAllLines(path)
                       .Skip(1)
                       .Select(l => l.Split(';'))
                       .Select(d => new Company
                       {
                           Name = d[0],
                           Address = d[1],
                       }));
            return result;
        }

        /// <summary>
        /// Loads customers from a CSV file.
        /// </summary>
        /// <param name="path">The path to the CSV file.</param>
        /// <returns>A list of customers.</returns>
        public static List<Customer> LoadCustomersFromCsv(string path)
        {
            var result = new List<Customer>();

            result.AddRange(File.ReadAllLines(path)
                       .Skip(1)
                       .Select(l => l.Split(';'))
                       .Select(d => new Customer
                       {
                           CompanyId = Convert.ToInt32(d[0]),
                           Name = d[1],
                           Email = d[2],
                       }));
            return result;
        }

        /// <summary>
        /// Loads employees from a CSV file.
        /// </summary>
        /// <param name="path">The path to the CSV file.</param>
        /// <returns>A list of employees.</returns>
        public static List<Employee> LoadEmployeesFromCsv(string path)
        {
            var result = new List<Employee>();

            result.AddRange(File.ReadAllLines(path)
                       .Skip(1)
                       .Select(l => l.Split(';'))
                       .Select(d => new Employee
                       {
                           CompanyId = Convert.ToInt32(d[0]),
                           FirstName = d[1],
                           LastName = d[2],
                           Email = d[3],
                       }));
            return result;
        }
        #endregion methods
    }
}
