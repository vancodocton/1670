using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApp.Models;
using WebApp.Models.Profiles;

namespace WebApp.ViewModels
{
    public class UserProfileViewModel
    {
        [Display(Name = "User Id")]
        public string Id { get; set; }

        public IList<string> Roles { get; set; }

        [Display(Name = "Full Name")]
        [StringLength(50)]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public virtual string Email { get; set; }

        [Range(0, int.MaxValue)]
        public int? Age { get; set; }

        [StringLength(50)]
        public string Address { get; set; }

        public UserProfileViewModel() { }

        public UserProfileViewModel(ApplicationUser user)
        {
            Id = user.Id;
            FullName = user.FullName;
            Email = user.Email;
            Age = user.Age;
            Address = user.Address;
        }

        [DataType(DataType.Date)]
        [Display(Name = "Date of birth")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? BirthDate { get; set; }

        [StringLength(50)]
        public string Education { get; set; }

        public UserProfileViewModel(Trainee trainee) : this(trainee.User)
        {
            BirthDate = trainee.BirthDate;
            Education = trainee.Education;
        }

        [StringLength(50)]
        public string Specialty { get; set; }

        public UserProfileViewModel(Trainer trainer) : this(trainer.User)
        {
            Specialty = trainer.Specialty;
        }

        public ApplicationUser User => new ApplicationUser()
        {
            Id = Id,
            FullName = FullName,
            Email = Email,
            Age = Age,
            Address = Address,
            UserName = Email
        };
    }
}