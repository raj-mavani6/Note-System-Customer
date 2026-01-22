using System.ComponentModel.DataAnnotations;

namespace NotesSystemAdmin.Models
{
    public class Admin
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        public string Password { get; set; } = string.Empty;

        [StringLength(500)]
        public string? ProfileImageUrl { get; set; }

        [StringLength(50)]
        public string Role { get; set; } = "Admin";

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public bool IsActive { get; set; } = true;
    }
}
