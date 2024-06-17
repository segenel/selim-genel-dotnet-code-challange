using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using CodeChallenge.Models;

using CodeCodeChallenge.Tests.Integration.Extensions;
using CodeCodeChallenge.Tests.Integration.Helpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace CodeChallenge.Tests.Integration
{
    [TestClass]
    public class CompensationControllerTests
    {
        private static HttpClient _httpClient;
        private static TestServer _testServer;

        [ClassInitialize]
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
            _httpClient = _testServer.NewClient();
        }
        
        [TestCleanup]
        public void CleanUpTest()
        {
            _httpClient.Dispose();
        }

        [TestMethod]
        public async Task CreateCompensation_Returns_Created()
        {
            // Arrange
            var compensation = new Compensation()
            {
                Employee = new Employee { EmployeeId = "16a596ae-edd3-4847-99fe-c4518e82c86f" },
                Salary = 75000,
                EffectiveDate = new DateTime(2023, 6, 1)
            };
            var requestContent = JsonConvert.SerializeObject(compensation);

            // Execute
            var response = await _httpClient.PostAsync("api/compensation",
                new StringContent(requestContent, Encoding.UTF8, "application/json"));

            // Assert
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
            var newCompensation = response.DeserializeContent<Compensation>();
            Assert.IsNotNull(newCompensation);
            Assert.AreEqual(compensation.Salary, newCompensation.Salary);
            Assert.AreEqual(compensation.EffectiveDate, newCompensation.EffectiveDate);
            Assert.AreEqual(compensation.Employee.EmployeeId, newCompensation.Employee.EmployeeId);
        }

        [TestMethod]
        public async Task CreateCompensation_Returns_BadRequest_When_Invalid()
        {
            // Arrange
            var invalidCompensation = new Compensation(); // Missing required fields
            var requestContent = JsonConvert.SerializeObject(invalidCompensation);

            // Execute
            var response = await _httpClient.PostAsync("api/compensation",
                new StringContent(requestContent, Encoding.UTF8, "application/json"));

            // Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [TestMethod]
        public async Task GetCompensationById_Returns_Ok()
        {
            // Arrange
            var employeeId = "16a596ae-edd3-4847-99fe-c4518e82c86f";

            // Execute
            var response = await _httpClient.GetAsync($"api/compensation/{employeeId}");

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var compensation = response.DeserializeContent<Compensation>();
            Assert.IsNotNull(compensation);
            Assert.AreEqual(employeeId, compensation.Employee.EmployeeId);
        }

        [TestMethod]
        public async Task GetCompensationById_Returns_NotFound_When_Invalid()
        {
            // Arrange
            var invalidEmployeeId = "non-existent-id";

            // Execute
            var response = await _httpClient.GetAsync($"api/compensation/{invalidEmployeeId}");

            // Assert
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
