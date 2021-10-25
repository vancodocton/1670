using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;
using WebApp.Models;

namespace WebApp.ViewModels
{
    [NotMapped]
    public class CreateCourseViewModel
    {
        public Course Course { get; set; }
        public SelectList Categories { get; set; }
    }
}