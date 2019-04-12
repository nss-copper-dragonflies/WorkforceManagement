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
            using (SqlConnection conn = new SqlConnection(connectionString)
            {
                conn.Open();
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"Select e.FirstName, e.LastName
                                        From Employee";

                SqlDataReader reader = cmd.ExecuteReader();

            }
        }
    }
}
