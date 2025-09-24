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

        public ActionResult Index()
        {
            var tasks = GetAllTasksFromDb();
            return View(tasks);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TaskLoggerModel model)
        {
            InsertTask(model);
            return PartialView("_TaskList", GetAllTasksFromDb());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(TaskLoggerModel model)
        {
            UpdateTask(model);
            return PartialView("_TaskList", GetAllTasksFromDb());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(Guid id)
        {
            DeleteTask(id);
            return PartialView("_TaskList", GetAllTasksFromDb());
        }

        public ActionResult Search(string date)
        {
            if (!DateTime.TryParse(date, out var dt))
                return PartialView("_TaskList", GetAllTasksFromDb());

            return PartialView("_TaskList", SearchTasks(dt));
        }

        // ==================== Helper Methods ====================
        private List<TaskLoggerModel> GetAllTasksFromDb()
        {
            var list = new List<TaskLoggerModel>();
            using (var con = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand("sp_GetAllTasks", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new TaskLoggerModel
                        {
                            Id = (Guid)reader["Id"],
                            Description = reader["Description"].ToString(),
                            DateDone = Convert.ToDateTime(reader["DateDone"]).ToString("yyyy-MM-dd"),
                            TotalHours = reader["TotalHours"].ToString()
                        });
                    }
                }
            }
            return list;
        }

        private void InsertTask(TaskLoggerModel model)
        {
            using (var con = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand("sp_InsertTask", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Id", Guid.NewGuid());
                cmd.Parameters.AddWithValue("@Description", model.Description ?? string.Empty);
                cmd.Parameters.AddWithValue("@DateDone", string.IsNullOrEmpty(model.DateDone) ? DBNull.Value : (object)DateTime.Parse(model.DateDone));
                cmd.Parameters.AddWithValue("@TotalHours", string.IsNullOrEmpty(model.TotalHours) ? DBNull.Value : (object)int.Parse(model.TotalHours));
                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        private void UpdateTask(TaskLoggerModel model)
        {
            using (var con = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand("sp_UpdateTask", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Id", model.Id);
                cmd.Parameters.AddWithValue("@Description", model.Description ?? string.Empty);
                cmd.Parameters.AddWithValue("@DateDone", string.IsNullOrEmpty(model.DateDone) ? DBNull.Value : (object)DateTime.Parse(model.DateDone));
                cmd.Parameters.AddWithValue("@TotalHours", string.IsNullOrEmpty(model.TotalHours) ? DBNull.Value : (object)int.Parse(model.TotalHours));
                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        private void DeleteTask(Guid id)
        {
            using (var con = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand("sp_DeleteTask", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Id", id);
                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        private List<TaskLoggerModel> SearchTasks(DateTime date)
        {
            var list = new List<TaskLoggerModel>();
            using (var con = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand("sp_SearchTasksByDate", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@DateDone", date);
                con.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new TaskLoggerModel
                        {
                            Id = (Guid)reader["Id"],
                            Description = reader["Description"].ToString(),
                            DateDone = Convert.ToDateTime(reader["DateDone"]).ToString("yyyy-MM-dd"),
                            TotalHours = reader["TotalHours"].ToString()
                        });
                    }
                }
            }
            return list;
        }
    }
}
