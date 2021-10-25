using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApp.Models.Profiles;

namespace WebApp.Models
{
    public class UserProfile : IProfile
    {
        [Key]
        [ForeignKey("User")]
        public string Id { get; set; }
        public ApplicationUser User { get; set; }

        [Display(Name = "Full Name")]
        [StringLength(50)]
        public string FullName { get; set; }

        [StringLength(320)]
        public string Email { get; set; }

        [Range(0, int.MaxValue)]
        public int Age { get; set; }

        [StringLength(50)]
        public string Address { get; set; }
    }
}