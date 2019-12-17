//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.Extensions.Configuration;
//using System.Data;
//using Microsoft.Data.SqlClient;
//using StudentExerciseAPI.Models;
//using Microsoft.AspNetCore.Http;
//using System.Collections.Generic;

//namespace StudentExerciseAPI.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class CohortController : ControllerBase
//    {
//        private readonly IConfiguration _config;

//        public CohortController(IConfiguration config)
//        {
//            _config = config;
//        }

//        public SqlConnection Connection
//        {
//            get
//            {
//                return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
//            }
//        }

//        [HttpGet]
//        public async Task<IActionResult> Get()
//        {
//            using (SqlConnection conn = Connection)
//            {
//                conn.Open();
//                using (SqlCommand cmd = conn.CreateCommand())
//                {
//                    cmd.CommandText = @"SELECT c.Id, c.Name,
//                                        FROM Cohort c
//                                        INNER JOIN Cohort c ON c.Id = CohortId";
//                    SqlDataReader reader = cmd.ExecuteReader();
//                    List<Cohort> cohorts = new List<Cohort>();

//                    while (reader.Read())
//                    {
//                        Cohort cohort = new Cohort
//                        {
//                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
//                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
//                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
//                            SlackHandle = reader.GetString(reader.GetOrdinal("SlackHandle")),
//                            Specialty = reader.GetString(reader.GetOrdinal("Specialty")),
//                            CohortId = reader.GetInt32(reader.GetOrdinal("CohortId")),

//                            Cohort = new CohortController()
//                            {

//                                Id = reader.GetInt32(reader.GetOrdinal("CoId")),
//                                Name = reader.GetString(reader.GetOrdinal("Name")),
//                            }
//                        };

//                        instructors.Add(instructor);
//                    }
//                    reader.Close();

//                    return Ok(instructors);
//                }
//            }
//        }
//    }
//}