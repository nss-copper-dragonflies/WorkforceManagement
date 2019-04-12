using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using BangazonWorkForce.Models.ViewModel;
using BangazonWorkForce.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

//Author: Brittany Ramos-Janeway
//This is the controller for training programs and it facilitates obtaining the records to view all training programs, view the details of a single training program, edit training programs, create new training programs, and delete future training programs.

namespace BangazonWorkForce.Controllers
{
    public class TrainingProgramController : Controller
    {
        private readonly IConfiguration _config;

        public TrainingProgramController(IConfiguration config)
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

        // GET All Future Training Programs
        public ActionResult Index()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT Id, Name, StartDate, 
                                            EndDate, MaxAttendees
	                                        FROM TrainingProgram
	                                        WHERE StartDate >= GETDATE()";
                    SqlDataReader reader = cmd.ExecuteReader();

                    List<TrainingProgram> trainingPrograms = new List<TrainingProgram>();
                    while (reader.Read())
                    {
                        TrainingProgram trainingProgram = new TrainingProgram
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("name")),
                            StartDate = reader.GetDateTime(reader.GetOrdinal("StartDate")),
                            EndDate = reader.GetDateTime(reader.GetOrdinal("EndDate")),
                            MaxAttendees = reader.GetInt32(reader.GetOrdinal("MaxAttendees"))
                        };

                        trainingPrograms.Add(trainingProgram);
                    }

                    reader.Close();

                    return View(trainingPrograms);
                }
            }
        }

        // Get individual training program details with a list of employees attending the training program
        public ActionResult Details(int id)
        {

            TrainingProgram trainingProgram = GetTrainingProgramById(id);
            if(trainingProgram == null)
            {
                return NotFound();
            }

            TrainingProgramDetailsViewModel viewModel = new TrainingProgramDetailsViewModel
            {
                Employees = GetAllEmployees(id),
                TrainingProgram = trainingProgram
            };
            return View(viewModel);
                
        }

        // Directs user to the page in which they can create a new training program
        public ActionResult Create()
        {
            return View();
        }

        // Once the user has created a training program, it is added to the database
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TrainingProgram trainingProgram)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"INSERT INTO TrainingProgram (Name, StartDate, 
                                            EndDate, MaxAttendees)
                                            OUTPUT INSERTED.Id
                                            VALUES (@Name, @StartDate, 
                                            @EndDate, @MaxAttendees)";
                        cmd.Parameters.Add(new SqlParameter("@Name", trainingProgram.Name));
                        cmd.Parameters.Add(new SqlParameter("@StartDate", trainingProgram.StartDate));
                        cmd.Parameters.Add(new SqlParameter("@EndDate", trainingProgram.EndDate));
                        cmd.Parameters.Add(new SqlParameter("@MaxAttendees", trainingProgram.MaxAttendees));

                        int newId = (int)cmd.ExecuteScalar();
                        trainingProgram.Id = newId;

                        return RedirectToAction(nameof(Index));
                    }
                }
            }
            catch
            {
                return View();
            }
        }

        // The user is directed to a page where they are able to edit the training program they have selected, and the fields are pre-filled with the program's current information
        public ActionResult Edit(int id)
        {
            TrainingProgram trainingProgram = GetTrainingProgramById(id);
            if(trainingProgram == null)
            {
                return NotFound();
            }

            return View(trainingProgram);
        }

        // Once the user has made the desired edits they are then added to the database
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, TrainingProgram trainingProgram)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"UPDATE TrainingProgram
                                            SET Name = @Name,
                                                StartDate = @StartDate,
                                                EndDate = @EndDate,
                                                MaxAttendees = @MaxAttendees
                                            WHERE Id = @id";
                        cmd.Parameters.Add(new SqlParameter("@id", id));
                        cmd.Parameters.Add(new SqlParameter("@Name", trainingProgram.Name));
                        cmd.Parameters.Add(new SqlParameter("@StartDate", trainingProgram.StartDate));
                        cmd.Parameters.Add(new SqlParameter("@EndDate", trainingProgram.EndDate));
                        cmd.Parameters.Add(new SqlParameter("@MaxAttendees", trainingProgram.MaxAttendees));

                        cmd.ExecuteNonQuery();

                        return RedirectToAction(nameof(Index));
                    }
                }
            }
            catch
            {
                return View(trainingProgram);
            }
        }

        // If the training program exists this passes the program to be deleted to the delete view so that the user can ensure they are choosing to delete the correct record
        public ActionResult Delete(int id)
        {
            TrainingProgram trainingProgram = GetTrainingProgramById(id);
            if (trainingProgram == null)
            {
                return NotFound();
            }
            return View(trainingProgram);
        }

        // Once the user decides which record to delete this method deletes the record from the database
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, TrainingProgram trainingProgram)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"DELETE FROM TrainingProgram 
                                            WHERE Id = @id";
                        cmd.Parameters.Add(new SqlParameter("@id", trainingProgram.Id));

                        cmd.ExecuteNonQuery();

                        return RedirectToAction(nameof(Index));
                    }
                }
            }
            catch
            {
                return View();
            }
        }

        // This method is used to get an individual training program by its Id. It is used for the details and edit methods.
        private TrainingProgram GetTrainingProgramById(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT t.id AS TrainingProgramId, 
                                                t.[Name], t.StartDate, 
                                                t.EndDate, t.MaxAttendees
                                            FROM TrainingProgram t
                                            WHERE Id = @Id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();

                    TrainingProgram trainingProgram = null;

                    if (reader.Read())
                    {
                        trainingProgram = new TrainingProgram
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("TrainingProgramId")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            StartDate = reader.GetDateTime(reader.GetOrdinal("StartDate")),
                            EndDate = reader.GetDateTime(reader.GetOrdinal("EndDate")),
                            MaxAttendees = reader.GetInt32(reader.GetOrdinal("MaxAttendees"))
                        };
                    }

                    reader.Close();

                    return trainingProgram;
                }
            }
        }

        // This method is used to get all the employees for a training program in order to display them in the program's details
        private List<Employee> GetAllEmployees(int TrainingProgramId)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT e.id AS EmployeeId, e.FirstName, e.LastName,
                                            e.IsSupervisor, e.DepartmentId
                                            FROM TrainingProgram t
                                            INNER JOIN EmployeeTraining et ON t.Id = et.TrainingProgramId
                                            INNER JOIN Employee e ON et.EmployeeId = e.Id
                                            WHERE t.Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", TrainingProgramId));
                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Employee> employees = new List<Employee>();

                    while (reader.Read())
                    {
                        employees.Add(new Employee
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("EmployeeId")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            IsSupervisor = reader.GetBoolean(reader.GetOrdinal("IsSupervisor")),
                            DepartmentId = reader.GetInt32(reader.GetOrdinal("DepartmentId"))
                        });
                    }
                    reader.Close();

                    return employees;
                }
            }
        }
    }
}
