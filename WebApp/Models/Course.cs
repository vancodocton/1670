using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApp.Models
{
    public class Course
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string Name { get; set; }

        [ForeignKey("CourseCategory")]
        public int CourseCategoryId { get; set; }
        [Display(Name = "Course Category")]
        public CourseCategory CourseCategory { get; set; }

        [StringLength(255)]
        public string Description { get; set; }

        public virtual ICollection<Trainee> Trainees { get; set; }

        public virtual ICollection<Trainer> Trainers { get; set; }
    }
}