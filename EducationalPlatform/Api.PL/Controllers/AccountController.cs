using AutoMapper;
using Core.Repository;
using CoreProject.Enteties;
using EducationalPlatform.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Collections.Concurrent;
using RepositoryProject;
using Api.PL.Controllers;
using Core.Entities;

namespace EducationalPlatform.Controllers
{
     
    public class AccountController : BaseController
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IMapper _mapper;
        private readonly ITokenServices _tokenServices;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IEmailSender _emailSender;

        // In-memory store for email change codes
        private static ConcurrentDictionary<string, (string Code, string NewEmail, DateTime Expiry)> _emailChangeCodes = new();
        // In-memory store for password reset codes
        private static ConcurrentDictionary<string, (string Code, DateTime Expiry)> _passwordResetCodes = new();
        // In-memory store for phone verification codes
        private static ConcurrentDictionary<string, (string Code, string ChangeType, string NewValue, DateTime Expiry)> _phoneVerificationCodes = new();

        public AccountController(UserManager<AppUser> userManager, IMapper mapper, ITokenServices tokenServices, SignInManager<AppUser> signInManager, IEmailSender emailSender)
        {
            _userManager = userManager;
            _mapper = mapper;
            _tokenServices = tokenServices;
            _signInManager = signInManager;
            _emailSender = emailSender;
        }
        [HttpPost("Register-Student")]
        public async Task<IActionResult> RegisterCustomer(RegisterDTO dto)
        {
            var user = _mapper.Map<AppUser>(dto);
            user.Role = UserRole.Student;
            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description);
                return BadRequest(new { errors = errors.ToList() });
            }
            await _userManager.AddToRoleAsync(user, "Student");
            var userDto = _mapper.Map<UserDto>(user);
            userDto.Password = dto.Password;
            userDto.Token = await _tokenServices.CreateToken(user, _userManager);
            userDto.Role = "Student";
            return Ok(userDto);
        }
        //private string GenerateRefreshToken()
        //{
        //    var randomBytes = new byte[64];
        //    using (var rng = System.Security.Cryptography.RandomNumberGenerator.Create())
        //    {
        //        rng.GetBytes(randomBytes);
        //    }
        //    return Convert.ToBase64String(randomBytes);
        //}


        [HttpPost("login2")]
        public async Task<IActionResult> Login([FromBody] LoginDTO model)
        {
            var user = await _userManager.FindByNameAsync(model.PhoneNumber);
            if (user is null) return Unauthorized(new { message = "User not found" });
            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
            if (!result.Succeeded) return Unauthorized(new { message = "Invalid credentials" });
            var roles = await _userManager.GetRolesAsync(user);
            string role = roles.FirstOrDefault() ?? user.Role.ToString();
            var userDto = _mapper.Map<UserDto>(user);
            userDto.Password = model.Password;
            userDto.Token = await _tokenServices.CreateToken(user, _userManager);
            userDto.Role = role;
            return Ok(userDto);
        }

        [HttpPost("login-with-phone")]
        public async Task<IActionResult> LoginWithPhone([FromBody] LoginDTO model)
        {
            var user = await _userManager.FindByNameAsync(model.PhoneNumber);
            if (user == null) return Unauthorized(new { message = "User not found" });
            
            if (!await _userManager.CheckPasswordAsync(user, model.Password))
                return Unauthorized(new { message = "Invalid password" });
                
            var token = await _tokenServices.CreateToken(user, _userManager);
            
            return Ok(new 
            {
                Token = token
               
            });
        }




        [HttpPost("login")]
        public async Task<IActionResult> Login2([FromBody] LoginDTO model)
        {
            var user = await _userManager.FindByNameAsync(model.PhoneNumber);
            if (user is null) return Unauthorized(new { message = "User not found" });
            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
            if (!result.Succeeded) return Unauthorized(new { message = "Invalid credentials" });
            var roles = await _userManager.GetRolesAsync(user);
            string role = roles.FirstOrDefault() ?? user.Role.ToString();

            var userDto = new UserDto
            {
                Id = user.Id,
                FullName = user.UserName,
                PhoneNumber = user.PhoneNumber,
                Role = role,
                Token = await _tokenServices.CreateToken(user, _userManager)
            };

            //// Generate and store refresh token
            //var refreshToken = GenerateRefreshToken();
            //var dbContext = HttpContext.RequestServices.GetService(typeof(EducationalPlatformDbContext)) as EducationalPlatformDbContext;
            //dbContext.RefreshTokens.Add(new RefreshToken
            //{
            //    Token = refreshToken,
            //    UserId = user.Id,
            //    Created = DateTime.UtcNow,
            //    Expires = DateTime.UtcNow.AddDays(7)
            //});
            //await dbContext.SaveChangesAsync();

            return Ok(new { user = userDto }); //refreshToken });
        }

        [Authorize(Roles = "Student")]
        [HttpPost("request-password-change-verification")]
        public async Task<IActionResult> RequestPasswordChangeVerification()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound();

            var code = new Random().Next(100000, 999999).ToString();
            var expiry = DateTime.UtcNow.AddMinutes(10);
            _phoneVerificationCodes[userId] = (code, "password", "", expiry);

            // Send SMS verification code (for now, just log it)
            Console.WriteLine($"Password change verification code for {user.PhoneNumber}: {code}");
            
            return Ok(new { 
                message = $"Password change verification code sent to {user.PhoneNumber}",
                phoneNumber = user.PhoneNumber
            });
        }

        [Authorize(Roles = "Student")]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
        {
            if (string.IsNullOrEmpty(dto.VerificationCode) || string.IsNullOrEmpty(dto.NewPassword))
                return BadRequest("Verification code and new password are required");

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound();

            if (!_phoneVerificationCodes.TryGetValue(userId, out var entry))
                return BadRequest("No verification code found. Please request a code first.");

            if (entry.ChangeType != "password")
                return BadRequest("Verification code is not for password change.");

            if (entry.Expiry < DateTime.UtcNow)
            {
                _phoneVerificationCodes.TryRemove(userId, out _);
                return BadRequest("Verification code expired. Please request a new code.");
            }

            if (entry.Code != dto.VerificationCode)
                return BadRequest("Invalid verification code.");

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, dto.NewPassword);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            _phoneVerificationCodes.TryRemove(userId, out _);
            return Ok("Password changed successfully.");
        }

        [Authorize(Roles = "Student")]
        [HttpPost("request-phone-change-verification")]
        public async Task<IActionResult> RequestPhoneChangeVerification()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound();

            var code = new Random().Next(100000, 999999).ToString();
            var expiry = DateTime.UtcNow.AddMinutes(10);
            _phoneVerificationCodes[userId] = (code, "phone", "", expiry);

            // Send SMS verification code (for now, just log it)
            Console.WriteLine($"Phone change verification code for {user.PhoneNumber}: {code}");
            
            return Ok(new { 
                message = $"Phone change verification code sent to {user.PhoneNumber}",
                phoneNumber = user.PhoneNumber
            });
        }

        [Authorize(Roles = "Student")]
        [HttpPost("change-phone")]
        public async Task<IActionResult> ChangePhone([FromBody] ChangePhoneDto dto)
        {
            if (string.IsNullOrEmpty(dto.VerificationCode) || string.IsNullOrEmpty(dto.NewPhoneNumber))
                return BadRequest("Verification code and new phone number are required");

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound();

            if (!_phoneVerificationCodes.TryGetValue(userId, out var entry))
                return BadRequest("No verification code found. Please request a code first.");

            if (entry.ChangeType != "phone")
                return BadRequest("Verification code is not for phone change.");

            if (entry.Expiry < DateTime.UtcNow)
            {
                _phoneVerificationCodes.TryRemove(userId, out _);
                return BadRequest("Verification code expired. Please request a new code.");
            }

            if (entry.Code != dto.VerificationCode)
                return BadRequest("Invalid verification code.");

            // Check if new phone number is already in use
            var existingUser = await _userManager.FindByNameAsync(dto.NewPhoneNumber);
            if (existingUser != null)
                return BadRequest("Phone number is already in use");

            user.UserName = dto.NewPhoneNumber;
            user.PhoneNumber = dto.NewPhoneNumber;
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            _phoneVerificationCodes.TryRemove(userId, out _);
            return Ok("Phone number changed successfully.");
        }

        [Authorize(Roles = "Student")]
        [HttpPut("change-name")]
        public async Task<IActionResult> ChangeName([FromBody] ChangeNameDto dto)
        {
            if (string.IsNullOrEmpty(dto.NewName))
                return BadRequest("New name is required");

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound();

            user.UserName = dto.NewName;
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok("Name changed successfully.");
        }

        [Authorize(Roles = "Student,Admin")]
        [HttpPut("update-account")]
        public async Task<IActionResult> UpdateAccount([FromBody] UpdateAccountDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound();

            // Update fields (only name, no password or phone changes here)
            if (!string.IsNullOrEmpty(dto.FullName)) 
                user.UserName = dto.FullName;

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
                return BadRequest(updateResult.Errors);

            return Ok("Account updated successfully.");
        }

        [Authorize(Roles = "Student,Admin")]
        [HttpDelete("delete-account")]
        public async Task<IActionResult> DeleteAccount()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound();

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok("Account deleted successfully.");
        }

        //[Authorize(Roles = "Student,Admin")]
        //[HttpPost("request-email-change")]
        //public async Task<IActionResult> RequestEmailChange([FromBody] RequestEmailChangeDto dto)
        //{
        //    if (string.IsNullOrEmpty(dto.NewEmail))
        //        return BadRequest("New email is required.");

        //    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        //    var code = new Random().Next(100000, 999999).ToString();
        //    var expiry = DateTime.UtcNow.AddMinutes(10);
        //    _emailChangeCodes[userId] = (code, dto.NewEmail, expiry);

        //    await _emailSender.SendEmailAsync(dto.NewEmail, "Email Change Verification Code", $"Your verification code is: {code}");
        //    return Ok("Verification code sent to new email.");
        //}

        //[Authorize(Roles = "Student,Admin")]
        //[HttpPost("confirm-email-change")]
        //public async Task<IActionResult> ConfirmEmailChange([FromBody] ConfirmEmailChangeDto dto)
        //{
        //    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        //    if (!_emailChangeCodes.TryGetValue(userId, out var entry))
        //        return BadRequest("No verification code found. Please request a code first.");
        //    if (entry.NewEmail != dto.NewEmail)
        //        return BadRequest("Email does not match the requested change.");
        //    if (entry.Expiry < DateTime.UtcNow)
        //    {
        //        _emailChangeCodes.TryRemove(userId, out _);
        //        return BadRequest("Verification code expired. Please request a new code.");
        //    }
        //    if (entry.Code != dto.VerificationCode)
        //        return BadRequest("Invalid verification code.");

        //    var user = await _userManager.FindByIdAsync(userId);
        //    if (user == null)
        //        return NotFound();
        //    user.Email = dto.NewEmail;
        //    var result = await _userManager.UpdateAsync(user);
        //    _emailChangeCodes.TryRemove(userId, out _);
        //    if (!result.Succeeded)
        //        return BadRequest(result.Errors);
        //    return Ok("Email updated successfully.");
        //}

        //[HttpPost("forgot-password")]
        //public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequestDto dto)
        //{
        //    if (string.IsNullOrEmpty(dto.Email))
        //        return BadRequest("Email is required.");

        //    var user = await _userManager.FindByEmailAsync(dto.Email);
        //    if (user == null)
        //        return Ok("If the email exists, a verification code has been sent."); // Don't reveal if email exists

        //    var code = new Random().Next(100000, 999999).ToString();
        //    var expiry = DateTime.UtcNow.AddMinutes(10);
        //    _passwordResetCodes[user.Id] = (code, expiry);

        //    await _emailSender.SendEmailAsync(dto.Email, "Password Reset Verification Code", $"Your verification code is: {code}");
        //    return Ok("If the email exists, a verification code has been sent.");
        //}

        [HttpPost("reset-password")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
        {
            if (string.IsNullOrEmpty(dto.Email) || string.IsNullOrEmpty(dto.VerificationCode) || string.IsNullOrEmpty(dto.NewPassword))
                return BadRequest("Email, verification code, and new password are required.");

            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                return BadRequest("Invalid email or verification code.");

            if (!_passwordResetCodes.TryGetValue(user.Id, out var entry))
                return BadRequest("No verification code found. Please request a code first.");
            if (entry.Expiry < DateTime.UtcNow)
            {
                _passwordResetCodes.TryRemove(user.Id, out _);
                return BadRequest("Verification code expired. Please request a new code.");
            }
            if (entry.Code != dto.VerificationCode)
                return BadRequest("Invalid verification code.");

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, dto.NewPassword);
            _passwordResetCodes.TryRemove(user.Id, out _);
            if (!result.Succeeded)
                return BadRequest(result.Errors);
            return Ok("Password reset successfully.");
        }

        //[HttpPost("refresh-token")]
        //public async Task<IActionResult> RefreshToken([FromBody] string refreshToken)
        //{
        //    var dbContext = HttpContext.RequestServices.GetService(typeof(EducationalPlatformDbContext)) as EducationalPlatformDbContext;
        //    var tokenEntity = dbContext.RefreshTokens.FirstOrDefault(rt => rt.Token == refreshToken && rt.IsActive);
        //    if (tokenEntity == null)
        //        return Unauthorized("Invalid or expired refresh token.");

        //    var user = await _userManager.FindByIdAsync(tokenEntity.UserId);
        //    if (user == null)
        //        return Unauthorized("User not found.");

        //    // Optionally: Invalidate the old refresh token and issue a new one
        //    tokenEntity.Revoked = DateTime.UtcNow;
        //    var newRefreshToken = GenerateRefreshToken();
        //    dbContext.RefreshTokens.Add(new RefreshToken
        //    {
        //        Token = newRefreshToken,
        //        UserId = user.Id,
        //        Created = DateTime.UtcNow,
        //        Expires = DateTime.UtcNow.AddDays(7)
        //    });
        //    await dbContext.SaveChangesAsync();

        //    var newAccessToken = await _tokenServices.CreateToken(user, _userManager);
        //    return Ok(new { accessToken = newAccessToken, refreshToken = newRefreshToken });
        //}
    }
}
