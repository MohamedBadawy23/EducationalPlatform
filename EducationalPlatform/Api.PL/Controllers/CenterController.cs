using Api.PL.Controllers;
using Api.PL.DTO;
using AutoMapper;
using Core.Entities;
using CoreProject.Enteties;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RepositoryProject;
using System.Security.Claims;

namespace EducationalPlatform.Controllers
{
    [Authorize(Roles = "Center")]
    [Route("api/[controller]")]
    [ApiController]
    public class CenterController : BaseController
    {
        private readonly EducationalPlatformDbContext _context;
        private readonly IMapper _mapper;
        private readonly UserManager<AppUser> _userManager;

        public CenterController(EducationalPlatformDbContext context, IMapper mapper, UserManager<AppUser> userManager)
        {
            _context = context;
            _mapper = mapper;
            _userManager = userManager;
        }

        // Helper method to get current center ID
        private async Task<int> GetCurrentCenterId()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var center = await _context.Centers.FirstOrDefaultAsync(c => c.UserId == userId);
            return center?.Id ?? 0;
        }

        #region Teacher and Subject Management

        [HttpPost("teachers")]
        public async Task<IActionResult> AddTeacherWithSubject([FromBody] AddTeacherWithSubject dto)
        {
            var centerId = await GetCurrentCenterId();
            if (centerId == 0)
                return Unauthorized("Center not found");

            // Check if teacher already exists
            var existingTeacher = await _context.Teachers
                .FirstOrDefaultAsync(t => t.Name == dto.TeacherName && t.CenterId == centerId);

            if (existingTeacher != null)
                return BadRequest("Teacher already exists in this center");

            // Create or find subject
            var subject = await _context.Subjects
                .FirstOrDefaultAsync(s => s.Name == dto.SubjectName);

            if (subject == null)
            {
                subject = new Subject
                {
                    Name = dto.SubjectName,
                    Description = $"Subject: {dto.SubjectName}"
                };
                await _context.Subjects.AddAsync(subject);
                await _context.SaveChangesAsync();
            }

            // Create teacher
            var teacher = new Teacher
            {
                Name = dto.TeacherName,
                Specialist = dto.SubjectName,
                CenterId = centerId,
                Subjects = new List<Subject> { subject }
            };

            await _context.Teachers.AddAsync(teacher);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Teacher and subject added successfully", teacherId = teacher.Id });
        }

        [HttpGet("teachers")]
        public async Task<IActionResult> GetTeachers()
        {
            var centerId = await GetCurrentCenterId();
            if (centerId == 0)
                return Unauthorized("Center not found");

            var teachers = await _context.Teachers
                .Where(t => t.CenterId == centerId)
                .Include(t => t.Subjects)
                .Select(t => new TeacherWithSubjectsDto
                {
                    Id = t.Id,
                    Name = t.Name,
                    Specialist = t.Specialist,
                    Subjects = t.Subjects.Select(s => s.Name).ToList()
                })
                .ToListAsync();

            return Ok(teachers);
        }

        [HttpPut("teachers/{teacherId}")]
        public async Task<IActionResult> UpdateTeacher(int teacherId, [FromBody] UpdateTeacherDto dto)
        {
            var centerId = await GetCurrentCenterId();
            if (centerId == 0)
                return Unauthorized("Center not found");

            var teacher = await _context.Teachers
                .FirstOrDefaultAsync(t => t.Id == teacherId && t.CenterId == centerId);

            if (teacher == null)
                return NotFound("Teacher not found");

            teacher.Name = dto.Name;
            teacher.Specialist = dto.Specialist;

            await _context.SaveChangesAsync();
            return Ok(new { message = "Teacher updated successfully" });
        }

        [HttpDelete("teachers/{teacherId}")]
        public async Task<IActionResult> DeleteTeacher(int teacherId)
        {
            var centerId = await GetCurrentCenterId();
            if (centerId == 0)
                return Unauthorized("Center not found");

            var teacher = await _context.Teachers
                .FirstOrDefaultAsync(t => t.Id == teacherId && t.CenterId == centerId);

            if (teacher == null)
                return NotFound("Teacher not found");

            _context.Teachers.Remove(teacher);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Teacher deleted successfully" });
        }

        #endregion

        #region Lesson Scheduling

        [HttpPost("lessons")]
        public async Task<IActionResult> ScheduleLesson([FromBody] ScheduleLessonDto dto)
        {
            var centerId = await GetCurrentCenterId();
            if (centerId == 0)
                return Unauthorized("Center not found");

            // Verify subject belongs to this center
            var subject = await _context.Subjects
                .Include(s => s.Teacher)
                .FirstOrDefaultAsync(s => s.Id == dto.SubjectId && s.Teacher.CenterId == centerId);

            if (subject == null)
                return NotFound("Subject not found or doesn't belong to this center");

            // Check for time conflicts
            var conflictingLesson = await _context.Lessons
                .Where(l => l.SubjectId == dto.SubjectId)
                .AnyAsync(l => (l.StartTime <= dto.StartTime && l.EndTime > dto.StartTime) ||
                               (l.StartTime < dto.EndTime && l.EndTime >= dto.EndTime) ||
                               (dto.StartTime <= l.StartTime && dto.EndTime >= l.EndTime));

            if (conflictingLesson)
                return BadRequest("Time slot conflicts with existing lesson");

            var lesson = new Lesson
            {
                Title = $"{subject.Name} - {dto.StartTime:HH:mm}",
                StartTime = dto.StartTime,
                EndTime = dto.EndTime,
                SubjectId = dto.SubjectId
            };

            await _context.Lessons.AddAsync(lesson);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Lesson scheduled successfully", lessonId = lesson.Id });
        }

        [HttpGet("lessons")]
        public async Task<IActionResult> GetLessons()
        {
            var centerId = await GetCurrentCenterId();
            if (centerId == 0)
                return Unauthorized("Center not found");

            var lessons = await _context.Lessons
                .Include(l => l.Subject)
                .ThenInclude(s => s.Teacher)
                .Where(l => l.Subject.Teacher.CenterId == centerId)
                .Select(l => new LessonScheduleDto
                {
                    Id = l.Id,
                    Title = l.Title,
                    StartTime = l.StartTime,
                    EndTime = l.EndTime,
                    SubjectName = l.Subject.Name,
                    TeacherName = l.Subject.Teacher.Name
                })
                .OrderBy(l => l.StartTime)
                .ToListAsync();

            return Ok(lessons);
        }

        [HttpPut("lessons/{lessonId}")]
        public async Task<IActionResult> UpdateLesson(int lessonId, [FromBody] ScheduleLessonDto dto)
        {
            var centerId = await GetCurrentCenterId();
            if (centerId == 0)
                return Unauthorized("Center not found");

            var lesson = await _context.Lessons
                .Include(l => l.Subject)
                .ThenInclude(s => s.Teacher)
                .FirstOrDefaultAsync(l => l.Id == lessonId && l.Subject.Teacher.CenterId == centerId);

            if (lesson == null)
                return NotFound("Lesson not found");

            lesson.StartTime = dto.StartTime;
            lesson.EndTime = dto.EndTime;
            lesson.Title = $"{lesson.Subject.Name} - {dto.StartTime:HH:mm}";

            await _context.SaveChangesAsync();
            return Ok(new { message = "Lesson updated successfully" });
        }

        [HttpDelete("lessons/{lessonId}")]
        public async Task<IActionResult> DeleteLesson(int lessonId)
        {
            var centerId = await GetCurrentCenterId();
            if (centerId == 0)
                return Unauthorized("Center not found");

            var lesson = await _context.Lessons
                .Include(l => l.Subject)
                .ThenInclude(s => s.Teacher)
                .FirstOrDefaultAsync(l => l.Id == lessonId && l.Subject.Teacher.CenterId == centerId);

            if (lesson == null)
                return NotFound("Lesson not found");

            _context.Lessons.Remove(lesson);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Lesson deleted successfully" });
        }

        #endregion

        #region Material Upload

        [HttpPost("materials")]
        public async Task<IActionResult> UploadMaterial([FromBody] UploadMaterialDto dto)
        {
            var centerId = await GetCurrentCenterId();
            if (centerId == 0)
                return Unauthorized("Center not found");

            // Verify subject belongs to this center
            var subject = await _context.Subjects
                .Include(s => s.Teacher)
                .FirstOrDefaultAsync(s => s.Id == dto.SubjectId && s.Teacher.CenterId == centerId);

            if (subject == null)
                return NotFound("Subject not found or doesn't belong to this center");

            // Create a lesson for the material if it doesn't exist
            var lesson = await _context.Lessons
                .FirstOrDefaultAsync(l => l.SubjectId == dto.SubjectId && l.Title == dto.Title);

            if (lesson == null)
            {
                lesson = new Lesson
                {
                    Title = dto.Title,
                    StartTime = DateTime.Now,
                    EndTime = DateTime.Now.AddHours(1),
                    SubjectId = dto.SubjectId
                };
                await _context.Lessons.AddAsync(lesson);
                await _context.SaveChangesAsync();
            }

            var material = new LessonMaterial
            {
                FilePath = dto.FilePath,
                Description = dto.Title,
                UploadedAt = DateTime.Now,
                LessonId = lesson.Id
            };

            await _context.LessonMaterials.AddAsync(material);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Material uploaded successfully", materialId = material.Id });
        }

        [HttpGet("materials")]
        public async Task<IActionResult> GetMaterials()
        {
            var centerId = await GetCurrentCenterId();
            if (centerId == 0)
                return Unauthorized("Center not found");

            var materials = await _context.LessonMaterials
                .Include(m => m.Lesson)
                .ThenInclude(l => l.Subject)
                .ThenInclude(s => s.Teacher)
                .Where(m => m.Lesson.Subject.Teacher.CenterId == centerId)
                .Select(m => new EducationalMaterialDto
                {
                    Id = m.Id,
                    Title = m.Description,
                    FilePath = m.FilePath,
                    SubjectName = m.Lesson.Subject.Name,
                    TeacherName = m.Lesson.Subject.Teacher.Name,
                    UploadedAt = m.UploadedAt
                })
                .OrderByDescending(m => m.UploadedAt)
                .ToListAsync();

            return Ok(materials);
        }

        #endregion

        #region Student Management

        [HttpGet("students")]
        public async Task<IActionResult> GetStudents()
        {
            var centerId = await GetCurrentCenterId();
            if (centerId == 0)
                return Unauthorized("Center not found");

            var students = await _context.Students
                .Where(s => s.CenterId == centerId)
                .Include(s => s.Subjects)
                .ThenInclude(sub => sub.Teacher)
                .Select(s => new StudentWithSubjectsDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    Grade = s.Grade,
                    PhoneNumber = s.PhoneNumber,
                    HasPaid = s.HasPaid,
                    EnrollmentDate = s.EnrollmentDate,
                    Subjects = s.Subjects.Select(sub => new StudentSubjectInfoDto
                    {
                        SubjectId = sub.Id,
                        SubjectName = sub.Name,
                        TeacherName = sub.Teacher?.Name ?? "Not Assigned"
                    }).ToList()
                })
                .ToListAsync();

            return Ok(students);
        }

        [HttpGet("students/{studentId}")]
        public async Task<IActionResult> GetStudent(int studentId)
        {
            var centerId = await GetCurrentCenterId();
            if (centerId == 0)
                return Unauthorized("Center not found");

            var student = await _context.Students
                .Where(s => s.Id == studentId && s.CenterId == centerId)
                .Include(s => s.Subjects)
                .ThenInclude(sub => sub.Teacher)
                .Select(s => new StudentWithSubjectsDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    Grade = s.Grade,
                    PhoneNumber = s.PhoneNumber,
                    HasPaid = s.HasPaid,
                    EnrollmentDate = s.EnrollmentDate,
                    Subjects = s.Subjects.Select(sub => new StudentSubjectInfoDto
                    {
                        SubjectId = sub.Id,
                        SubjectName = sub.Name,
                        TeacherName = sub.Teacher?.Name ?? "Not Assigned"
                    }).ToList()
                })
                .FirstOrDefaultAsync();

            if (student == null)
                return NotFound("Student not found");

            return Ok(student);
        }

        [HttpPut("students/payment")]
        public async Task<IActionResult> UpdatePaymentStatus([FromBody] UpdatePaymentStatusDto dto)
        {
            var centerId = await GetCurrentCenterId();
            if (centerId == 0)
                return Unauthorized("Center not found");

            var student = await _context.Students
                .FirstOrDefaultAsync(s => s.Id == dto.StudentId && s.CenterId == centerId);

            if (student == null)
                return NotFound("Student not found");

            student.HasPaid = dto.HasPaid;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Payment status updated successfully" });
        }

        [HttpGet("students/unpaid")]
        public async Task<IActionResult> GetUnpaidStudents()
        {
            var centerId = await GetCurrentCenterId();
            if (centerId == 0)
                return Unauthorized("Center not found");

            var unpaidStudents = await _context.Students
                .Where(s => s.CenterId == centerId && !s.HasPaid)
                .Select(s => new
                {
                    s.Id,
                    s.Name,
                    s.PhoneNumber,
                    s.Grade,
                    s.EnrollmentDate
                })
                .ToListAsync();

            return Ok(unpaidStudents);
        }

        #endregion

        #region Messaging

        [HttpPost("messages")]
        public async Task<IActionResult> SendMessage([FromBody] SendMessageDto dto)
        {
            var centerId = await GetCurrentCenterId();
            if (centerId == 0)
                return Unauthorized("Center not found");

            // Verify student belongs to this center
            var student = await _context.Students
                .FirstOrDefaultAsync(s => s.Id == dto.StudentId && s.CenterId == centerId);

            if (student == null)
                return NotFound("Student not found");

            // Get student's user account
            var studentUser = await _userManager.Users
                .FirstOrDefaultAsync(u => u.PhoneNumber == student.PhoneNumber);

            if (studentUser == null)
                return BadRequest("Student user account not found");

            var message = new Message
            {
                Content = dto.Content,
                SentDate = DateTime.Now,
                IsRead = false,
                UserId = studentUser.Id
            };

            await _context.messages.AddAsync(message);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Message sent successfully", messageId = message.Id });
        }

        [HttpGet("messages/sent")]
        public async Task<IActionResult> GetSentMessages()
        {
            var centerId = await GetCurrentCenterId();
            if (centerId == 0)
                return Unauthorized("Center not found");

            // Get all students in this center
            var studentPhoneNumbers = await _context.Students
                .Where(s => s.CenterId == centerId)
                .Select(s => s.PhoneNumber)
                .ToListAsync();

            var sentMessages = await _context.messages
                .Include(m => m.AppUser)
                .Where(m => studentPhoneNumbers.Contains(m.AppUser.PhoneNumber))
                .Select(m => new
                {
                    m.Id,
                    m.Content,
                    m.SentDate,
                    m.IsRead,
                    StudentName = m.AppUser.FullName,
                    StudentPhone = m.AppUser.PhoneNumber
                })
                .OrderByDescending(m => m.SentDate)
                .ToListAsync();

            return Ok(sentMessages);
        }

        #endregion

        #region Dashboard Statistics

        [HttpGet("dashboard")]
        public async Task<IActionResult> GetDashboardStats()
        {
            var centerId = await GetCurrentCenterId();
            if (centerId == 0)
                return Unauthorized("Center not found");

            var stats = new
            {
                TotalStudents = await _context.Students.CountAsync(s => s.CenterId == centerId),
                TotalTeachers = await _context.Teachers.CountAsync(t => t.CenterId == centerId),
                TotalSubjects = await _context.Subjects
                    .Include(s => s.Teacher)
                    .CountAsync(s => s.Teacher.CenterId == centerId),
                UnpaidStudents = await _context.Students.CountAsync(s => s.CenterId == centerId && !s.HasPaid),
                TotalLessons = await _context.Lessons
                    .Include(l => l.Subject)
                    .ThenInclude(s => s.Teacher)
                    .CountAsync(l => l.Subject.Teacher.CenterId == centerId),
                TotalMaterials = await _context.LessonMaterials
                    .Include(m => m.Lesson)
                    .ThenInclude(l => l.Subject)
                    .ThenInclude(s => s.Teacher)
                    .CountAsync(m => m.Lesson.Subject.Teacher.CenterId == centerId)
            };

            return Ok(stats);
        }

        #endregion
    }
} 