using System.Collections.Generic;
using WebApp.Models;

namespace WebApp.ViewModels
{
    public class AssignViewModel
    {
        public Course Course { get; set; }

        public ICollection<Trainee> Trainees { get; set; }

        public ICollection<Trainer> Trainers { get; set; }

        public string TraineeId { get; set; }

        public string TrainerId { get; set; }

        public ICollection<Trainee> AssignedTrainees { get; set; }

        public ICollection<Trainer> AssignedTrainers { get; set; }
    }
}