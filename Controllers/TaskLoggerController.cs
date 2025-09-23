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
        // Hardcoded connection string (replace "test" with your DB name if needed)
        private readonly string connectionString =
            "Data Source=.;Initial Catalog=test;Integrated Security=True;TrustServerCertificate=True";

        // ---------------- READ ALL ----------------
        public ActionResult Index()
        {
            var tasks = new List<TaskLoggerModel>();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM TaskLoggerDb ORDER BY DateDone DESC";
                SqlCommand cmd = new SqlCommand(query, con);

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

            return View(tasks);
        }

        // ---------------- CREATE ----------------
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(TaskLoggerModel model)
        {
            if (ModelState.IsValid)
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand("sp_InsertTask", con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@Id", Guid.NewGuid());
                    cmd.Parameters.AddWithValue("@Description", model.Description);
                    cmd.Parameters.AddWithValue("@DateDone", model.DateDone);
                    cmd.Parameters.AddWithValue("@TotalHours", model.TotalHours);

                    con.Open();
                    cmd.ExecuteNonQuery();
                }

                return RedirectToAction("Index");
            }

            return View(model);
        }

        // ---------------- UPDATE ----------------
        public ActionResult Edit(Guid id)
        {
            TaskLoggerModel task = null;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM TaskLoggerDb WHERE Id=@Id";
                SqlCommand cmd = new SqlCommand(query, con);
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

            if (task == null) return HttpNotFound();
            return View(task);
        }

        [HttpPost]
        public ActionResult Edit(TaskLoggerModel model)
        {
            if (ModelState.IsValid)
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand("sp_UpdateTask", con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@Id", model.Id);
                    cmd.Parameters.AddWithValue("@Description", model.Description);
                    cmd.Parameters.AddWithValue("@DateDone", model.DateDone);
                    cmd.Parameters.AddWithValue("@TotalHours", model.TotalHours);

                    con.Open();
                    cmd.ExecuteNonQuery();
                }

                return RedirectToAction("Index");
            }

            return View(model);
        }

        // ---------------- DELETE ----------------
        public ActionResult Delete(Guid id)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("sp_DeleteTask", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Id", id);

                con.Open();
                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Index");
        }

        // ---------------- SEARCH ----------------
        public ActionResult Search(string date)
        {
            var tasks = new List<TaskLoggerModel>();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("sp_SearchTasksByDate", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@DateDone", date);

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

            return View("Index", tasks);
        }
    }
}
