using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReceptionSystem.Models
{
    public class Experience
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "اسم الشركة")]
        public string? CompanyName { get; set; }

        [Display(Name = "المسمى الوظيفي")]
        public string? JobTitle { get; set; }

        [Display(Name = "من تاريخ")]
        public DateTime? FromDate { get; set; }

        [Display(Name = "إلى تاريخ")]
        public DateTime? ToDate { get; set; }

        [Display(Name = "آخر راتب")]
        public string? LastSalary { get; set; }

        // Foreign Key
        public int JobApplicationId { get; set; }

        [ForeignKey("JobApplicationId")]
        public JobApplication? JobApplication { get; set; }
    }
}