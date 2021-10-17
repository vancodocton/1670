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

        public ICollection<TraineeProfile> AssignedTrainees { get; set; }

        public ICollection<TraineeProfile> AssignedTrainers { get; set; }

        public ICollection<TraineeProfile> UnassignedTrainees { get; set; }

        public ICollection<TraineeProfile> UnassignedTrainers { get; set; }
    }
}