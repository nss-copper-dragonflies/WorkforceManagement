using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using BangazonWorkForce.Models;
using BangazonWorkForce.Models.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
//AUTHOR: JORDAN ROSAS Controller for computer will have the Get, Details, Add, Delete
namespace BangazonWorkForce.Controllers
{
    public class ComputerController : Controller
    {
        private readonly IConfiguration _config;

        public ComputerController(IConfiguration config)
        {
            this._config = config;
        }

        public SqlConnection Connection
        {
            get
            {
                return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            }
        }
        // GET: Computer
        //Get method will return the list of computers makes and manufacturers
        public ActionResult Index()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT id, Make, Manufacturer, purchaseDate
                                        From Computer";

                    SqlDataReader reader = cmd.ExecuteReader();
                    List<Computer> computerList = new List<Computer>();
                    while (reader.Read())
                    {
                        Computer computer = new Computer
                        {
                            id = reader.GetInt32(reader.GetOrdinal("id")),
                            Make = reader.GetString(reader.GetOrdinal("Make")),
                            Manufacturer = reader.GetString(reader.GetOrdinal("Manufacturer")),
                            purchaseDate = reader.GetDateTime(reader.GetOrdinal("purchaseDate"))
                        };
                        computerList.Add(computer);
                    }
                    reader.Close();             
                    return View(computerList);
                }
            }
        }

        // GET: Computer/Details/5
        public ActionResult Details(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"select id, Make, Manufacturer, PurchaseDate 
                                        from computer
                                        where Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();

                    Computer computer = null;

                    if (reader.Read())
                    {
                        computer = new Computer
                        {
                            id = reader.GetInt32(reader.GetOrdinal("id")),
                            Make = reader.GetString(reader.GetOrdinal("Make")),
                            Manufacturer = reader.GetString(reader.GetOrdinal("Manufacturer")),
                            purchaseDate = reader.GetDateTime(reader.GetOrdinal("PurchaseDate"))
                        };
                    }
                    reader.Close();
                    return View(computer);
                }
            }
        }

        // GET: Computer/Create
        public ActionResult Create()
        {
            ComputerCreateViewModel viewModel =
            new ComputerCreateViewModel(_config.GetConnectionString("DefaultConnection"));
            return View(viewModel);
        }

        // POST: Computer/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Computer computer)
        {
            try
            {
                // TODO: Add insert logic here
                using(SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"INSERT INTO computer (Make, Manufacturer, PurchaseDate)
                                            VALUES (@make, @manufacturer, @purchaseDate)";
                        cmd.Parameters.Add(new SqlParameter("@make", computer.Make));
                        cmd.Parameters.Add(new SqlParameter("@manufacturer", computer.Manufacturer));
                        cmd.Parameters.Add(new SqlParameter("@purchaseDate", computer.purchaseDate));

                        cmd.ExecuteNonQuery();

                        return RedirectToAction(nameof(Index));

                    }
                }
            }
            catch
            {
                return View(computer);
            }
        }

        // GET: Computer/Delete/5
        public ActionResult Delete(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"select Make, Manufacturer, PurchaseDate 
                                        from computer
                                        where Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();

                    Computer computer = null;

                    if (reader.Read())
                    {
                        computer = new Computer
                        {
                            Make = reader.GetString(reader.GetOrdinal("Make")),
                            Manufacturer = reader.GetString(reader.GetOrdinal("Manufacturer")),
                            purchaseDate = reader.GetDateTime(reader.GetOrdinal("PurchaseDate"))
                        };
                    }
                    reader.Close();
                    return View(computer);
                }
            }
        }   

        // POST: Computer/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"DELETE from computer where id = @id 
                                            AND NOT exists(select EmployeeId from [ComputerEmployee] 
                                            WHERE EmployeeId = @id)";
                        cmd.Parameters.Add(new SqlParameter("@id", id));
                        cmd.ExecuteNonQuery();
                    }
                }
                    
                    return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}