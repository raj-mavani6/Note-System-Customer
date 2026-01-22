using System.ComponentModel.DataAnnotations;

namespace NotesSystemAdmin.Models
{
    public class EditProfileViewModel
    {
        [Required(ErrorMessage = "Full name is required")]
        [StringLength(100, ErrorMessage = "Full name cannot exceed 100 characters")]
        [Display(Name = "Full Name")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [StringLength(500)]
        [Display(Name = "Profile Image URL")]
        public string? ProfileImageUrl { get; set; }
    }
}
