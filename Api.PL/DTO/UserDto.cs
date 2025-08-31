namespace EducationalPlatform.DTO
{
    public class UserDto
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public string Token { get; set; }
        public string RefreshToken { get; set; } // إضافة الـ Refresh Token

    }
}



//public class AuthResponseDto
//{
//    public string Token { get; set; }
//    public string RefreshToken { get; set; } // إضافة الـ Refresh Token
//    public string Role { get; set; }
//    public DateTime ExpiresAt { get; set; }
//}