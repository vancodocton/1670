using System.Collections.Generic;
using WebApp.Models;

namespace WebApp.ViewModels
{
    public class CourseViewModel
    {
        public Course Course { get; set; }

        public ICollection<Trainee> AssignedTrainees { get; set; }

        public ICollection<Trainer> AssignedTrainers { get; set; }
    }
}