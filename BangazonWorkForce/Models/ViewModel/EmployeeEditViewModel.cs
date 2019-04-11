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
            TrainingPrograms = new List<TrainingProgram>();
        }

        public Employee Employee { get; set; }
        public TrainingProgram trainingProgram { get; set; }
        public List<TrainingProgram> TrainingPrograms { get; set; }
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
