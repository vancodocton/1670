using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApp.Models;

namespace WebApp.ViewModels
{
    public class GroupedUsersViewModel<T>
    {
        public string Type { get; set; }
        public List<T> Users { get; set; }
    }
}