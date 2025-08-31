using System.ComponentModel.DataAnnotations;

namespace Core.Entities
{
    public class RefreshToken
    {
        [Key]
        public int Id { get; set; }
        public string Token { get; set; }
        public string UserId { get; set; }
        public DateTime ExpiryDate { get; set; }
        public bool IsRevoked { get; set; } = false;

        // Navigation property
        public AppUser User { get; set; }
    }
}
