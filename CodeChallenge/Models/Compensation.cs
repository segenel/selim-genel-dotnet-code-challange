using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CodeChallenge.Models
{
    public class Compensation
    {
        [Key, ForeignKey("Employee")]
        private string EmployeeId { get; set; } // Primary key and foreign key

        public Employee Employee { get; set; }

        [Required]
        public decimal Salary { get; set; }

        [Required]
        public DateTime EffectiveDate { get; set; }
    }
}