using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeChallenge.Models;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using CodeChallenge.Data;

namespace CodeChallenge.Repositories
{
    public class EmployeeRespository : IEmployeeRepository
    {
        private readonly EmployeeContext _employeeContext;
        private readonly ILogger<IEmployeeRepository> _logger;

        public EmployeeRespository(ILogger<IEmployeeRepository> logger, EmployeeContext employeeContext)
        {
            _employeeContext = employeeContext;
            _logger = logger;
        }

        public Employee Add(Employee employee)
        {
            employee.EmployeeId = Guid.NewGuid().ToString();
            // If the employee has a manager (ManagerId not null) add employee to Manager's DirectReports
            if (employee.ManagerId != null)
            {
                Employee manager = GetById(employee.ManagerId);
                if (manager.DirectReports == null){
                    manager.DirectReports = new List<Employee>();
                }
                
                // Check and remove direct reports of the new employee from the manager's direct reports as employee is their new manager
                if (employee.DirectReports != null && employee.DirectReports.Count > 0)
                {
                    var employeeDirectReportsIds = employee.DirectReports.Select(dr => dr.EmployeeId).ToHashSet();
                    manager.DirectReports.RemoveAll(dr => employeeDirectReportsIds.Contains(dr.EmployeeId));
                }
                
                manager.DirectReports.Add(employee);
            }
            _employeeContext.Employees.Add(employee);
            return employee;
        }

        public Employee GetById(string id)
        {
            return _employeeContext.Employees
                .Include(e => e.DirectReports) // Ensuring that directReports are included
                .SingleOrDefault(e => e.EmployeeId == id);
        }

        public Task SaveAsync()
        {
            return _employeeContext.SaveChangesAsync();
        }

        public Employee Remove(Employee employee)
        {
            return _employeeContext.Remove(employee).Entity;
        }
    }
}
