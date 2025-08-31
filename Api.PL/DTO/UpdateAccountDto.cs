namespace EducationalPlatform.DTO
{
    public class UpdateAccountDto
    {
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string FullName { get; set; }
        public string CurrentPassword { get; set; } // For password change
        public string NewPassword { get; set; }
    }
}