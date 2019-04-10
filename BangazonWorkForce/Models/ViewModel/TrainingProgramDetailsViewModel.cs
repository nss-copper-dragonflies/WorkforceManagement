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
        public TrainingProgramDetailsViewModel()
        {
            Employees = new List<Employee>();
        }

        public TrainingProgramDetailsViewModel(string connectionString)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT t.[Name], t.StartDate, 
                                                t.EndDate, t.MaxAttendees, 
                                                e.FirstName, e.LastName
	                                        FROM TrainingProgram t
	                                        INNER JOIN EmployeeTraining et ON t.Id = et.TrainingProgramId
	                                        INNER JOIN Employee e ON et.EmployeeId = e.Id;";
                    SqlDataReader reader = cmd.ExecuteReader();

                    Employees = new List<Employee>();

                    while (reader.Read())
                    {
                        Employees.Add(new Employee
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("e.Id")),
                            FirstName = reader.GetString(reader.GetOrdinal("e.FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("e.LastName"))
                        });
                    }
                    reader.Close();
                }
            }
        }


        public TrainingProgram TrainingProgram { get; set; }
        public List<Employee> Employees { get; set; }

        public List<SelectListItem> EmployeeOptions
        {
            get
            {
                return Employees.Select(e => new SelectListItem
                {
                    Value = e.Id.ToString(),
                    Text = ($"{e.FirstName} {e.LastName}")
                }).ToList();
            }
        }
    }
}
