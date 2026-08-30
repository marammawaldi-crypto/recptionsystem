using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReceptionSystem.Models
{
    public class JobApplication
    {
        internal readonly object Graduationyear;

        [Key]
        public int Id { get; set; }
        [Required]
        [Display(Name = "تاريخ تقديم الطلب")]
        [DataType(DataType.Date)]
        public DateTime ApplicationSubmissionDate { get; set; }
        [Display(Name = "رقم الطلب")]
        public string ApplicationNumber { get; set; } = string.Empty;

        [Display(Name = "تاريخ تقديم الطلب")]
        public DateTime ApplicationDate { get; set; }

        // Personal Information

        [Required]
        [Display(Name = "الاسم الكامل")]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [Display(Name = "الجنس")]
        public string Gender { get; set; } = string.Empty;

        [Required]
        [Display(Name = "الجنسية")]
        public string Nationality { get; set; } = string.Empty;

        [Display(Name = "مكان الولادة")]
        public string? PlaceOfBirth { get; set; }

        [Required]
        [Display(Name = "تاريخ الميلاد")]
        public DateTime? DateOfBirth { get; set; }

        [Display(Name = "الرقم الوطني / رقم الهوية")]
        public string? NationalNumber { get; set; }

        [Display(Name = "الحالة الاجتماعية")]
        public string? MaritalStatus { get; set; }

        [Display(Name = "العنوان الحالي")]
        public string? Address { get; set; }

        [Display(Name = "رقم الهاتف")]
        public string? Phone { get; set; }

        // Driving license relation: foreign key to DrivingLicenseType
        public int? DrivingLicenseTypeId { get; set; }

        [ForeignKey("DrivingLicenseTypeId")]
        public DrivingLicenseType? DrivingLicenseType { get; set; }

        [EmailAddress]
        [Display(Name = "البريد الإلكتروني")]
        public string? Email { get; set; }

        [Display(Name = "إجازة القيادة")]
        public string? DrivingLicense { get; set; }

        // Job Information

        [Display(Name = "الوظيفة المتقدم لها")]
        public string? Position { get; set; }

        [Display(Name = "الراتب المتوقع")]
        public string? ExpectedSalary { get; set; }

        [Display(Name = "متى يمكنك مباشرة العمل؟")]
        public DateTime? AvailableStartDate { get; set; }

        [Display(Name = "هل تعمل حالياً؟")]
        public bool CurrentlyWorking { get; set; }

        [Display(Name = "هل لديك أي حالة صحية قد تؤثر على أداء العمل؟")]
        public string? HealthCondition { get; set; }

        [Display(Name = "هل سبق أن تم الحكم عليك؟")]
        public bool CriminalRecord { get; set; }

        [Display(Name = "ملاحظات")]
        public string? Notes { get; set; }


        // CV

        [Display(Name = "السيرة الذاتية")]
        public byte[]? CvFileData { get; set; }

        public string? CvFileName { get; set; }

        public string? CvContentType { get; set; }

        // Optional file path when storing CVs on disk or a CDN
        public string? CvFilePath { get; set; }

        public List<Language> Languages { get; set; } = new();

        public List<Course> Courses { get; set; } = new();

        public List<ComputerSkill> ComputerSkills { get; set; } = new();

        public List<Experience> Experiences { get; set; } = new();

        public List<Qualification> Qualifications { get; set; } = new();
    }
}
