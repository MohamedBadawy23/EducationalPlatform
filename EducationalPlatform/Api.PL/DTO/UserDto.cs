using System.ComponentModel.DataAnnotations;

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
    }
}
