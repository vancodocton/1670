using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WebApp.Models
{
    public class Trainee
    {
        [Key]
        [ForeignKey("User")]
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        [DataType(DataType.Date)]
        public DateTime? BirthDate { get; set; }

        [StringLength(255)]
        public string Education { get; set; }

        public ICollection<CourseTrainee> CourseTrainees { get; set; }
    }
}