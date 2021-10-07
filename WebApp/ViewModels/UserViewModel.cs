using System.Collections.Generic;
using WebApp.Models;

namespace WebApp.ViewModels
{
    public class UserViewModel
    {
        public ApplicationUser User { get; set; }

        public List<string> Roles { get; set; }

        public string Specialty { get; set; }
    }
}