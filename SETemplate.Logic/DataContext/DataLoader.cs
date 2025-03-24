//@Ignore
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
        public static List<Entities.Company> LoadCompaniesFromCsv(string path)
        {
            var result = new List<Entities.Company>();

            result.AddRange(File.ReadAllLines(path)
                       .Skip(1)
                       .Select(l => l.Split(';'))
                       .Select(d => new Entities.Company
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
        public static List<Entities.Customer> LoadCustomersFromCsv(string path)
        {
            var result = new List<Entities.Customer>();

            result.AddRange(File.ReadAllLines(path)
                       .Skip(1)
                       .Select(l => l.Split(';'))
                       .Select(d => new Entities.Customer
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
        public static List<Entities.BaseData.Employee> LoadEmployeesFromCsv(string path)
        {
            var result = new List<Entities.BaseData.Employee>();

            result.AddRange(File.ReadAllLines(path)
                       .Skip(1)
                       .Select(l => l.Split(';'))
                       .Select(d => new Entities.BaseData.Employee
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
