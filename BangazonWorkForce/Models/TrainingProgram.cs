using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BangazonWorkForce.Models
{
    public class TrainingProgram
    {
        public int id { get; set; }
        public string Name { get; set; }
        public DateTime StartDate { get; set; } = new DateTime();
        public DateTime EndDate { get; set; } = new DateTime();
        public int MaxAttendees { get; set; }
    }
}
