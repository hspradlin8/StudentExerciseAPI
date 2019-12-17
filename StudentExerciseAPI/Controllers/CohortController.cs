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
        public async Task<IActionResult> Get()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT c.Id as CohortId, c.Name as CohortName, i.Id as InstructorId, 
                                        i.FirstName as InstructorFirstName, i.LastName as InstructoLastName, 
                                        i.Specialty as InstructorSpecialty, i.SlackHandle as InstructorSlack, 
                                        i.CohortId as InstructorCohortId, s.Id, s.FirstName as StudentFirstName, 
                                        s.LastName as StudentLastName, s.SlackHandle as StudentSlack, 
                                        s.CohortId as StudentCohortId
                                        FROM Cohort c
                                        LEFT JOIN Student s ON c.Id = s.Id
                                        LEFT JOIN Instructor i ON c.Id = i.CohortId";
                    SqlDataReader reader = cmd.ExecuteReader();
                    List<Cohort> cohorts = new List<Cohort>();

                    while (reader.Read())
                    {
                        int currentCohortID = reader.GetInt32(reader.GetOrdinal("CohortID"));
                        Cohort newCohort = cohorts.FirstOrDefault(i => i.Id == currentCohortID);
                        //If there's no cohort, create one and add it to the list.
                        if (newCohort == null)

                            newCohort = new Cohort
                            {
                                Id = currentCohortID,
                                Name = reader.GetString(reader.GetOrdinal("CohortName")),
                                Student = new List<Student>(),
                                instructors = new List<Instructor>()

                            };

                        cohorts.Add(newCohort);
                    }
                    reader.Close();

                    return Ok(cohorts);
                }
            }
        }

        //[HttpGet("{id}", Name = "GetEmployee")]
        //public async Task<IActionResult> Get([FromRoute] int id)
        //{
        //    using (SqlConnection conn = Connection)
        //    {
        //        conn.Open();
        //        using (SqlCommand cmd = conn.CreateCommand())
        //        {
        //            cmd.CommandText = @"
        //                SELECT
        //                    Id, FirstName, LastName, DepartmentId
        //                FROM Employee
        //                WHERE Id = @id";
        //            cmd.Parameters.Add(new SqlParameter("@id", id));
        //            SqlDataReader reader = cmd.ExecuteReader();

        //            Employee employee = null;

        //            if (reader.Read())
        //            {
        //                employee = new Employee
        //                {
        //                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
        //                    FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
        //                    LastName = reader.GetString(reader.GetOrdinal("LastName")),
        //                    DepartmentId = reader.GetInt32(reader.GetOrdinal("DepartmentId")),

        //                };
        //            }
        //            reader.Close();

        //            return Ok(employee);
        //        }
        //    }
        //}
    }
}