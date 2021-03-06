﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BangazonWorkForce.Models
{
    public class Employee
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string FullName
        {
            get
            {
                return $"{FirstName} {LastName}";
            }
        }

        public bool IsSupervisor { get; set; }
        public int DepartmentId { get; set; }
        public Department Department { get; set; }
        public Computer Computer { get; set; } = new Computer();
        public List<TrainingProgram> CurrentTrainingPrograms { get; set; }
        public List<TrainingProgram> allTrainingPrograms { get; set; }


    }
}