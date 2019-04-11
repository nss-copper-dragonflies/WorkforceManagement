using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace BangazonWorkForce.Models.ViewModel
{
    public class EmployeeEditViewModel
    {
        public EmployeeEditViewModel()
        {
            Departments = new List<Department>();
            TrainingPrograms = new List<TrainingProgram>();
            Computers = new List<Computer>();
        }

        public Employee Employee { get; set; }
        public TrainingProgram trainingProgram { get; set; }
        public Computer computer { get; set; }
        public List<TrainingProgram> TrainingPrograms { get; set; }
        public List<Computer> Computers { get; set; }
        public List<Department> Departments { get; set; }

        public EmployeeEditViewModel(string connectionString)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT id, [Name] from TrainingProgram;";
                    SqlDataReader reader = cmd.ExecuteReader();

                    TrainingPrograms = new List<TrainingProgram>();

                    while (reader.Read())
                    {
                        TrainingPrograms.Add(new TrainingProgram
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name"))
                        });
                    }
                    reader.Close();
                }
            }
            using (SqlConnection conn1 = new SqlConnection(connectionString))
            {
                conn1.Open();
                using (SqlCommand cmd = conn1.CreateCommand())
                {
                    cmd.CommandText = @"SELECT id, Make from Computer;";
                    SqlDataReader reader = cmd.ExecuteReader();

                    Computers = new List<Computer>();

                    while (reader.Read())
                    {
                        Computers.Add(new Computer
                        {
                            id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Make = reader.GetString(reader.GetOrdinal("Make"))
                        });
                    }
                    reader.Close();
                }
            }
        }
        public List<SelectListItem> ComputerOptions
        {
            get
            {
                if (Computers == null)
                {
                    return null;
                }

                return Computers.Select(d => new SelectListItem
                {
                    Value = d.id.ToString(),
                    Text = d.Make
                }).ToList();
            }
        }


        public List<SelectListItem> TrainingProgramOptions
        {
            get
            {
                if (TrainingPrograms == null)
                {
                    return null;
                }

                return TrainingPrograms.Select(d => new SelectListItem
                {
                    Value = d.Id.ToString(),
                    Text = d.Name
                }).ToList();
            }
        }

        public List<SelectListItem> DepartmentOptions
        {
            get
            {
                if (Departments == null)
                {
                    return null;
                }

                return Departments.Select(d => new SelectListItem
                {
                    Value = d.Id.ToString(),
                    Text = d.Name
                }).ToList();
            }
        }
    }
}
