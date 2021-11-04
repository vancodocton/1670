using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using WebApp.Models;

namespace WebApp.ViewModels
{
    public class CourseAssignViewModel
    {
        [Required]
        public int CourseId { get; set; }
        public Course Course { get; set; }

        [Required]
        public string Role { get; set; }

        [Required]
        public string UserId { get; set; }

        public SelectList Users { get; set; }
    }
}