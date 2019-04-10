
using BangazonWorkForce.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BangazonAPI.Models
{
    public class EmployeeTraining
    {
        public int Id { get; set; }

        public int EmployeeId { get; set; }

        public Employee Employee { get; set; }

        public int TrainingProgramId { get; set; }

        public TrainingProgram TrainingProgram { get; set; }

    }
}
