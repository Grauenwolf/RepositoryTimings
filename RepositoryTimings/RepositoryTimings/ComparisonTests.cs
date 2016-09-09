using BenchmarkDotNet.Attributes;
using System.Configuration;
using System.Linq;
using Tortuga.Chain;

namespace RepositoryTimings
{
    public class ComparisonTests
    {
        readonly EmployeeRepositoryDapper m_DapperRepo;
        readonly EmployeeRepositoryChain m_ChainRepo;
        readonly EmployeeRepositoryChainCompiled m_ChainCompiledRepo;
        readonly EmployeeRepositoryEF_Intermediate m_EFIntermediateRepo;
        readonly EmployeeRepositoryEF_Intermediate_NoTrack m_EFIntermediateNoTrackRepo;
        readonly EmployeeRepositoryEF_Novice m_EFNoviceRepo;
        readonly SqlServerDataSource m_DataSource;

        public ComparisonTests()
        {
            m_DataSource = SqlServerDataSource.CreateFromConfig("CodeFirstModels");
            m_DapperRepo = new EmployeeRepositoryDapper(ConfigurationManager.ConnectionStrings["CodeFirstModels"].ConnectionString);
            m_ChainRepo = new EmployeeRepositoryChain(SqlServerDataSource.CreateFromConfig("CodeFirstModels"));
            m_ChainCompiledRepo = new EmployeeRepositoryChainCompiled(SqlServerDataSource.CreateFromConfig("CodeFirstModels"));
            m_EFIntermediateRepo = new EmployeeRepositoryEF_Intermediate();
            m_EFIntermediateNoTrackRepo = new EmployeeRepositoryEF_Intermediate_NoTrack();
            m_EFNoviceRepo = new EmployeeRepositoryEF_Novice();
        }

        [Benchmark]
        public void Dapper() => CrudTestCore(m_DapperRepo);

        [Benchmark]
        public void Chain() => CrudTestCore(m_ChainRepo);

        [Benchmark]
        public void ChainCompiled() => CrudTestCore(m_ChainCompiledRepo);

        [Benchmark]
        public void EFIntermediateRepo() => CrudTestCore(m_EFIntermediateRepo);

        [Benchmark]
        public void EFIntermediateNoTrackRepo() => CrudTestCore(m_EFIntermediateNoTrackRepo);

        [Benchmark]
        public void EFNoviceRepo() => CrudTestCore(m_EFNoviceRepo);

        
        static void CrudTestCore(ISimpleEmployeeRepository repo)
        {
            //Create an employee
            var emp1 = new Employee() { FirstName = "Tom", LastName = "Jones", Title = "President" };
            var employeeKey1 = repo.Insert(emp1);
            var echo1 = repo.Get(employeeKey1);

            //Assert.AreNotEqual(0, echo1.EmployeeKey, "EmployeeKey was not set");
            //Assert.AreEqual(emp1.FirstName, echo1.FirstName, "FirstName");
            //Assert.AreEqual(emp1.LastName, echo1.LastName, "LastName");
            //Assert.AreEqual(emp1.Title, echo1.Title, "Title");

            //Update that employee
            echo1.MiddleName = "G";
            repo.Update(echo1);
            var echo1b = repo.Get(employeeKey1);
            //Assert.AreEqual("G", echo1b.MiddleName);

            //Create second employee
            var emp2 = new Employee() { FirstName = "Lisa", LastName = "Green", Title = "VP Transportation", ManagerKey = echo1.EmployeeKey };
            var echo2 = repo.InsertAndReturn(emp2);

            //Assert.AreNotEqual(0, echo2.EmployeeKey, "EmployeeKey was not set");
            //Assert.AreEqual(emp2.FirstName, echo2.FirstName, "FirstName");
            //Assert.AreEqual(emp2.LastName, echo2.LastName, "LastName");
            //Assert.AreEqual(emp2.Title, echo2.Title, "Title");
            //Assert.AreEqual(emp2.ManagerKey, echo2.ManagerKey, "ManagerKey");
            //Assert.IsNotNull(echo2.CreatedDate);

            //Get the list of employees
            var list = repo.GetAll();
            //Assert.IsTrue(list.Any(e => e.EmployeeKey == echo1.EmployeeKey), "Employee 1 is missing");
            //Assert.IsTrue(list.Any(e => e.EmployeeKey == echo2.EmployeeKey), "Employee 2 is missing");

            //Search for an employee
            var whereSearch1 = repo.GetByManager(employeeKey1);
            //Assert.IsFalse(whereSearch1.Any(x => x.EmployeeKey == echo1.EmployeeKey), "Emp1 should not have been returned");
            //Assert.IsTrue(whereSearch1.Any(x => x.EmployeeKey == echo2.EmployeeKey), "Emp2 should have been returned");

            //Get the phone list
            var projection = repo.GetOfficePhoneNumbers();
            //Assert.IsTrue(projection.Any(e => e.EmployeeKey == echo1.EmployeeKey), "Employee 1 is missing");
            //Assert.IsTrue(projection.Any(e => e.EmployeeKey == echo2.EmployeeKey), "Employee 2 is missing");

            //Update from a projected model
            var projection1 = projection.Single(e => e.EmployeeKey == echo1.EmployeeKey);
            projection1.OfficePhone = "123-456-7890";
            repo.Update(projection1);

            //Verify update
            var echo1c = repo.Get(employeeKey1);
            //Assert.AreEqual("123-456-7890", echo1c.OfficePhone);

            //Delete records
            repo.Delete(echo2.EmployeeKey);
            repo.Delete(echo1.EmployeeKey);

            //Verify deletion
            var list2 = repo.GetAll();
            //Assert.AreEqual(list.Count - 2, list2.Count);
        }
    }
}
