
using Core.Entities;

namespace Core.Repository
{
    public interface ITokenServices
    {
        Task<string> CreateToken(AppUser user/*, UserManager<AppUser> userManager*/);
        //Task<string> CreateToken(Student user, UserManager<Student> userManager);



        Task<(string AccessToken, string RefreshToken)> GenerateTokensAsync(AppUser user); // دالة جديدة
        Task<bool> ValidateRefreshTokenAsync(string refreshToken, string userId); // للتحقق من الـ Refresh Token
        Task<string> GenerateNewAccessTokenAsync(string refreshToken); // لتوليد Access Token جديد
    }
}
