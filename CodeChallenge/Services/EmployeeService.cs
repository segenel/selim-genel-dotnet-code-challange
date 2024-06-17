using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeChallenge.Models;
using Microsoft.Extensions.Logging;
using CodeChallenge.Repositories;

namespace CodeChallenge.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly ILogger<EmployeeService> _logger;

        public EmployeeService(ILogger<EmployeeService> logger, IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
            _logger = logger;
        }

        public Employee Create(Employee employee)
        {
            if (employee != null)
            {
                // Ensure the DirectReports are valid
                ValidateDirectReports(employee);
                
                _employeeRepository.Add(employee);
                // Finally for each of the new employee's direct reports, assign their mangererIds
                if (employee.DirectReports != null)
                {
                    foreach (var directReport in employee.DirectReports)
                    {
                        directReport.ManagerId = employee.EmployeeId;
                    }
                }

                _employeeRepository.SaveAsync().Wait();
            }

            return employee;
        }

        public Employee GetById(string id)
        {
            if(!String.IsNullOrEmpty(id))
            {
                return _employeeRepository.GetById(id);
            }

            return null;
        }

        private void ValidateDirectReports(Employee employee)
        {
            if (employee.DirectReports != null && employee.DirectReports.Count > 0)
            {
                var referencedEmployees = new List<Employee>(employee.DirectReports.Count);

                foreach (var directReport in employee.DirectReports)
                {
                    // Fetch the existing direct report from the database
                    var existingDirectReport = _employeeRepository.GetById(directReport.EmployeeId);

                    if (existingDirectReport != null)
                    {
                        // Change the existing direct report's managerId and add to the list
                        existingDirectReport.ManagerId = employee.EmployeeId;
                        referencedEmployees.Add(existingDirectReport);
                    }
                    else
                    {
                        _logger.LogDebug($"Employee with id {directReport.EmployeeId} doesn't exist" +
                                         $"and was not added to the DirectReports of {employee.EmployeeId}");
                    }
                }

                // Assign the referenced employees to the employee's DirectReports
                employee.DirectReports = referencedEmployees;
            }
        }
        
        public Employee Replace(Employee originalEmployee, Employee newEmployee)
        {
            if(originalEmployee != null)
            {
                _employeeRepository.Remove(originalEmployee);
                if (newEmployee != null)
                {
                    // ensure the original has been removed, otherwise EF will complain another entity w/ same id already exists
                    _employeeRepository.SaveAsync().Wait();

                    _employeeRepository.Add(newEmployee);
                    // overwrite the new id with previous employee id
                    newEmployee.EmployeeId = originalEmployee.EmployeeId;
                }
                _employeeRepository.SaveAsync().Wait();
            }

            return newEmployee;
        }

        public ReportingStructure GetReportingStructure(String employeeId)
        {
            Employee employee = GetById(employeeId);

            if (employee == null)
            {
                return null;
            }

            int numberOfReports = CalculateNumberOfReports(employee);

            return new ReportingStructure
            {
                Employee = employee,
                NumberOfReports = numberOfReports
            };
        }

        private int CalculateNumberOfReports(Employee employee)
        {
            if (employee.DirectReports == null)
            {
                return 0;
            }

            int count = employee.DirectReports.Count;
            foreach (var report in employee.DirectReports)
            {
                var directReport = GetById(report.EmployeeId);
                count += CalculateNumberOfReports(directReport);
            }
            return count;
        }
    }
}
