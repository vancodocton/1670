using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WebApp.Models;

namespace WebApp.ViewModels
{
    public class UserViewModel
    {
        public ApplicationUser User { get; set; }

        public List<string> Roles { get; set; }

        public string Specialty { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Date of birth")]
        public DateTime BirthDate { get; set; }

        public string Education { get; set; }
    }
}