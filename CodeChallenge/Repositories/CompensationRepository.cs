using System;
using System.Collections.Generic;
using System.Linq;
using CodeChallenge.Models;
using CodeChallenge.Data;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace CodeChallenge.Repositories
{
    public class CompensationRepository : ICompensationRepository
    {
        private readonly EmployeeContext _employeeContext;
        
        public CompensationRepository(EmployeeContext employeeContext)
        {
            _employeeContext = employeeContext;
        }
        
        public Compensation Add(Compensation compensation)
        {
            _employeeContext.Compensations.Add(compensation);
            return compensation;
        }

        public Compensation GetByEmployeeId(String employeeId)
        {
            return _employeeContext.Compensations
                .Include(e => e.Employee)
                .SingleOrDefault(c => c.Employee.EmployeeId == employeeId);
        }
        
        public Task SaveAsync()
        {
            return _employeeContext.SaveChangesAsync();
        }
    }
}