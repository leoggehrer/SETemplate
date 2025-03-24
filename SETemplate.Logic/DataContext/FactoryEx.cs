//@Ignore
#if GENERATEDCODE_ON
namespace SETemplate.Logic.DataContext
{
    partial class Factory
    {
        static partial void AfterInitDatabase()
        {
            var path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)!;
            var context = CreateContext();

            var companies = DataLoader.LoadCompaniesFromCsv(Path.Combine(path, "Data", "companies.csv"));

            companies.ToList().ForEach(e => context.CompanySet.Add(e));
            context.SaveChanges();

            var customers = DataLoader.LoadCustomersFromCsv(Path.Combine(path, "Data", "customers.csv"));
            customers.ToList().ForEach(e => context.CustomerSet.Add(e));

            var employees = DataLoader.LoadEmployeesFromCsv(Path.Combine(path, "Data", "employees.csv"));
            employees.ToList().ForEach(e => context.EmployeeSet.Add(e));

            context.SaveChanges();
        }
    }
}
#endif