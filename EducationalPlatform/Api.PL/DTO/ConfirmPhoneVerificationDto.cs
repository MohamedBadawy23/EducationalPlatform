namespace EducationalPlatform.DTO
{
    public class ConfirmPhoneVerificationDto
    {
        public string ChangeType { get; set; } // "password" or "phone"
        public string VerificationCode { get; set; }
        public string NewPassword { get; set; } // For password change
        public string NewPhoneNumber { get; set; } // For phone change
    }
} 