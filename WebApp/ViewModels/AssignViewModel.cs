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

        public ICollection<Trainee> AssignedTrainees { get; set; }

        public ICollection<ApplicationUser> AssignedTrainers { get; set; }

        public ICollection<ApplicationUser> UnassignedTrainees { get; set; }

        public ICollection<ApplicationUser> UnassignedTrainers { get; set; }
    }
}