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
            CurrentTrainingPrograms = new List<TrainingProgram>();
            Computers = new List<Computer>();
            allTrainingPrograms = new List<TrainingProgram>();
            Computer = new Computer();
        }

        public Employee Employee { get; set; }
        public TrainingProgram trainingProgram { get; set; }
        public Computer Computer { get; set; }
        public List<TrainingProgram> CurrentTrainingPrograms{ get; set; }
        public List<int> SelectedTrainingPrograms{ get; set; }
        public List<TrainingProgram> allTrainingPrograms { get; set; }
        public List<Computer> Computers { get; set; }
        public List<Department> Departments { get; set; }

        public EmployeeEditViewModel(string connectionString, int id)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT id, [Name] from TrainingProgram;";
                    SqlDataReader reader = cmd.ExecuteReader();

                    allTrainingPrograms = new List<TrainingProgram>();

                    while (reader.Read())
                    {
                        allTrainingPrograms.Add(new TrainingProgram
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
                    cmd.CommandText = @"SELECT c.id as cId, Make from Computer c
                                        left join ComputerEmployee ce on ce.ComputerId = c.Id
                                        where ce.EmployeeId is null or ce.employeeid = @id or ce.UnassignDate is not null ";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();

                    Computers = new List<Computer>();

                    while (reader.Read())
                    {
                        Computers.Add(new Computer
                        {
                            id = reader.GetInt32(reader.GetOrdinal("cId")),
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

                return Computers.Select(c => new SelectListItem
                {
                    Value = c.id.ToString(),
                    Text = c.Make
                }).ToList();
            }
        }
        
       
        public List<SelectListItem> TrainingProgramOptions
        {
            get
            {
                if (allTrainingPrograms == null)
                {
                    return null;
                }



                return allTrainingPrograms.Select(tp => new SelectListItem
                {
                    Value = tp.Id.ToString(),
                    Text = tp.Name
                }).ToList();
            }
        }
        //list ints youretaininglist .select
        //new select list lit item of all alltraining foreacg id in list int if it matches the dropdown use Selected

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
