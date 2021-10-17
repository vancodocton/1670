using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WebApp.Models
{
    public class CourseTrainee
    {
        [Key, Column(Order = 0)]
        [ForeignKey("Course")]
        public int CourseId { get; set; }
        public Course Course { get; set; }

        [Key, Column(Order = 1)]
        [ForeignKey("TraineeProfile")]
        public string TraineeUserId { get; set; }
        public TraineeProfile TraineeProfile { get; set; }
    }
}