using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebApp.Utils
{
    public class AssignedSearchForm : SearchForm
    {                  
        public string UserRole { get; set; }

        public List<string> Roles { get; set; }
    }
}