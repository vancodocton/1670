using System.Collections.Generic;

namespace WebApp.ViewModels
{
    public class GroupedUsersViewModel<T>
    {
        public string Type { get; set; }
        public List<T> Users { get; set; }
    }
}