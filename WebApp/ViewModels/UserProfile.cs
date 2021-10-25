using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApp.Models.Profiles;

namespace WebApp.ViewModels
{
    public class UserProfileViewModel
    {
        [Display(Name = "User Id")]
        public string Id { get; set; }

        [Display(Name = "Full Name")]
        [StringLength(50)]
        public string FullName { get; set; }

        [EmailAddress]
        [Display(Name = "Email")]
        public virtual string Email { get; set; }

        [Range(0, int.MaxValue)]
        public int? Age { get; set; }

        [StringLength(50)]
        public string Address { get; set; }

        //public UserProfile() { }

        //public UserProfile(ApplicationUser user)
        //{
        //    Id = user.Id;
        //    FullName = user.FullName;
        //    Email = user.Email;
        //    Age = user.Age;
        //    Address = user.Address;
        //}

        [DataType(DataType.Date)]
        public DateTime? BirthDate { get; set; }

        [StringLength(50)]
        public string Education { get; set; }

        //public UserProfile(Trainee user)
        //{
        //    Id = user.Id;
        //    FullName = user.FullName;
        //    Email = user.Email;
        //    Age = user.Age;
        //    Address = user.Address;
        //    BirthDate = user.BirthDate;
        //    Education = user.Education;
        //}

        [StringLength(50)]
        public string Specialty { get; set; }

        //public UserProfile(Trainer user)
        //{
        //    Id = user.Id;
        //    FullName = user.FullName;
        //    Email = user.Email;
        //    Age = user.Age;
        //    Address = user.Address;
        //    Specialty = user.Specialty;
        //}
    }
}