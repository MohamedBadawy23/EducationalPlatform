 
using Core.Entities;
using Core.Repository;
using Core.Specefications;
using CoreProject.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using RepositoryProject;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Repository.Repository
{
    public class TokenServices : ITokenServices
    {
        private readonly IConfiguration _configuration;
        private readonly EducationalPlatformDbContext _context;
        private readonly IGenericRepository<Student> _studentRepository;

        public TokenServices(IConfiguration configuration, EducationalPlatformDbContext context,IGenericRepository<Student>studentRepository)
        {
            _configuration = configuration;
            _context = context;
            _studentRepository = studentRepository;
        }
        public async Task<string> CreateToken(AppUser user/*, UserManager<AppUser> userManager*/)
        {
            var student =await _studentRepository.GetByIdWithSpecAsync(new StudentByIdSpec(user.Id));
            //Private Claims
            var center = await _context.Centers.FirstOrDefaultAsync(c => c.UserId == user.Id);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString() ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.Jti, user.UserName ?? string.Empty),
                //new Claim(JwtRegisteredClaimNames.Jti, user.Id ?? string.Empty),
                new Claim(ClaimTypes.GivenName, user.UserName ?? string.Empty),
                new Claim(ClaimTypes.MobilePhone, user.PhoneNumber ?? string.Empty),
               new Claim("CenterId", center?.Id.ToString()?? "0"),
                new Claim(ClaimTypes.Role, user.Role.ToString()),
                new Claim("StudentId",student?.Id.ToString() ?? "0")
            };



            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? string.Empty));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            //Registered Claims حاجات كلنا عارفينها مش حاجات خاصة
            var token = new JwtSecurityToken
                (
                claims: claims,
                expires: DateTime.UtcNow.AddDays(7),
                signingCredentials: creds,
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"]

                );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<(string AccessToken, string RefreshToken)> GenerateTokensAsync(AppUser user)
        {
            // توليد Access Token
            var accessToken = await CreateToken(user);

            // توليد Refresh Token
            var refreshToken = Guid.NewGuid().ToString();
            var refreshTokenExpiryDays = int.Parse(_configuration.GetSection("JwtSettings")["RefreshTokenExpiryDays"] ?? "7");

            var refreshTokenEntity = new RefreshToken
            {
                Token = refreshToken,
                UserId = user.Id,
                ExpiryDate = DateTime.UtcNow.AddDays(refreshTokenExpiryDays),
                IsRevoked = false
            };

            _context.RefreshTokens.Add(refreshTokenEntity);
            await _context.SaveChangesAsync();

            return (accessToken, refreshToken);
        }

        public async Task<bool> ValidateRefreshTokenAsync(string refreshToken, string userId)
        {
            var token = await _context.RefreshTokens
                .FirstOrDefaultAsync(rt => rt.Token == refreshToken && rt.UserId == userId);

            if (token == null || token.IsRevoked || token.ExpiryDate < DateTime.UtcNow)
                return false;

            return true;
        }

        public async Task<string> GenerateNewAccessTokenAsync(string refreshToken)
        {
            var token = await _context.RefreshTokens
                .Include(rt => rt.User)
                .FirstOrDefaultAsync(rt => rt.Token == refreshToken);

            if (token == null || token.IsRevoked || token.ExpiryDate < DateTime.UtcNow)
                return null;

            return await CreateToken(token.User);
        }

        //For interface compatibility
        //public async Task<string> CreateToken(AppUser user, UserManager<AppUser> userManager)
        //{
        //    return await CreateToken(user, userManager, null);
        //}
    }
}
