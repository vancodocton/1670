using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApp.Models;

namespace WebApp.ViewModels
{
    public class GroupedUsersViewModel
    {
        public List<ApplicationUser> Staffs { get; set; }

        public List<ApplicationUser> Trainers { get; set; }

        public List<ApplicationUser> Trainees { get; set; }

    }
}