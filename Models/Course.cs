using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReceptionSystem.Models
{
    public class Course
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "اسم الدورة")]
        public string? CourseName { get; set; }

        [Display(Name = "الجهة المانحة")]
        public string? Organization { get; set; }

        [Display(Name = "مدة الدورة")]
        public string? Duration { get; set; }

        // Foreign Key
        public int JobApplicationId { get; set; }

        [ForeignKey("JobApplicationId")]
        public JobApplication? JobApplication { get; set; }
    }
}