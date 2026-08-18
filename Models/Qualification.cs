using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReceptionSystem.Models
{
    public class Qualification
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "الدرجة العلمية")]
        public string? Degree { get; set; }

        [Display(Name = "الاختصاص")]
        public string? Specialization { get; set; }

        [Display(Name = "الجامعة / المعهد")]
        public string? University { get; set; }

        [Display(Name = "سنة التخرج")]
        public string? GraduationYear { get; set; }

        // Foreign Key
        public int JobApplicationId { get; set; }

        [ForeignKey("JobApplicationId")]
        public JobApplication? JobApplication { get; set; }
    }
}