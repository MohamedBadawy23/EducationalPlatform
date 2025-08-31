using Api.PL.Controllers;
using Api.PL.DTO;
using AutoMapper;
using Azure;
using Core.Entities;
using Core.Repository;
using Core.ServiceContract;
using EducationalPlatform.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Concurrent;
using System.Security.Claims;

namespace EducationalPlatform.Controllers
{

    public class AccountController : BaseController
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IMapper _mapper;
        private readonly ITokenServices _tokenServices;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly Core.ServiceContract.IEmailSender _emailSender;
        private readonly IOtpService _OtpService;
      



        public AccountController(UserManager<AppUser> userManager, IMapper mapper, ITokenServices tokenServices, SignInManager<AppUser> signInManager, Core.ServiceContract.IEmailSender emailSender, IOtpService OtpService
)
        {
            _userManager = userManager;
            _mapper = mapper;
            _tokenServices = tokenServices;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _OtpService = OtpService;
             
        }
        [HttpPost("Register-Student")]
        public async Task<IActionResult> RegisterStudent(RegisterDTO dto)
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
            userDto.Token = await _tokenServices.CreateToken(user /*  , _userManager*/);
            userDto.Role = "Student";
            return Ok(userDto);
        }
         


        //[HttpPost("login2")]
        //public async Task<IActionResult> Login([FromBody] LoginDTO model)
        //{
        //    var user = await _userManager.FindByNameAsync(model.PhoneNumber);
        //    if (user is null) return Unauthorized(new { message = "User not found" });
        //    var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
        //    if (!result.Succeeded) return Unauthorized(new { message = "Invalid credentials" });
        //    var roles = await _userManager.GetRolesAsync(user);
        //    string role = roles.FirstOrDefault() ?? user.Role.ToString();
        //    var userDto = _mapper.Map<UserDto>(user);
        //    userDto.Password = model.Password;
        //    userDto.Token = await _tokenServices.CreateToken(user /*  , _userManager*/);
        //    userDto.Role = role;
        //    return Ok(userDto);
        //}

        //[HttpPost("login-with-phone")]
        //public async Task<IActionResult> LoginWithPhone([FromBody] LoginDTO model)
        //{
        //    var user = await _userManager.FindByNameAsync(model.PhoneNumber);
        //    if (user == null) return Unauthorized(new { message = "User not found" });

        //    if (!await _userManager.CheckPasswordAsync(user, model.Password))
        //        return Unauthorized(new { message = "Invalid password" });

        //    var token = await _tokenServices.CreateToken(user /*  , _userManager*/);

        //    return Ok(new
        //    {
        //        Token = token

        //    });
        //}




        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO model)
        {
            var user = await _userManager.FindByNameAsync(model.PhoneNumber);
            if (user is null) return Unauthorized(new { message = "User not found" });
            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
            if (!result.Succeeded) return Unauthorized(new { message = "Invalid credentials" });
            var roles = await _userManager.GetRolesAsync(user);
            string role = roles.FirstOrDefault() ?? user.Role.ToString();
            var (accessToken, refreshToken) = await _tokenServices.GenerateTokensAsync(user);
            var userDto = new UserDto
            {

                Role = role,
                Token = accessToken,
                RefreshToken = refreshToken

            };



            return Ok(new { user = userDto }); //refreshToken });
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







        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDto DTo)
        {
            var newAccessToken = await _tokenServices.GenerateNewAccessTokenAsync(DTo.RefreshToken);
            if (newAccessToken == null)
                return BadRequest("Invalid or expired refresh token");

            return Ok(new { Token = newAccessToken });
        }

        [HttpPost("request-otp")]
        [Authorize]
        public async Task<IActionResult> RequestOtp()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User ID not found in token.");

            try
            {
                await _OtpService.GenerateOtpAsync(userId);
                return Ok(new { Message = "تم إرسال رمز التحقق على بريدك الإلكتروني." });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        #region Password Change with OTP




        [HttpPost("reset-password")]
        [Authorize]
        public async Task<IActionResult> ResetPassword([FromBody] ChangePasswordDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User ID not found in token.");

            try
            {
                await _OtpService.ResetPasswordAsync(userId, dto.OtpCode, dto.NewPassword);
                return Ok(new { Message = "تم تغيير كلمة المرور بنجاح." });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        #endregion

        #region Phone Change with OTP
        [HttpPost("reset-phone")]
        [Authorize]
        public async Task<IActionResult> ResetPhone([FromBody] ChangePhoneDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User ID not found in token.");

            try
            {
                await _OtpService.ResetPhoneAsync(userId, dto.OtpCode, dto.NewPhoneNumber);
                return Ok(new { Message = "تم تغيير رقم الهاتف بنجاح." });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        #endregion

        
        }

    }

