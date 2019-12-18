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
    public class ExerciseController : ControllerBase
    {
        private readonly IConfiguration _config;

        public ExerciseController(IConfiguration config)
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
        public async Task<IActionResult> Get([FromQuery] string Language, string Name)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT Id, Name, Language 
                                        FROM Exercise
                                        WHERE 1=1";

                    if (Language != null)
                    {
                        cmd.CommandText += " AND [Language] LIKE @Language";
                        cmd.Parameters.Add(new SqlParameter("@Language", "%" + Language + "%"));
                    }

                    if (Name != null)
                    {
                        cmd.CommandText += " AND [Name] LIKE @Name";
                        cmd.Parameters.Add(new SqlParameter("@Name", "%" + Name + "%"));
                    }

                    
                    SqlDataReader reader = cmd.ExecuteReader();
                    List<Exercise> exercise = new List<Exercise>();

                    while (reader.Read())
                    {
                        Exercise exercises = new Exercise
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            Language = reader.GetString(reader.GetOrdinal("Language")),


                        };

                        exercise.Add(exercises);
                    }
                    reader.Close();

                    return Ok(exercise);
                }
            }
        }

        [HttpGet("{id}", Name = "GetExercises")]
        public async Task<IActionResult> Get([FromRoute]int Id, string include)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {

                    List<Exercise> exercises = new List<Exercise>();
                    // if they are looking for students 
                    if (include != null)

                    {
                        cmd.CommandText = @"SELECT s.FirstName, s.LastName, s.Id, se.Id, se.StudentId, se.ExerciseId, e.[Name], e.Id, e.[Language]
                                            FROM StudentExercise se
                                            LEFT JOIN Student s ON s.Id = se.StudentId
                                            LEFT JOIN Exercise e ON e.Id = se.ExerciseId
                                            WHERE 1=1";
    }
                    else
                    {
                        //does not include students 
                        cmd.CommandText = @"SELECT  se.Id, se.StudentId, se.ExerciseId, e.[Name], e.Id, e.[Language]
                                            FROM StudentExercise se
                                            LEFT JOIN Exercise e ON e.Id = se.ExerciseId";
                                            
                    }
                    SqlDataReader reader = cmd.ExecuteReader();

                    //List<Student> this is where we are storing all of the exercises from the sql statement

                        List<Student> students = new List<Student>();

                    while (reader.Read())
                    {
                        if (include != null) 
                        {
                            Student student = new Student
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                SlackHandle = reader.GetString(reader.GetOrdinal("SlackHandle")),
                                CohortId = reader.GetInt32(reader.GetOrdinal("CohortId")),
                            };
                            students.Add(student);
                        }
                        Exercise exercise = new Exercise
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("ExerciseName")),
                            Language = reader.GetString(reader.GetOrdinal("Language")),


                            //this is considered all of the student exercises they are currently working on
                            Student = students
                        };

                        exercises.Add(exercise);
                    }
                    reader.Close();

                    return Ok(students);
                }
            }
        }
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Exercise exercise)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO Exercise (Name, Language)
                                        OUTPUT INSERTED.Id
                                        VALUES (@Name)";
                    cmd.Parameters.Add(new SqlParameter("@Name", exercise.Name));
                    cmd.Parameters.Add(new SqlParameter("@Language", exercise.Language));

                    int newId = (int)cmd.ExecuteScalar();
                    exercise.Id = newId;
                    return CreatedAtRoute("GetExercise", new { id = newId }, exercise);
                }
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put([FromRoute] int id, [FromBody] Exercise exercise)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"UPDATE Exercise
                                            SET Name = @Name, SET Language = @Language
                                            WHERE Id = @id";
                        cmd.Parameters.Add(new SqlParameter("@Name", exercise.Name));
                        cmd.Parameters.Add(new SqlParameter("@Language", exercise.Language));
                        cmd.Parameters.Add(new SqlParameter("@id", id));

                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            return new StatusCodeResult(StatusCodes.Status204NoContent);
                        }
                        throw new Exception("No rows affected");
                    }
                }
            }
            catch (Exception)
            {
                if (!ExerciseExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }



        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"DELETE FROM Exercise WHERE Id = @id";
                        cmd.Parameters.Add(new SqlParameter("@id", id));

                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            return new StatusCodeResult(StatusCodes.Status204NoContent);
                        }
                        throw new Exception("No rows affected");
                    }
                }
            }
            catch (Exception)
            {
                if (!ExerciseExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        private bool ExerciseExists(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT Id, Name, Language
                        FROM Exercise
                        WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    SqlDataReader reader = cmd.ExecuteReader();
                    return reader.Read();
                }
            }
        }
    }
}



