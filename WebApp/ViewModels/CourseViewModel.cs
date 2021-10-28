using System.Collections.Generic;
using WebApp.Models;

namespace WebApp.ViewModels
{
    public class CourseViewModel
    {
        public Course Course { get; set; }

        public List<GroupedUsersViewModel<ApplicationUser>> UserGroups { get; set; }

        public CourseViewModel()
        {
            UserGroups = new List<GroupedUsersViewModel<ApplicationUser>>();
        }
    }
}