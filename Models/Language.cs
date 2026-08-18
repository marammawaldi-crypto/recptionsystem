using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReceptionSystem.Models
{
    public class Language
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "اللغة")]
        public string LanguageName { get; set; } = string.Empty;

        [Display(Name = "المحادثة")]
        public string? Speaking { get; set; }

        [Display(Name = "الكتابة")]
        public string? Writing { get; set; }

        public int JobApplicationId { get; set; }

        [ForeignKey("JobApplicationId")]
        public JobApplication? JobApplication { get; set; }
    }
}