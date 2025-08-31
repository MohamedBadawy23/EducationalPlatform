using Core.Entities;
using CoreProject.Enteties;
using System.ComponentModel.DataAnnotations;

namespace EducationalPlatform.DTO
{
    public class RegisterDTO
    {
        [Required]
        public string FullName { get; set; }
        [Required]
        [Phone]
        public string PhoneNumber { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; }
        public UserRole Role { get; set; } = UserRole.Student;
    }
}
