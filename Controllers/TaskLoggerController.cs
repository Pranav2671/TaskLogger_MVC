using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web.Mvc;
using TaskLoggerDotNet.Models;

namespace TaskLoggerDotNet.Controllers
{
    public class TaskLoggerController : Controller
    {
        private string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["TaskDbConnection"].ConnectionString;

        // GET: TaskLogger
        public ActionResult Index()
        {
            List<TaskLoggerModel> tasks = new List<TaskLoggerModel>();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_GetAllTasks", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        tasks.Add(new TaskLoggerModel
                        {
                            Id = (Guid)reader["Id"],
                            Description = reader["Description"].ToString(),
                            DateDone = reader["DateDone"].ToString(),
                            TotalHours = reader["TotalHours"].ToString()
                        });
                    }
                }
            }

            return View(tasks);
        }

        // GET: TaskLogger/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: TaskLogger/Create
        [HttpPost]
        public ActionResult Create(TaskLoggerModel model)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_InsertTask", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", Guid.NewGuid());
                    cmd.Parameters.AddWithValue("@Description", model.Description);
                    cmd.Parameters.AddWithValue("@DateDone", model.DateDone);
                    cmd.Parameters.AddWithValue("@TotalHours", model.TotalHours);

                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            return RedirectToAction("Index");
        }

        // GET: TaskLogger/Edit/{id}
        public ActionResult Edit(Guid id)
        {
            TaskLoggerModel task = null;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_GetTaskById", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", id);
                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        task = new TaskLoggerModel
                        {
                            Id = (Guid)reader["Id"],
                            Description = reader["Description"].ToString(),
                            DateDone = reader["DateDone"].ToString(),
                            TotalHours = reader["TotalHours"].ToString()
                        };
                    }
                }
            }

            if (task == null)
                return HttpNotFound();

            return View(task);
        }

        // POST: TaskLogger/Edit
        [HttpPost]
        public ActionResult Edit(TaskLoggerModel model)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_UpdateTask", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", model.Id);
                    cmd.Parameters.AddWithValue("@Description", model.Description);
                    cmd.Parameters.AddWithValue("@DateDone", model.DateDone);
                    cmd.Parameters.AddWithValue("@TotalHours", model.TotalHours);

                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            return RedirectToAction("Index");
        }

        // GET: TaskLogger/Delete/{id}
        public ActionResult Delete(Guid id)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_DeleteTask", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", id);

                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            return RedirectToAction("Index");
        }

        // GET: TaskLogger/Search
        public ActionResult Search(string date)
        {
            List<TaskLoggerModel> filteredTasks = new List<TaskLoggerModel>();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_SearchTasksByDate", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@DateDone", date);

                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        filteredTasks.Add(new TaskLoggerModel
                        {
                            Id = (Guid)reader["Id"],
                            Description = reader["Description"].ToString(),
                            DateDone = reader["DateDone"].ToString(),
                            TotalHours = reader["TotalHours"].ToString()
                        });
                    }
                }
            }

            return View("Index", filteredTasks);
        }
    }
}
