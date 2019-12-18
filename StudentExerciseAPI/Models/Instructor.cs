using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StudentExerciseAPI.Models
{
    public class Instructor
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Instructor must have a first name associated")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Instructor must have a last name associated")]
        public string LastName { get; set; }

        public string Specialty { get; set; }
        [Required(ErrorMessage = "Instructor must have a slackhandle associated")]
        [StringLength(12, MinimumLength = 3, ErrorMessage = "Slack handle must be between 3 and 12 characters")]
        public string SlackHandle { get; set; }

        public int CohortId { get; set; }
        public Cohort Cohort { get; set; }
    }
}
