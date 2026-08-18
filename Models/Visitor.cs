using System.ComponentModel.DataAnnotations;

namespace ReceptionSystem.Models
{
    public class Visitor
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "اسم الزائر")]
        public string VisitorName { get; set; } = string.Empty;

        [Required]
        [Display(Name = "تلريخ الزيارة")]
        public DateTime VisitDate { get; set; }

        [Required]
        [Display(Name = "وقت الدخول")]
        public TimeSpan CheckIn { get; set; }

        [Display(Name = "وقت الخروج")]
        public TimeSpan? CheckOut { get; set; }

        [Required]
        [Display(Name = "الوجهة")]
        public string Destination { get; set; } = string.Empty;




        [Display(Name = "رقم الهاتف")]
        public string? Phone { get; set; }

        [Display(Name = "الغرض")]
        public string? Purpose { get; set; }

        [Display(Name = "ملاحظات")]
        public string? Notes { get; set; }
    }
}
