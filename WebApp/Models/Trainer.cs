using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApp.Models.Profiles;

namespace WebApp.Models
{
    public class Trainer: IProfileTrainer, IProfile
    {
        [Key]
        [ForeignKey("User")]
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        [StringLength(50)]
        public string Specialty { get; set; }

        public virtual ICollection<Course> Courses { get; set; }
    }
}