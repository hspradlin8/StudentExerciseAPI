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
    public class StudentController : ControllerBase
    {
        private readonly IConfiguration _config;

        public StudentController(IConfiguration config)
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
        public async Task<IActionResult> Get([FromQuery]int Id, string include)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    
                    List<Student> students = new List<Student>();
                    // if they are looking for exercises
                    if (include !=null)
                    
                    {
                        cmd.CommandText = @"SELECT s.Id, s.FirstName, s.LastName, s.SlackHandle, s.CohortId, e.Id AS ExerciseId, e.Name AS ExerciseName, e.Language, c.[Name] AS CohortName, c.Id AS CoId
                                        FROM Student s
                                        LEFT JOIN Cohort c ON s.CohortId = c.Id
                                        INNER JOIN  StudentExercise se ON se.StudentId = s.Id
                                        INNER JOIN Exercise e ON e.Id = se.ExerciseId";

                    }else
                    {
                        cmd.CommandText = @"SELECT s.Id, s.FirstName, s.LastName, s.SlackHandle, s.CohortId, c.Id AS CoId, c.Name AS CohortName 
                                        FROM Student s
                                        LEFT JOIN  Cohort c ON s.CohortId = c.Id";
                                       
                    }
                    SqlDataReader reader = cmd.ExecuteReader();

                    //this is where we are storing all of the exercises from the sql statement
                   

                    while (reader.Read())
                    {
                    List<Exercise> exercises = new List<Exercise>();
                        if (include != null)
                        {
                            Exercise exercise = new Exercise
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Name = reader.GetString(reader.GetOrdinal("ExerciseName")),
                                Language = reader.GetString(reader.GetOrdinal("Language"))

                            };
                            exercises.Add(exercise);
                        }
                        Student student = new Student
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            SlackHandle = reader.GetString(reader.GetOrdinal("SlackHandle")),
                            CohortId = reader.GetInt32(reader.GetOrdinal("CohortId")),

                            Cohort = new Cohort()
                            {

                                Id = reader.GetInt32(reader.GetOrdinal("CoId")),
                                Name = reader.GetString(reader.GetOrdinal("CohortName")),
                            },

                            //this is considered all of the student exercises they are currently working on
                            Exercise = exercises
                        };

                        students.Add(student);
                    }
                    reader.Close();

                    return Ok(students);
                }
            }
        }

    }
}
