using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StudentExerciseAPI.Models
{
    public class Cohort
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "A cohort must have a cohort name")]
        [StringLength(11, MinimumLength = 5, ErrorMessage = "Cohort name must be between 5 and 11 characters")]
        public string Name { get; set; }

        public List<Student> Student { get; set; } = new List<Student>();
        public List<Instructor> instructors { get; set; } = new List<Instructor>();
    }
}
