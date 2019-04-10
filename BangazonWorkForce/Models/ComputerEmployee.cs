using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BangazonAPI.Models
{
    public class ComputerEmployee
    {
        public int Id { get; set; }

        public int EmployeeId { get; set; }

        public Employee Employee { get; set; }

        public int ComputerId { get; set; }

        public Computer Computer { get; set; }

        public DateTime AssignDate { get; set; }

        public DateTime? UnassignDate { get; set; }
    }
}
