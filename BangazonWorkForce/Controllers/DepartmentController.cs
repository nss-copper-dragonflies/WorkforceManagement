using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using BangazonWorkForce.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace BangazonWorkForce.Controllers
{
    public class DepartmentController : Controller
    {
        private readonly IConfiguration _config;

        public DepartmentController(IConfiguration config)
        {
            _config = config;
        }

        public SqlConnection Connection
        {
            get
            {
                return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            }
        }
        // GET: Department
        public ActionResult Index()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT d.Id, d.[name], d.Budget, e.Id as employeeId, e.FirstName
                                        FROM Department d left join Employee e
                                        ON e.DepartmentId = d.Id";
                    SqlDataReader reader = cmd.ExecuteReader();
                    Dictionary<int, Department> departmentDictionary = new Dictionary<int, Department>();
                    while (reader.Read())
                    {
                        int deptId = reader.GetInt32(reader.GetOrdinal("Id"));
                        if (!departmentDictionary.ContainsKey(deptId))
                        {
                            Department department = new Department
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Name = reader.GetString(reader.GetOrdinal("name")),
                                Budget = reader.GetInt32(reader.GetOrdinal("Budget")),
                            };
                            departmentDictionary.Add(deptId, department);
                        }
                        if (!reader.IsDBNull(reader.GetOrdinal("employeeId")))
                        {
                            Department currentDepartment = departmentDictionary[deptId];
                            currentDepartment.employeeList.Add(
                            new Employee
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("employeeId")),
                                FirstName = reader.GetString(reader.GetOrdinal("FirstName"))  
                                
                            });
                        }
                        
                    }
                    reader.Close();

                    return View(departmentDictionary.Values.ToList());
                }
            }
        }

        // GET: Department/Details/5
        public ActionResult Details(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT d.Id, d.[name], d.Budget, e.Id as employeeId, e.FirstName
                                        FROM Department d 
                                        join Employee e
                                        ON e.DepartmentId = d.Id
                                        where d.id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();
                    Department department = null;

                    while (reader.Read())
                    {
                        if (department == null)
                        {
                            department = new Department
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Name = reader.GetString(reader.GetOrdinal("name")),
                                Budget = reader.GetInt32(reader.GetOrdinal("Budget"))

                            };

                        }
                        if (!reader.IsDBNull(reader.GetOrdinal("employeeId")))
                        {
                            int employeeId = reader.GetInt32(reader.GetOrdinal("employeeId"));
                            if (!department.employeeList.Any(e => e.Id == employeeId))
                                department.employeeList.Add(new Employee
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("employeeId")),
                                    FirstName = reader.GetString(reader.GetOrdinal("FirstName"))
                                });
                        }
                    }
                    reader.Close();

                    return View(department);
                }
            }
        }

        // GET: Department/Create
        public ActionResult Create()
        {
            return View();    
        }

        // POST: Department/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Department department)
        {
            try
            {
                // TODO: Add insert logic here
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"INSERT INTO Department (Name, Budget)
                                            Values (@name, @budget)";
                        cmd.Parameters.Add(new SqlParameter("@name", department.Name));
                        cmd.Parameters.Add(new SqlParameter("@budget", department.Budget));

                        cmd.ExecuteNonQuery();

                        return RedirectToAction(nameof(Index));
                    }
                }
            }
            catch
            {
                return View(department);
            }
        }
       
    }   
}