using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StudentExerciseAPI.Models
{
    public class Student
    {
        public int Id { get; set; }

        public string  FirstName { get; set; }
        public string LastName { get; set; }
        [StringLength(12, MinimumLength = 3, ErrorMessage = "Slack handle must be between 3 and 12 characters")]
        public string SlackHandle { get; set; }

        public int CohortId { get; set; }
        public Cohort Cohort { get; set; }
        public List<Exercise> Exercise { get; set; } = new List<Exercise>();
    }
}
