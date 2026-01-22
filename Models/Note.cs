using System.ComponentModel.DataAnnotations;

namespace NotesSystemAdmin.Models
{
    public class Note
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Content { get; set; } = string.Empty;

        [StringLength(500)]
        public string? ImageUrl { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? UpdatedAt { get; set; }

        public bool IsImportant { get; set; } = false;

        public int UserId { get; set; }

        // Navigation property
        public virtual User? User { get; set; }
    }
}
