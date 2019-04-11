using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace BangazonWorkForce.Models.ViewModel
{
    public class TrainingProgramDetailsViewModel
    {
            public TrainingProgram TrainingProgram { get; set; }
            public List<Employee> Employees { get; set; }
    }
}
