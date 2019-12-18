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
using StudentExerciseAPI.Controllers;

namespace CoffeeShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CohortController : ControllerBase
    {
        private readonly IConfiguration _config;

        public CohortController(IConfiguration config)
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
        public async Task<IActionResult> Get([FromQuery]string Name)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT c.Id as CohortId, c.Name as CohortName, i.Id as InstructorId, 
                                        i.FirstName as InstructorFirstName, i.LastName as InstructorLastName, 
                                        i.Specialty, i.SlackHandle as InstructorSlack, 
                                        i.CohortId as InstructorCohortId, s.Id, s.FirstName as StudentFirstName, 
                                        s.LastName as StudentLastName, s.SlackHandle as StudentSlack, 
                                        s.CohortId as StudentCohortId
                                        FROM Cohort c
                                        LEFT JOIN Student s ON c.Id = s.Id
                                        LEFT JOIN Instructor i ON c.Id = i.CohortId
                                        WHERE 1=1";

                    if (Name != null)
                    {
                        cmd.CommandText += " AND [Name] LIKE @Name";
                        cmd.Parameters.Add(new SqlParameter("@Name", "%" + Name + "%"));
                    }
                    SqlDataReader reader = cmd.ExecuteReader();
                    List<Cohort> cohorts = new List<Cohort>();

                    while (reader.Read())
                    {
                        Cohort newCohort = null;
                        int cohortId = reader.GetInt32(reader.GetOrdinal("CohortId"));
                        if (!cohorts.Any(c => c.Id == cohortId))
                        {
                            newCohort = new Cohort()
                            {
                                Id = cohortId,
                                Name = reader.GetString(reader.GetOrdinal("CohortName"))
                            };

                            cohorts.Add(newCohort);
                        }

                        Cohort existingCohort = cohorts.Find(c => c.Id == cohortId);
                        if (!reader.IsDBNull(reader.GetOrdinal("Id")))
                        {
                            int studentId = reader.GetInt32(reader.GetOrdinal("Id"));
                            if (!existingCohort.Student.Any(s => s.Id == studentId))
                            {
                                Student newStudent = new Student()
                                {
                                    Id = studentId,
                                    FirstName = reader.GetString(reader.GetOrdinal("StudentFirstName")),
                                    LastName = reader.GetString(reader.GetOrdinal("StudentLastName")),
                                    SlackHandle = reader.GetString(reader.GetOrdinal("StudentSlack")),
                                    CohortId = reader.GetInt32(reader.GetOrdinal("StudentCohortId")),

                                };
                                existingCohort.Student.Add(newStudent);
                            };
                        }

                        if (!reader.IsDBNull(reader.GetOrdinal("InstructorId")))
                        {
                            int instructorId = reader.GetInt32(reader.GetOrdinal("InstructorId"));
                            if (!existingCohort.instructors.Any(i => i.Id == instructorId))
                            {
                                Instructor newInstructor = new Instructor()
                                {
                                    Id = instructorId,
                                    FirstName = reader.GetString(reader.GetOrdinal("InstructorFirstName")),
                                    LastName = reader.GetString(reader.GetOrdinal("InstructorLastName")),
                                    Specialty = reader.GetString(reader.GetOrdinal("Specialty")),
                                    SlackHandle = reader.GetString(reader.GetOrdinal("InstructorSlack")),
                                    CohortId = reader.GetInt32(reader.GetOrdinal("InstructorCohortId"))
                                };
                                existingCohort.instructors.Add(newInstructor);
                            };
                        }
                    }
                    reader.Close();

                    return Ok(cohorts);

                }

            }
        }

    }
}

      