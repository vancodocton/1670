using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApp.Models;

namespace WebApp.ViewModels
{
    public class AssignViewModel
    {
        public Course Course { get; set; }

        public ICollection<Trainee> Trainees { get; set; }

        public ICollection<Trainer> Trainers { get; set; }

        public Trainee AssignedTrainee { get; set; }

        public Trainer AssignedTrainer { get; set; }
    }
}