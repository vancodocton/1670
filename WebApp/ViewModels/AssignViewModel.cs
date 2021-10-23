using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WebApp.Models;

namespace WebApp.ViewModels
{
    public class AssignViewModel
    {
        [Required]
        public int CourseId { get; set; }

        [Required]
        public string Role { get; set; }

        [Required]
        public string UserId { get; set; }

        public ICollection<Trainee> Trainees { get; set; }

        public ICollection<Trainer> Trainers { get; set; }
    }
}