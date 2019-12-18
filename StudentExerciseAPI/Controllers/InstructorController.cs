using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Data;
using Microsoft.Data.SqlClient;
using StudentExerciseAPI.Models;
using Microsoft.AspNetCore.Http;

namespace StudentExerciseAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InstructorController : ControllerBase
    {
        private readonly IConfiguration _config;

        public InstructorController(IConfiguration config)
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

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery]string FirstName, string LastName, string SlackHandle)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT i.Id, i.FirstName, i.LastName, i.SlackHandle, 
                                        i.CohortId, i.Specialty, c.Name, c.Id AS CoId
                                        FROM Instructor i
                                        INNER JOIN Cohort c ON c.Id = CohortId";

                    if (FirstName != null)
                    {
                        cmd.CommandText += " AND FirstName LIKE @FirstName";
                        cmd.Parameters.Add(new SqlParameter("@FirstName", "%" + FirstName + "%"));
                    }

                    if (LastName != null)
                    {
                        cmd.CommandText += " AND LastName LIKE @LastName";
                        cmd.Parameters.Add(new SqlParameter("@LastName", "%" + LastName + "%"));
                    }

                    if (SlackHandle != null)
                    {
                        cmd.CommandText += " AND SlackHandle LIKE @SlackHandle";
                        cmd.Parameters.Add(new SqlParameter("@SlackHandle", "%" + SlackHandle + "%"));
                    }


                    SqlDataReader reader = cmd.ExecuteReader();
                    List<Instructor> instructors = new List<Instructor>();

                    while (reader.Read())
                    {
                        Instructor instructor = new Instructor
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            SlackHandle = reader.GetString(reader.GetOrdinal("SlackHandle")),
                            Specialty = reader.GetString(reader.GetOrdinal("Specialty")),
                            CohortId = reader.GetInt32(reader.GetOrdinal("CohortId")),

                            Cohort = new Cohort()
                            {

                                Id = reader.GetInt32(reader.GetOrdinal("CoId")),
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                            }
                        };

                        instructors.Add(instructor);
                    }
                    reader.Close();

                    return Ok(instructors);
                }
            }
        }
    }
}
