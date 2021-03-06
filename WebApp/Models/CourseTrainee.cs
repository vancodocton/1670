using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApp.Models
{
    public class CourseTrainee
    {
        [Key, Column(Order = 0)]
        [ForeignKey("Course")]
        public int CourseId { get; set; }
        public Course Course { get; set; }

        [Key, Column(Order = 1)]
        [ForeignKey("Trainee")]
        public string TraineeUserId { get; set; }
        public Trainee Trainee { get; set; }

        public ICollection<CourseTrainee> CourseTrainees { get; set; }
    }
}