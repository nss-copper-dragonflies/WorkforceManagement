﻿using BangazonAPI.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BangazonWorkForce.Models
{
    public class Computer
    {
        public int id { get; set; }

        public string Make { get; set; }

        public string Manufacturer { get; set; }    
        [Display(Name = "Purchase Date")]
        public DateTime purchaseDate { get; set; }

        public ComputerEmployee ComputerEmployee { get; set; }

        public DateTime? DecommisionDate { get; set; }

        [Display(Name = "Employees")]
        public string employees { get; set; }

        public Employee employeeObj { get; set; }
    }
}
