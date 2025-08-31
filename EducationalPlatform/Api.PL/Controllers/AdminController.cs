using Api.PL.Controllers;
using Api.PL.DTO;
using AutoMapper;
using Core.Entities;
using CoreProject.Enteties;
using CoreProject.Repository;
using CoreProject.Specefications;
using EducationalPlatform.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RepositoryProject;
using System.Security.Claims;
using Talabat.Core.Specefications;
using Talabat.Repository;

namespace EducationalPlatform.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : BaseController
    {
        private readonly EducationalPlatformDbContext _context;
        private readonly IMapper _mapper;
        private readonly UserManager<AppUser> _userManager;
        private readonly IGenericRepository<Center> _repository;

        public AdminController(EducationalPlatformDbContext context, IMapper mapper, UserManager<AppUser> userManager, IGenericRepository<Center> repository)
        {
            _context = context;
            _mapper = mapper;
            _userManager = userManager;
            _repository = repository;
        }

        #region Center Management

        [HttpPost("centers")]
        public async Task<IActionResult> CreateCenter([FromBody] CreateCenterDto createCenterDto)
        {
            if (createCenterDto == null || string.IsNullOrEmpty(createCenterDto.Name) || string.IsNullOrEmpty(createCenterDto.PhoneNumber))
            {
                return BadRequest("Invalid center data. Name and PhoneNumber are required.");
            }

            // Check if phone number already exists
            var existingUser = await _userManager.FindByNameAsync(createCenterDto.PhoneNumber);
            if (existingUser != null)
            {
                return BadRequest("A center with this phone number already exists.");
            }

            // Check if center with same name exists
            var existingCenter = await _context.Centers.FirstOrDefaultAsync(c => c.CenterName == createCenterDto.Name);
            if (existingCenter != null)
            {
                return BadRequest("A center with this name already exists.");
            }

            var password = GenerateCenterCode();
            
            // Create user account for the center
            var user = new AppUser
            {
                UserName = createCenterDto.PhoneNumber,
                PhoneNumber = createCenterDto.PhoneNumber,
                FullName = createCenterDto.Name,
                Role = UserRole.Center
            };

            var result = await _userManager.CreateAsync(user, password);

            if (!result.Succeeded)
            {
                return BadRequest(new { errors = result.Errors.Select(e => e.Description).ToList() });
            }

            // Create center entity
            var center = new Center
            {
                CenterName = createCenterDto.Name,
                Address = createCenterDto.Address,
                Description = createCenterDto.Description,
                PhoneNumber = createCenterDto.PhoneNumber,
                UserId = user.Id,
                CreatedAt = DateTime.UtcNow
            };

            await _context.Centers.AddAsync(center);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Center created successfully",
                centerId = center.Id,
                phoneNumber = createCenterDto.PhoneNumber,
                password = password,
                loginInstructions = "Use the phone number as username and the generated password to login."
            });
        }

        [HttpGet("centers")]
        public async Task<IActionResult> GetCenters()
        {
            var centers = await _context.Centers
                .Select(c => new CenterDto
                {
                    Id = c.Id,
                    Name = c.CenterName,
                    PhoneNumber = c.PhoneNumber,
                    Address = c.Address
                })
                .ToListAsync();
            
            return Ok(centers);
        }

        [HttpGet("centers/{centerId}")]
        public async Task<IActionResult> GetCenterDetails(int centerId)
        {
            var center = await _context.Centers
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.Id == centerId);

            if (center == null)
                return NotFound("Center not found");

            // Get statistics for the center
            var statistics = new CenterStatisticsDto
            {
                TotalStudents = await _context.Students.CountAsync(s => s.CenterId == centerId),
                TotalTeachers = await _context.Teachers.CountAsync(t => t.CenterId == centerId),
                TotalSubjects = await _context.Subjects
                    .Include(s => s.Teacher)
                    .CountAsync(s => s.Teacher.CenterId == centerId),
                TotalLessons = await _context.Lessons
                    .Include(l => l.Subject)
                    .ThenInclude(s => s.Teacher)
                    .CountAsync(l => l.Subject.Teacher.CenterId == centerId),
                TotalMaterials = await _context.LessonMaterials
                    .Include(m => m.Lesson)
                    .ThenInclude(l => l.Subject)
                    .ThenInclude(s => s.Teacher)
                    .CountAsync(m => m.Lesson.Subject.Teacher.CenterId == centerId),
                UnpaidStudents = await _context.Students.CountAsync(s => s.CenterId == centerId && !s.HasPaid)
            };

            var centerDetails = new CenterDetailsDto
            {
                Id = center.Id,
                Name = center.CenterName,
                Address = center.Address,
                Description = center.Description,
                PhoneNumber = center.PhoneNumber,
                CreatedAt = center.CreatedAt,
                UserId = center.UserId,
                Statistics = statistics
            };

            return Ok(centerDetails);
        }

        [HttpGet("centers/details/all")]
        public async Task<IActionResult> GetAllCentersWithDetails()
        {
            var centers = await _context.Centers
                .Include(c => c.User)
                .ToListAsync();

            var centersWithDetails = new List<CenterDetailsDto>();

            foreach (var center in centers)
            {
                var statistics = new CenterStatisticsDto
                {
                    TotalStudents = await _context.Students.CountAsync(s => s.CenterId == center.Id),
                    TotalTeachers = await _context.Teachers.CountAsync(t => t.CenterId == center.Id),
                    TotalSubjects = await _context.Subjects
                        .Include(s => s.Teacher)
                        .CountAsync(s => s.Teacher.CenterId == center.Id),
                    TotalLessons = await _context.Lessons
                        .Include(l => l.Subject)
                        .ThenInclude(s => s.Teacher)
                        .CountAsync(l => l.Subject.Teacher.CenterId == center.Id),
                    TotalMaterials = await _context.LessonMaterials
                        .Include(m => m.Lesson)
                        .ThenInclude(l => l.Subject)
                        .ThenInclude(s => s.Teacher)
                        .CountAsync(m => m.Lesson.Subject.Teacher.CenterId == center.Id),
                    UnpaidStudents = await _context.Students.CountAsync(s => s.CenterId == center.Id && !s.HasPaid)
                };

                centersWithDetails.Add(new CenterDetailsDto
                {
                    Id = center.Id,
                    Name = center.CenterName,
                    Address = center.Address,
                    Description = center.Description,
                    PhoneNumber = center.PhoneNumber,
                    CreatedAt = center.CreatedAt,
                    UserId = center.UserId,
                    Statistics = statistics
                });
            }

            return Ok(centersWithDetails);
        }

        [HttpPut("centers/{centerId}")]
        public async Task<IActionResult> UpdateCenter(int centerId, [FromBody] UpdateCenterDto updateCenterDto)
        {
            var center = await _context.Centers
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.Id == centerId);

            if (center == null)
                return NotFound("Center not found");

            // Check if phone number is being changed and if it already exists
            if (!string.IsNullOrEmpty(updateCenterDto.PhoneNumber) && 
                updateCenterDto.PhoneNumber != center.PhoneNumber)
            {
                var existingUser = await _userManager.FindByNameAsync(updateCenterDto.PhoneNumber);
                if (existingUser != null)
                {
                    return BadRequest("A center with this phone number already exists.");
                }
            }

            // Update center information
            if (!string.IsNullOrEmpty(updateCenterDto.Name))
                center.CenterName = updateCenterDto.Name;
            
            if (!string.IsNullOrEmpty(updateCenterDto.Address))
                center.Address = updateCenterDto.Address;
            
            if (!string.IsNullOrEmpty(updateCenterDto.PhoneNumber))
                center.PhoneNumber = updateCenterDto.PhoneNumber;

            // Update user account if phone number changed
            if (center.User != null && !string.IsNullOrEmpty(updateCenterDto.PhoneNumber))
            {
                center.User.UserName = updateCenterDto.PhoneNumber;
                center.User.PhoneNumber = updateCenterDto.PhoneNumber;
                center.User.FullName = updateCenterDto.Name ?? center.CenterName;

                var userUpdateResult = await _userManager.UpdateAsync(center.User);
                if (!userUpdateResult.Succeeded)
                {
                    return BadRequest(new { errors = userUpdateResult.Errors.Select(e => e.Description).ToList() });
                }
            }

            await _context.SaveChangesAsync();

            return Ok(new { message = "Center updated successfully" });
        }

        [HttpDelete("centers/{centerId}")]
        public async Task<IActionResult> DeleteCenter(int centerId)
        {
            var center = await _context.Centers
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.Id == centerId);

            if (center == null)
                return NotFound("Center not found");

            // Check if center has any data
            var hasStudents = await _context.Students.AnyAsync(s => s.CenterId == centerId);
            var hasTeachers = await _context.Teachers.AnyAsync(t => t.CenterId == centerId);

            if (hasStudents || hasTeachers)
            {
                return BadRequest("Cannot delete center. It has associated students or teachers. Please remove all associated data first.");
            }

            // Delete user account
            if (center.User != null)
            {
                var userDeleteResult = await _userManager.DeleteAsync(center.User);
                if (!userDeleteResult.Succeeded)
                {
                    return BadRequest(new { errors = userDeleteResult.Errors.Select(e => e.Description).ToList() });
                }
            }

            // Delete center
            _context.Centers.Remove(center);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Center deleted successfully" });
        }

        [HttpPost("centers/{centerId}/reset-password")]
        public async Task<IActionResult> ResetCenterPassword(int centerId)
        {
            var center = await _context.Centers
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.Id == centerId);

            if (center == null)
                return NotFound("Center not found");

            if (center.User == null)
                return BadRequest("Center user account not found");

            var newPassword = GenerateCenterCode();
            var token = await _userManager.GeneratePasswordResetTokenAsync(center.User);
            var result = await _userManager.ResetPasswordAsync(center.User, token, newPassword);

            if (!result.Succeeded)
            {
                return BadRequest(new { errors = result.Errors.Select(e => e.Description).ToList() });
            }

            return Ok(new
            {
                message = "Center password reset successfully",
                centerId = center.Id,
                phoneNumber = center.PhoneNumber,
                newPassword = newPassword
            });
        }

        #endregion

        #region Dashboard Statistics

        [HttpGet("dashboard")]
        public async Task<IActionResult> GetAdminDashboard()
        {
            var totalCenters = await _context.Centers.CountAsync();
            var totalStudents = await _context.Students.CountAsync();
            var totalTeachers = await _context.Teachers.CountAsync();
            var totalSubjects = await _context.Subjects.CountAsync();
            var totalLessons = await _context.Lessons.CountAsync();
            var totalMaterials = await _context.LessonMaterials.CountAsync();
            var unpaidStudents = await _context.Students.CountAsync(s => !s.HasPaid);

            var dashboardStats = new
            {
                totalCenters,
                totalStudents,
                totalTeachers,
                totalSubjects,
                totalLessons,
                totalMaterials,
                unpaidStudents,
                averageStudentsPerCenter = totalCenters > 0 ? Math.Round((double)totalStudents / totalCenters, 2) : 0,
                averageTeachersPerCenter = totalCenters > 0 ? Math.Round((double)totalTeachers / totalCenters, 2) : 0
            };

            return Ok(dashboardStats);
        }

        #endregion

        private string GenerateCenterCode()
        {
            // Generate a 6-digit code
            return new Random().Next(100000, 999999).ToString();
        }
    }
} 