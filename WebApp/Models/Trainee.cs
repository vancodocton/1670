using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApp.Models.Profiles;

namespace WebApp.Models
{
    public class Trainee : ApplicationUser, IProfileTrainee
    {
        [DataType(DataType.Date)]
        public DateTime? BirthDate { get; set; }

        [StringLength(50)]
        public string Education { get; set; }

        public virtual ICollection<Course> Courses { get; set; }
    }
}