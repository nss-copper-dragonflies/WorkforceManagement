using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace BangazonWorkForce.Models.ViewModel
{
    public class ComputerCreateViewModel
    {
        public ComputerCreateViewModel()
        {
            Employees = new List<Employee>();
        }

        public ComputerCreateViewModel(string connectionString)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"select id, FirstName, LastName 
                                        From Employee";
                    SqlDataReader reader = cmd.ExecuteReader();

                    Employees = new List<Employee>();
                    while (reader.Read())
                    {
                        Employees.Add(new Employee
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName"))
                        });
                    }
                    reader.Close();
                }
            }
        }
        public Computer computer { get; set; }
        public List<Employee> Employees { get; set; }

        public List<SelectListItem> employeeOptions
        {
            get
            {
                return Employees.Select(e => new SelectListItem
                {
                    Value = e.Id.ToString(),
                    Text = $"{e.FirstName} {e.LastName}"

                }).ToList();
            }
        }



    }
}
