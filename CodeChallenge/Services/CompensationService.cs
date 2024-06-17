using System;
using CodeChallenge.Models;
using CodeChallenge.Repositories;
using Microsoft.Extensions.Logging;

namespace CodeChallenge.Services
{
    public class CompensationService : ICompensationService
    {
        private readonly ICompensationRepository _compensationRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly ILogger<EmployeeService> _logger;

        public CompensationService(ICompensationRepository compensationRepository,
            IEmployeeRepository employeeRepository, ILogger<EmployeeService> logger)
        {
            _compensationRepository = compensationRepository;
            _employeeRepository = employeeRepository;
            _logger = logger;
        }

        public Compensation Create(Compensation compensation)
        {
            if (compensation != null)
            {
                // Validate the employee
                var employee = _employeeRepository.GetById(compensation.Employee.EmployeeId);
                if (employee == null)
                {
                    _logger.LogDebug($"Employee with id { compensation.Employee.EmployeeId }");
                    return null;
                }
                
                // If a compensation already exists for this employee, don't create
                var existingCompensation = GetByEmployeeId(compensation.Employee.EmployeeId);
                if (existingCompensation != null)
                {
                    _logger.LogDebug($"Employee with id { compensation.Employee.EmployeeId } already has a compensation");
                    return null;
                }

                // Assign the valid employee reference
                compensation.Employee = employee;

                // Add the compensation to the repository
                _compensationRepository.Add(compensation);

                // Save changes asynchronously
                _compensationRepository.SaveAsync().Wait();
            }

            return compensation;
        }

        public Compensation GetByEmployeeId(String employeeId)
        {
            return _compensationRepository.GetByEmployeeId(employeeId);
        }
    }
}