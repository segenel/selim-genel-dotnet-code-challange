
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using CodeChallenge.Models;

using CodeCodeChallenge.Tests.Integration.Extensions;
using CodeCodeChallenge.Tests.Integration.Helpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CodeCodeChallenge.Tests.Integration
{
    [TestClass]
    public class EmployeeControllerTests
    {
        private static HttpClient _httpClient;
        private static TestServer _testServer;

        [ClassInitialize]
        // Attribute ClassInitialize requires this signature
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "<Pending>")]
        public static void InitializeClass(TestContext context)
        {
            _testServer = new TestServer();
        }

        [ClassCleanup]
        public static void CleanUpClass()
        {
            _testServer.Dispose();
        }
        
        [TestInitialize]
        public void InitializeTest()
        {
            _testServer = new TestServer();
            _httpClient = _testServer.NewClient();
        }
        
        [TestCleanup]
        public void CleanUpTest()
        {
            _httpClient.Dispose();
            _testServer.Dispose();
        }
        
        [TestMethod]
        public void CreateEmployee_Returns_Created()
        {
            // Arrange
            var employee = new Employee()
            {
                Department = "Complaints",
                FirstName = "Debbie",
                LastName = "Downer",
                Position = "Receiver",
                ManagerId = "62c1084e-6e34-4630-93fd-9153afb65309",
            };

            var requestContent = new JsonSerialization().ToJson(employee);

            // Execute
            var postRequestTask = _httpClient.PostAsync("api/employee",
               new StringContent(requestContent, Encoding.UTF8, "application/json"));
            var response = postRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);

            var newEmployee = response.DeserializeContent<Employee>();
            Assert.IsNotNull(newEmployee.EmployeeId);
            Assert.AreEqual(employee.FirstName, newEmployee.FirstName);
            Assert.AreEqual(employee.LastName, newEmployee.LastName);
            Assert.AreEqual(employee.Department, newEmployee.Department);
            Assert.AreEqual(employee.Position, newEmployee.Position);
            Assert.AreEqual(employee.ManagerId, newEmployee.ManagerId);
        }

        [TestMethod]
        public void GetEmployeeById_Returns_Ok()
        {
            // Arrange
            var employeeId = "16a596ae-edd3-4847-99fe-c4518e82c86f";
            var expectedFirstName = "John";
            var expectedLastName = "Lennon";

            // Execute
            var getRequestTask = _httpClient.GetAsync($"api/employee/{employeeId}");
            var response = getRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var employee = response.DeserializeContent<Employee>();
            Assert.AreEqual(expectedFirstName, employee.FirstName);
            Assert.AreEqual(expectedLastName, employee.LastName);
        }

        [TestMethod]
        public void UpdateEmployee_Returns_Ok()
        {
            // Arrange
            var employee = new Employee()
            {
                EmployeeId = "03aa1462-ffa9-4978-901b-7c001562cf6f",
                Department = "Engineering",
                FirstName = "Pete",
                LastName = "Best",
                Position = "Developer VI",
                ManagerId = "16a596ae-edd3-4847-99fe-c4518e82c86f"
            };
            var requestContent = new JsonSerialization().ToJson(employee);

            // Execute
            var putRequestTask = _httpClient.PutAsync($"api/employee/{employee.EmployeeId}",
                new StringContent(requestContent, Encoding.UTF8, "application/json"));
            var putResponse = putRequestTask.Result;
            
            // Assert
            Assert.AreEqual(HttpStatusCode.OK, putResponse.StatusCode);
            var newEmployee = putResponse.DeserializeContent<Employee>();

            Assert.AreEqual(employee.FirstName, newEmployee.FirstName);
            Assert.AreEqual(employee.LastName, newEmployee.LastName);
            Assert.AreEqual(employee.ManagerId, newEmployee.ManagerId);
        }
        
        [TestMethod]
        public void UpdateEmployee_Returns_NotFound()
        {
            // Arrange
            var employee = new Employee()
            {
                EmployeeId = "Invalid_Id",
                Department = "Music",
                FirstName = "Sunny",
                LastName = "Bono",
                Position = "Singer/Song Writer",
            };
            var requestContent = new JsonSerialization().ToJson(employee);

            // Execute
            var postRequestTask = _httpClient.PutAsync($"api/employee/{employee.EmployeeId}",
               new StringContent(requestContent, Encoding.UTF8, "application/json"));
            var response = postRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }
        
        [TestMethod]
        public async Task GetReportingStructure_InvalidEmployeeId_Returns_NotFound()
        {
            // Arrange
            var invalidEmployeeId = "non-existent-id";

            // Execute
            var response = await _httpClient.GetAsync($"api/employee/reportingStructure/{invalidEmployeeId}");

            // Assert
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }

        [TestMethod]
        public async Task GetReportingStructure_NoDirectReports_Returns_Zero()
        {
            // Arrange
            var employeeId = "62c1084e-6e34-4630-93fd-9153afb65309";

            // Execute
            var response = await _httpClient.GetAsync($"api/employee/reportingStructure/{employeeId}");

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var reportingStructure = response.DeserializeContent<ReportingStructure>();
            Assert.IsNotNull(reportingStructure.Employee);
            Assert.AreEqual(employeeId, reportingStructure.Employee.EmployeeId);
            Assert.AreEqual(0, reportingStructure.NumberOfReports);
        }
        
        [TestMethod]
        public async Task GetReportingStructure_Multiple_Levels_Of_Reports_Returns_CorrectCount()
        {
            // Arrange
            var employeeId = "16a596ae-edd3-4847-99fe-c4518e82c86f";
            var expectedNumberOfReports = 4;

            // Execute
            var response = await _httpClient.GetAsync($"api/employee/reportingStructure/{employeeId}");

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var reportingStructure = response.DeserializeContent<ReportingStructure>();
            Assert.IsNotNull(reportingStructure.Employee);
            Assert.AreEqual(employeeId, reportingStructure.Employee.EmployeeId);
            Assert.AreEqual(expectedNumberOfReports, reportingStructure.NumberOfReports);
        }
    }
}
