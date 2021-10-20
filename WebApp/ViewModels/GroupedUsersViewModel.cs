using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApp.Models;

namespace WebApp.ViewModels
{
    public class GroupedUsersViewModel
    {
        public string Type { get; set; }
        public List<ApplicationUser> Users { get; set; }
    }
}