using Api.PL.DTO;
using AutoMapper;
using Core.Entities;
using Core.Specefications;
using CoreProject.Enteties;
using CoreProject.Repository;
using EducationalPlatform.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static Azure.Core.HttpHeader;

namespace Api.PL.Controllers
{
    [Authorize(Roles = "Center")]
    public class TeacherController : BaseController
    {
        private readonly IGenericRepository<Teacher> _teacherService;
        private readonly IMapper _mapper;
        private readonly IGenericRepository<Subject> _subjectService;

        public TeacherController(IGenericRepository<Teacher> teacherService, IMapper mapper, IGenericRepository<Subject> subjectService)
        {
            _teacherService = teacherService;
            _mapper = mapper;
            _subjectService = subjectService;
        }
        private Guid GetCurrentCenterId()
        {
            var centerIdString = User.FindFirst("CenterId")?.Value;
            if (string.IsNullOrEmpty(centerIdString))
                throw new UnauthorizedAccessException("Center ID not found in token.");

            return Guid.Parse(centerIdString);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var centerId = GetCurrentCenterId();
            var spec = new TeachersByCenterSpec(centerId);
            var teachers = await _teacherService.GetAllWithSpecAsync(spec);
            var dto = _mapper.Map<IEnumerable<TeacherDto>>(teachers);
            return Ok(dto);
        }

        [HttpGet("{centerId}")]
        public async Task<ActionResult<IEnumerable<TeacherDto>>> GetTeachers()
        {
            var centerId = GetCurrentCenterId();
            var spec = new TeacherWithSubjectAndStudents(centerId);
            var teachers = await _teacherService.GetAllWithSpecAsync(spec);

            var result = teachers.Select(t => new TeacherDto
            {
                
                Name = t.Name,
                Email = t.Email,
                Phone = t.PhoneNumber,
                SubjectName = t.Subject.Name,
                Specialist = t.Specialist,
                IsActive = t.IsActive,
                Rating = t.Rating,
                StudentCount = t.Students.Count,
                
            });

            return Ok(result);
        }

        [HttpPost("CenterId")]
        public async Task<ActionResult<TeacherDto>> AddTeacher( [FromBody] TeacherCreateDto dto)
        {
            var centerId = GetCurrentCenterId();
            var subjectSpec = new SubjectByName(dto.SubjectName, centerId);
            var subject = await _subjectService.FristOrDefaultAsync(subjectSpec);

            if (subject == null)
                return BadRequest($"Subject '{dto.SubjectName}' not found for this center.");

            var teacher = new Teacher
            {
                 
                Name = dto.Name,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                Specialist = dto.Specialist,
                SubjectId = subject.Id,
                CenterId = centerId // 👈 نكتبها من عندنا مش من dto
            };

            await _teacherService.AddAsync(teacher);
            await _teacherService.SaveChangeAsync();

            return Ok(_mapper.Map<TeacherDto>(teacher));
        }





        [HttpPut("centerId")]
        public async Task<IActionResult> UpdateTeacher(    [FromBody] TeacherCreateDto dto)
        {
            var centerId = GetCurrentCenterId();
            var Spec= new TeachersByCenterSpec(centerId);
            var teacher = await _teacherService.GetByIdWithSpecAsync(Spec);
            if (teacher == null || teacher.CenterId != centerId)
                return NotFound("Teacher not found");

             var subjectSpec=new SubjectByName( dto.SubjectName,centerId);
            var subject = await _subjectService.FristOrDefaultAsync(subjectSpec);

            if (subject == null)
                return BadRequest($"Subject '{dto.SubjectName}' not found for this center.");

            teacher.Name = dto.Name;
            teacher.Email = dto.Email;
            teacher.PhoneNumber = dto.PhoneNumber;
            teacher.Specialist = dto.Specialist;
            teacher.SubjectId = subject.Id;

            _teacherService.Update(teacher);
            await _teacherService.SaveChangeAsync();

            return NoContent();
        }

        // 📌 حذف معلم
        [HttpDelete("centerId")]
        public async Task<IActionResult> DeleteTeacher( )
        {
            var centerId = GetCurrentCenterId();
            var Spec = new TeachersByCenterSpec(centerId);
            var teacher = await _teacherService.GetByIdWithSpecAsync(Spec);
            if (teacher == null || teacher.CenterId != centerId)
                return NotFound("Teacher not found");

            _teacherService.Delete(teacher);
            await _teacherService.SaveChangeAsync();

            return NoContent();
        }
 
    }
    }
