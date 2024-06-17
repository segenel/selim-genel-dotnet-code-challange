using System;
using Microsoft.AspNetCore.Mvc;
using CodeChallenge.Models;
using CodeChallenge.Services;
using Microsoft.Extensions.Logging;

namespace CodeChallenge.Controllers
{
    [ApiController]
    [Route("api/compensation")]
    public class CompensationController : ControllerBase
    {
        public readonly ILogger _logger;
        private readonly ICompensationService _compensationService;

        public CompensationController(ILogger<CompensationController> logger, ICompensationService compensationService)
        {
            _logger = logger;
            _compensationService = compensationService;
        }

        [HttpPost]
        public IActionResult Create([FromBody] Compensation compensation)
        {
            _logger.LogDebug($"Received compensation create request for");
            
            var newCompensation = _compensationService.Create(compensation);
            if (newCompensation == null)
            {
                return BadRequest("Error creating compensation.");
            }
            
            _logger.LogDebug($"Created compensation for { compensation.Employee.FirstName } {compensation.Employee.LastName}");
            
            return CreatedAtRoute("GetCompensationByEmployeeId", new { id = compensation.Employee.EmployeeId }, compensation);
        }

        [HttpGet("{id}", Name = "GetCompensationByEmployeeId")]
        public IActionResult GetByEmployeeId(String id)
        {
            _logger.LogDebug($"Received compensation get request for '{ id }'");

            var compensation = _compensationService.GetByEmployeeId(id);
            if (compensation == null)
            {
                return NotFound();
            }
            return Ok(compensation);
        }
    }
}