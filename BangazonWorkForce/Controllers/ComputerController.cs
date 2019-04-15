using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using BangazonWorkForce.Models;
using BangazonWorkForce.Models.ViewModel;
using BangazonWorkForceManagement.Models.ViewModels;
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
                    cmd.CommandText = @"select c.id, c.make, c.manufacturer, c.PurchaseDate, e.FirstName, e.LastName
                                        from Computer c
                                        left join ComputerEmployee ce on ce.ComputerId = c.id
                                        left join Employee e on ce.EmployeeId = e.Id";

                    SqlDataReader reader = cmd.ExecuteReader();
                    List<Computer> computerList = new List<Computer>();
                    while (reader.Read())
                    {
                        if (!reader.IsDBNull(reader.GetOrdinal("FirstName")))
                        {

                            Computer computer = new Computer
                            {
                                id = reader.GetInt32(reader.GetOrdinal("id")),
                                Make = reader.GetString(reader.GetOrdinal("Make")),
                                Manufacturer = reader.GetString(reader.GetOrdinal("Manufacturer")),
                                purchaseDate = reader.GetDateTime(reader.GetOrdinal("purchaseDate")),
                                employeeObj = new Employee
                                {
                                    FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                    LastName = reader.GetString(reader.GetOrdinal("LastName"))
                                }

                            };
                            computerList.Add(computer);
                        }
                        else
                        {
                            Computer computer = new Computer
                            {
                                id = reader.GetInt32(reader.GetOrdinal("id")),
                                Make = reader.GetString(reader.GetOrdinal("Make")),
                                Manufacturer = reader.GetString(reader.GetOrdinal("Manufacturer")),
                                purchaseDate = reader.GetDateTime(reader.GetOrdinal("purchaseDate")),

                            };
                            computerList.Add(computer);
                        }

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
                    cmd.CommandText = @"select c.id, c.make, c.manufacturer, c.PurchaseDate, e.FirstName, e.LastName
                                        from Computer c
                                        left join ComputerEmployee ce on ce.ComputerId = c.id
                                        left join Employee e on ce.EmployeeId = e.Id
                                        where c.Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();

                    Computer computer = null;

                    while (reader.Read())
                    {
                        if (!reader.IsDBNull(reader.GetOrdinal("FirstName")))
                        {
                            computer = new Computer
                            {
                                id = reader.GetInt32(reader.GetOrdinal("id")),
                                Make = reader.GetString(reader.GetOrdinal("Make")),
                                Manufacturer = reader.GetString(reader.GetOrdinal("Manufacturer")),
                                purchaseDate = reader.GetDateTime(reader.GetOrdinal("PurchaseDate")),
                                employeeObj = new Employee
                                {
                                    FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                    LastName = reader.GetString(reader.GetOrdinal("LastName"))
                                }
                            };
                        }
                        else
                        {
                            computer = new Computer
                            {
                                id = reader.GetInt32(reader.GetOrdinal("id")),
                                Make = reader.GetString(reader.GetOrdinal("Make")),
                                Manufacturer = reader.GetString(reader.GetOrdinal("Manufacturer")),
                                purchaseDate = reader.GetDateTime(reader.GetOrdinal("PurchaseDate")),
                            };
                        }
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
        public ActionResult Create(ComputerCreateViewModel viewModel)
        {
            int employeeId = int.Parse(viewModel.employeeObj);
            //Employee employee = GetEmployeeList(employeeId);

            try
            {
                // TODO: Add insert logic here
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        if (viewModel.employeeObj != "0")
                        {
                            cmd.CommandText = @"INSERT INTO computer (Make, Manufacturer, PurchaseDate)
                                                OUTPUT INSERTED.Id      
                                                VALUES (@make, @manufacturer, @purchaseDate)";
                            cmd.Parameters.Add(new SqlParameter("@make", viewModel.computer.Make));
                            cmd.Parameters.Add(new SqlParameter("@manufacturer", viewModel.computer.Manufacturer));
                            cmd.Parameters.Add(new SqlParameter("@purchaseDate", viewModel.computer.purchaseDate));


                            int newId = (int)cmd.ExecuteScalar();
                            viewModel.computer.id = newId;

                            cmd.CommandText = @"INSERT INTO computerEmployee (EmployeeId, ComputerId, AssignDate)                                               
                                                VALUES (@employeeId, @computerId, getDate())";
                            cmd.Parameters.Add(new SqlParameter("@employeeId", employeeId));
                            cmd.Parameters.Add(new SqlParameter("@computerId", newId));

                            cmd.ExecuteNonQuery();

                        }
                        else
                        {
                            cmd.CommandText = @"INSERT INTO computer (Make, Manufacturer, PurchaseDate)
                                                OUTPUT INSERTED.Id      
                                                VALUES (@make, @manufacturer, @purchaseDate)";
                            cmd.Parameters.Add(new SqlParameter("@make", viewModel.computer.Make));
                            cmd.Parameters.Add(new SqlParameter("@manufacturer", viewModel.computer.Manufacturer));
                            cmd.Parameters.Add(new SqlParameter("@purchaseDate", viewModel.computer.purchaseDate));

                            int newId = (int)cmd.ExecuteScalar();
                            viewModel.computer.id = newId;
                        }
                        return RedirectToAction(nameof(Index));


                    }
                }
            }
            catch
            {
                return View(viewModel);
            }
        }

        // GET: Computer/Delete/5
        // GET: Computers/Delete/5
        public ActionResult Delete(int id)
        {
            Computer computer = GetComputerById(id);
            if (computer == null)
            {
                return NotFound();
            }

            using (SqlConnection conn = Connection)
            {
                int? assignedcomputer = null;
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT c.Id AS ComputerId,
                                        c.PurchaseDate, 
                                        c.Make, c.Manufacturer, ce.ComputerId as ComputerEmployeeCID
                                        FROM Computer c LEFT JOIN ComputerEmployee ce on c.id = ce.ComputerId
                                        WHERE c.Id = @id;";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    cmd.ExecuteNonQuery();
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())

                    assignedcomputer = reader.IsDBNull(reader.GetOrdinal("ComputerEmployeeCID")) ? (int?)null : (int?)reader.GetInt32(reader.GetOrdinal("ComputerEmployeeCID"));
                    ComputerDeleteViewModel viewModel = new ComputerDeleteViewModel
                    {
                        Id = id,
                        Make = computer.Make,
                        Manufacturer = computer.Manufacturer,
                        PurchaseDate = computer.purchaseDate,
                        ShouldDisplayDelete = assignedcomputer == null
                    };

                    reader.Close();
                    return View(viewModel);


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
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "DELETE FROM computer WHERE id = @id;";
                        cmd.Parameters.Add(new SqlParameter("@id", id));

                        cmd.ExecuteNonQuery();
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return RedirectToAction(nameof(Index));
            }
        }
        private Employee GetEmployeeList(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"select c.id, c.make, c.manufacturer, c.PurchaseDate, e.id employeeId, e.FirstName, e.LastName
                                        from Computer c
                                        left join ComputerEmployee ce on ce.ComputerId = c.id
                                        left join Employee e on ce.EmployeeId = e.Id
                                        where Id = @employeeId";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();

                    Employee employee = null;

                    if (reader.Read())
                    {
                        employee = new Employee
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName"))
                        };


                    }
                    return employee;
                }
            }
        }
        private Computer GetComputerById(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT Id,
                                               PurchaseDate, 
                                               Make, Manufacturer
                                               FROM Computer
                                        WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();

                    Computer computer = null;

                    if (reader.Read())
                    {
                        computer = new Computer
                        {
                            id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Make = reader.GetString(reader.GetOrdinal("Make")),
                            Manufacturer = reader.GetString(reader.GetOrdinal("Manufacturer")),
                            purchaseDate = reader.GetDateTime(reader.GetOrdinal("PurchaseDate")),
                        };
                    }
                    reader.Close();
                    return computer;
                }
            }
        }
    }
}