using System.ComponentModel.DataAnnotations;

namespace ReceptionSystem.Models
{
    public class Permission
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Controller { get; set; } = string.Empty;

        [Required]
        public string Action { get; set; } = string.Empty;
    }
}