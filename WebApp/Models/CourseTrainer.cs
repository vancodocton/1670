using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApp.Models
{
    public class CourseTrainer
    {
        [Key, Column(Order = 0)]
        [ForeignKey("Course")]
        public int CourseId { get; set; }
        public Course Course { get; set; }

        [Key, Column(Order = 1)]
        [ForeignKey("Trainer")]
        public string TrainerUserId { get; set; }
        public Trainer Trainer { get; set; }

        public ICollection<CourseTrainer> CourseTrainers { get; set; }
    }
}