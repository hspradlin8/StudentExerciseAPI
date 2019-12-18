using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StudentExerciseAPI.Models
{
    public class Exercise
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Exercise must have a name associated")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Exercise must have a language associated")]
        public string Language { get; set; }
        public List<Student> Student { get; set; } = new List<Student>();
    }
}
