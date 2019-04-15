using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using BangazonAPI.Models;
using BangazonWorkForce.Models;
using BangazonWorkForce.Models.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace BangazonWorkForce.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly IConfiguration _configuration;

        public EmployeeController(IConfiguration configuration)
        {
            this._configuration = configuration;
        }

        public SqlConnection Connection
        {
            get
            {
                return new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            }
        }
        // GET: Employee
        public ActionResult Index()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"select e.Id, d.Id as dId, e.FirstName, e.LastName, d.[Name] 
                                        from Employee e
                                        join department d on e.DepartmentId = d.Id";
                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Employee> employees = new List<Employee>();

                    while (reader.Read())
                    {
                        Employee employee = new Employee
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            DepartmentId = reader.GetInt32(reader.GetOrdinal("dId")),
                            Department = new Department
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("dId")),
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                            }
                        };
                        employees.Add(employee);
                    }

                    reader.Close();
                    return View(employees);
                }
            }
        }

        // GET: Employee/Details/5
        public ActionResult Details(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"select et.EmployeeId as etId, ec.EmployeeId as ecId, e.Id, d.Id as dId, e.FirstName, 
                                                e.LastName, d.[Name], c.Make, c.Manufacturer, tp.[Name] as tpName, tp.Id as tpId,
                                                tp.EndDate, tp.StartDate
                                                from Employee e
                                                left join department d on e.DepartmentId = d.Id
                                                left join ComputerEmployee ec on  ec.EmployeeId = e.Id
                                                left join Computer c on ec.ComputerId = c.Id
                                                left join EmployeeTraining et on et.EmployeeId = e.Id
                                                left join TrainingProgram tp on et.TrainingProgramId = tp.Id
                                                where e.Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();

                    Dictionary<int, Employee> employees = new Dictionary<int, Employee>();

                    while (reader.Read())
                    {
                        int employeeid = reader.GetInt32(reader.GetOrdinal("Id"));

                        if (!employees.ContainsKey(employeeid))
                        {

                            Employee employee = new Employee
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                DepartmentId = reader.GetInt32(reader.GetOrdinal("dId")),
                                CurrentTrainingPrograms = new List<TrainingProgram>(),
                                Computer = new Computer(),
                                Department = new Department
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("dId")),
                                    Name = reader.GetString(reader.GetOrdinal("Name")),
                                },
                            };
                            employees.Add(employeeid, employee);
                        };
                        if (!reader.IsDBNull(reader.GetOrdinal("ecId")))
                        {
                            Employee currentemployee = employees[employeeid];
                            currentemployee.Computer = new Computer
                            {
                                Make = reader.GetString(reader.GetOrdinal("Make")),
                                Manufacturer = reader.GetString(reader.GetOrdinal("Manufacturer")),
                            };
                        }
                        if (!reader.IsDBNull(reader.GetOrdinal("etId")))
                        {
                            Employee currentemployee = employees[employeeid];

                            if (!currentemployee.CurrentTrainingPrograms.Any(x => x.Id == reader.GetInt32(reader.GetOrdinal("tpId"))))
                            {
                                currentemployee.CurrentTrainingPrograms.Add(new TrainingProgram
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("tpId")),
                                    Name = reader.GetString(reader.GetOrdinal("tpName")),
                                    StartDate = reader.GetDateTime(reader.GetOrdinal("StartDate")),
                                    EndDate = reader.GetDateTime(reader.GetOrdinal("EndDate"))
                                });
                            }
                        }
                    }

                    reader.Close();

                    List<Employee> employeeDetail = employees.Values.ToList();

                    foreach (Employee e in employeeDetail)
                    {
                        return View(e);
                    }
                    return View();
                }
            }
        }

        public ActionResult Create()
        {
            EmployeeCreateViewModel viewModel =
                new EmployeeCreateViewModel(_configuration.GetConnectionString("DefaultConnection"));
            return View(viewModel);
        }

        // POST: Employee/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(EmployeeCreateViewModel viewModel)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"INSERT INTO Employee (firstname, lastname, IsSupervisor, departmentId)
                                            VALUES (@firstname, @lastname, @IsSupervisor, @departmentId)";
                        cmd.Parameters.Add(new SqlParameter("@firstname", viewModel.Employee.FirstName));
                        cmd.Parameters.Add(new SqlParameter("@lastname", viewModel.Employee.LastName));
                        cmd.Parameters.Add(new SqlParameter("@IsSupervisor", viewModel.Employee.IsSupervisor));
                        cmd.Parameters.Add(new SqlParameter("@departmentId", viewModel.Employee.DepartmentId));

                        cmd.ExecuteNonQuery();

                        return RedirectToAction(nameof(Index));
                    }
                }
            }
            catch
            {
                return View(viewModel);
            }
        }

        // GET: Employee/Edit/5
        public ActionResult Edit(int id)
        {
            List<Employee> employeeById = GetEmployeeList(id);

            foreach (Employee e in employeeById)
            {
                e.CurrentTrainingPrograms = GetYourTrainingPrograms(e.Id);
                if (e == null)
                {
                    return NotFound();
                }


                var currenttrainingPrograms = GetYourTrainingPrograms(e.Id);

                EmployeeEditViewModel viewModel = new EmployeeEditViewModel(_configuration.GetConnectionString("DefaultConnection"), e.Id)
                {
                    Departments = GetAllDepartments(),
                    Computers = GetAllComputers(e.Id),
                    Computer = GetComputerById(e.Id),
                    CurrentTrainingPrograms = currenttrainingPrograms,
                    SelectedTrainingPrograms = currenttrainingPrograms.Select(tp => tp.Id).ToList(),
                    allTrainingPrograms = GetAllTrainingPrograms(),
                    Employee = e,
                };
                return View(viewModel);
            }
            return View();

        }

        // POST: Employee/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, EmployeeEditViewModel viewModel)
        {
            Computer idcomputer = GetComputerById(id);

            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"UPDATE employee 
                                        SET firstname = @firstname, 
                                        lastname = @lastname,
                                        IsSuperVisor = @isupervisor, 
                                        DepartmentId = @departmentid
                                        where id = @id;

                                        DELETE from EmployeeTraining
                                        where EmployeeId = @id;";
                
                    
                    cmd.Parameters.Add(new SqlParameter("@firstname", viewModel.Employee.FirstName));
                    cmd.Parameters.Add(new SqlParameter("@lastname", viewModel.Employee.LastName));
                    cmd.Parameters.Add(new SqlParameter("@isupervisor", viewModel.Employee.IsSupervisor));
                    cmd.Parameters.Add(new SqlParameter("@departmentid", viewModel.Employee.DepartmentId));
                    cmd.Parameters.Add(new SqlParameter("@computerid", viewModel.Employee.Computer.id));
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                        
                    cmd.ExecuteNonQuery();

                    if ((idcomputer.id != 0) && (idcomputer.id != viewModel.Employee.Computer.id))
                    {
                        cmd.CommandText = @"INSERT into ComputerEmployee(employeeId, computerId, assignDate)
                                            values(@id, @computerid, GETDATE());

                                            UPDATE ComputerEmployee
                                            set unassignDate = GETDATE()
                                            WHERE Id = @Oldid;";

                        cmd.Parameters.Add(new SqlParameter("@Oldid", idcomputer.ComputerEmployee.Id));
                        cmd.ExecuteNonQuery();
                    }
                    else
                    {
                        cmd.CommandText = @"INSERT into ComputerEmployee(employeeId, computerId, assignDate)
                                            values(@id, @computerid, GETDATE());";
                        cmd.ExecuteNonQuery();
                    }


                    cmd.CommandText = @"INSERT into EmployeeTraining
                                        values(@id, @trainingprogramId);";

                    if (viewModel.SelectedTrainingPrograms != null)
                    {
                        foreach (int tpId in viewModel.SelectedTrainingPrograms)
                        {
                            cmd.Parameters.Clear();
                            cmd.Parameters.Add(new SqlParameter("@id", id));
                            cmd.Parameters.Add(new SqlParameter("@trainingprogramId", tpId));
                            cmd.ExecuteNonQuery();
                        }
                    }

                    return RedirectToAction(nameof(Index));
                }
            }
        }
           

        private List<Employee> GetEmployeeList(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"select et.EmployeeId as etId, ec.EmployeeId as ecId, e.Id, d.Id as dId, e.FirstName, e.LastName, d.[Name], c.Make, c.Manufacturer, tp.[Name] as tpName, tp.Id as tpId
                                        from Employee e
                                        left join department d on e.DepartmentId = d.Id
                                        left join ComputerEmployee ec on  ec.EmployeeId = e.Id
                                        left join Computer c on ec.ComputerId = c.Id
                                        left join EmployeeTraining et on et.EmployeeId = e.Id
                                        left join TrainingProgram tp on et.TrainingProgramId = tp.Id
                                        where e.Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();

                    Dictionary<int, Employee> employees = new Dictionary<int, Employee>();

                    while (reader.Read())
                    {
                        int employeeid = reader.GetInt32(reader.GetOrdinal("Id"));

                        if (!employees.ContainsKey(employeeid))
                        {

                            Employee employee = new Employee
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                DepartmentId = reader.GetInt32(reader.GetOrdinal("dId")),
                                CurrentTrainingPrograms = new List<TrainingProgram>(),
                                Computer = new Computer(),
                                Department = new Department
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("dId")),
                                    Name = reader.GetString(reader.GetOrdinal("Name")),
                                },
                            };
                            employees.Add(employeeid, employee);
                        };
                        if (!reader.IsDBNull(reader.GetOrdinal("ecId")))
                        {
                            Employee currentemployee = employees[employeeid];
                            currentemployee.Computer = new Computer
                            {
                                Make = reader.GetString(reader.GetOrdinal("Make")),
                                Manufacturer = reader.GetString(reader.GetOrdinal("Manufacturer")),
                            };
                        }
                        if (!reader.IsDBNull(reader.GetOrdinal("etId")))
                        {
                            Employee currentemployee = employees[employeeid];

                            if (!currentemployee.CurrentTrainingPrograms.Any(x => x.Id == reader.GetInt32(reader.GetOrdinal("tpId"))))
                            {
                                currentemployee.CurrentTrainingPrograms.Add(new TrainingProgram
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("tpId")),
                                    Name = reader.GetString(reader.GetOrdinal("tpName"))
                                });
                            }
                        }
                    }

                    reader.Close();

                    List<Employee> employeeDetail = employees.Values.ToList();

                    return employeeDetail;
                }
            }
        }

        private List<Department> GetAllDepartments()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT id, [Name] from Department;";
                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Department> deparments = new List<Department>();

                    while (reader.Read())
                    {
                        deparments.Add(new Department
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name"))
                        });
                    }
                    reader.Close();

                    return deparments;
                }
            }
        }
        private List<TrainingProgram> GetYourTrainingPrograms(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT tp.id as tpId, [Name], e.id from TrainingProgram tp
                                       left join employeetraining et on et.trainingprogramid = tp.id 
                                       left join employee e on et.EmployeeId = e.id
										where e.id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();

                    List<TrainingProgram> programs = new List<TrainingProgram>();

                    while (reader.Read())
                    {
                        programs.Add(new TrainingProgram
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("tpId")),
                            Name = reader.GetString(reader.GetOrdinal("Name"))
                        });
                    }
                    reader.Close();

                    return programs;
                }
            }

        }

        private List<Computer> GetAllComputers(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT c.id as cId, Make from Computer c
                                        left join ComputerEmployee ce on ce.ComputerId = c.Id
                                        where ce.EmployeeId is null or ce.employeeid = @id or ce.UnassignDate is not null ";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Computer> Computers = new List<Computer>();

                    while (reader.Read())
                    {
                        Computers.Add(new Computer
                        {
                            id = reader.GetInt32(reader.GetOrdinal("cId")),
                            Make = reader.GetString(reader.GetOrdinal("Make"))
                        });
                    }
                    reader.Close();
                    return Computers;
                }
            }
        }
        

        private List<TrainingProgram> GetAllTrainingPrograms()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT id, [Name] from TrainingProgram;";
                    SqlDataReader reader = cmd.ExecuteReader();

                    List<TrainingProgram> allPrograms = new List<TrainingProgram>();

                    while (reader.Read())
                    {
                        allPrograms.Add(new TrainingProgram
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name"))
                        });
                    }
                    reader.Close();
                    return allPrograms;
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
                    cmd.CommandText = @"SELECT c.id as cId, Make, e.Id, ce.id as ceid, ce.UnassignDate
                                        from Computer c
                                        left join ComputerEmployee ce on ce.ComputerId = c.Id
                                        left join Employee e on ce.EmployeeId = e.id
                                        where e.id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();

                    Computer idcomputer = null;

                    while (reader.Read())
                    {
                        idcomputer = new Computer
                        {
                            id = reader.GetInt32(reader.GetOrdinal("cId")),
                            Make = reader.GetString(reader.GetOrdinal("Make")),
                            ComputerEmployee = new ComputerEmployee()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("ceid")),
                            }
                        };
                        if (!reader.IsDBNull(reader.GetOrdinal("UnassignDate")))
                        {
                            idcomputer.ComputerEmployee.UnassignDate = reader.GetDateTime(reader.GetOrdinal("UnassignDate"));                           
                        }
                    }
                    reader.Close();
                    return idcomputer;
                }
            }
        }
    }
}

