using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudentExerciseAPI.Models
{
    public class Cohort
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public List<Student> Student { get; set; } = new List<Student>();
        public List<Instructor> instructors { get; set; } = new List<Instructor>();
    }
}
