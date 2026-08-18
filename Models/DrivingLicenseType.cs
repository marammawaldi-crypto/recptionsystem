using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace ReceptionSystem.Models
{
    public class DrivingLicenseType
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "فئة إجازة القيادة")]
        public string Category { get; set; } = string.Empty;

    }
}