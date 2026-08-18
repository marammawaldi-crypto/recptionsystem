using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReceptionSystem.Models
{
    public class ComputerSkill
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "مهارة الحاسوب")]
        public string SkillName { get; set; } = string.Empty;

        public int JobApplicationId { get; set; }

        [ForeignKey("JobApplicationId")]
        public JobApplication? JobApplication { get; set; }
    }
}