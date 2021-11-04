using System.Collections.Generic;
using WebApp.Models;
using WebApp.Utils;
using WebApp.ViewModels;

namespace WebApp.Areas.Staff.Data
{
    public class AssignedSearchVewModel : AssignedSearchForm
    {
        //public AssignedSearchForm SearchForm { get; set; }

        //public AssignedSearchVewModel(int CourseId)
        //{
        //    this.CourseId = CourseId;
        //    SearchForm = new AssignedSearchForm()
        //    {
        //        CourseId = CourseId,
        //        Roles = new List<string>() { Role.Trainer, Role.Trainee, }
        //    };
        //}

        public List<GroupedUsersViewModel<ApplicationUser>> GroupedUsers { get; set; }
    }
}