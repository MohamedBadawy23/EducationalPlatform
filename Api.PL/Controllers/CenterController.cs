 
using Api.PL.DTO;
using Core.Entities;
using Core.ServiceContract;
using Core.Specefications;
using CoreProject.Enteties;
using CoreProject.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

 
 

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Center")]
public class CenterController : ControllerBase
{
    private readonly IGenericRepository<Center> _centerRepository;
    private readonly IGenericRepository<Teacher> _teacherRepository;
    private readonly IGenericRepository<Subject> _subjectRepository;
    private readonly IGenericRepository<Student> _studentRepository;
    private readonly IGenericRepository<Schedule> _scheduleRepository;
    private readonly IGenericRepository<LessonMaterial> _materialRepository;
    private readonly IGenericRepository<Message> _messageRepository;
    private readonly IGenericRepository<Room> _roomRepository;
    private readonly IOtpService _otpService;
    private readonly UserManager<AppUser> _userManager;

    //private readonly IGenericRepository<Payment> _paymentRepository;

    public CenterController(
        IGenericRepository<Center> centerRepository,
        IGenericRepository<Teacher> teacherRepository,
        IGenericRepository<Subject> subjectRepository,
        IGenericRepository<Student> studentRepository,
        IGenericRepository<Schedule> scheduleRepository,
        IGenericRepository<LessonMaterial> materialRepository,
        IGenericRepository<Message> messageRepository,
        IGenericRepository<Room> roomRepository,
        IOtpService otpService,
        UserManager<AppUser> userManager
        )
      //  IGenericRepository<Payment> paymentRepository)
    {
        _centerRepository = centerRepository;
        _teacherRepository = teacherRepository;
        _subjectRepository = subjectRepository;
        _studentRepository = studentRepository;
        _scheduleRepository = scheduleRepository;
        _materialRepository = materialRepository;
        _messageRepository = messageRepository;
        _roomRepository = roomRepository;
        _otpService = otpService;
        _userManager = userManager;
        // _paymentRepository = paymentRepository;
    }

    private Guid GetCurrentCenterIdAsync()
    {
        var centerIdString = User.FindFirst("CenterId")?.Value;
        if (string.IsNullOrEmpty(centerIdString))
            throw new UnauthorizedAccessException("Center ID not found in token.");

        return Guid.Parse(centerIdString);
    }
    private string GenerateTemporaryPassword()
    {
        // مثال: 8 أحرف، يحتوي على حرف كبير، حرف صغير، رقم
        var random = new Random();
        string upper = ((char)random.Next(65, 91)).ToString();  // حرف كبير
        string lower = ((char)random.Next(97, 123)).ToString(); // حرف صغير
        string number = random.Next(0, 10).ToString();           // رقم
        string special = "!@#".Substring(random.Next(0, 3));     // حرف خاص اختياري

        // حروف عشوائية إضافية
        string rest = Guid.NewGuid().ToString().Substring(0, 4);

        // دمج كل العناصر
        return upper + lower + number + special + rest;
    }
 

   
        // ===================== Subjects =====================
        [HttpGet("subjects")]
    public async Task<IActionResult> GetAllSubjects()
    {
        var centerId =  GetCurrentCenterIdAsync();
        var spec = new SubjectsByCenterSpec(centerId);
        var subjects = await _subjectRepository.GetAllWithSpecAsync(spec);
        return Ok(subjects);
    }

    [HttpGet("subjects/{id}")]
    //public async Task<IActionResult> GetSubjectById(int id)
    //{
    //    var centerId = await GetCurrentCenterIdAsync();
    //    var spec = new StudentByIdSpec(centerId, id);
    //    var subject = await _subjectRepository.GetByIdWithSpecAsync(spec);
    //    if (subject == null) return NotFound();
    //    return Ok(subject);
    //}

    [HttpPost("subjects")]
    public async Task<IActionResult> AddSubject([FromBody] SubjectDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            return BadRequest("Subject name is required.");

        var centerId =   GetCurrentCenterIdAsync(); // 👈 نربط المادة بالسنتر الحالي

        // إنشاء المادة وربطها بالمركز
        var subject = new Subject
        {
            
            Name = dto.Name.Trim(),
            Description = dto.Description?.Trim(),
            Price = dto.Price,
            CenterId = centerId   // 👈 هنا أهم إضافة
        };

        await _subjectRepository.AddAsync(subject);
        await _subjectRepository.SaveChangeAsync();

        return Ok(subject);
    }

    [HttpPut("subjects/{id}")]
    public async Task<IActionResult> UpdateSubject(int id, [FromBody] SubjectDto dto)
    {
        var subject = await _subjectRepository.GetByIdAsync(id);
        if (subject == null) return NotFound();
        subject.Name = dto.Name;
       // subject.TeacherId = dto.TeacherId;
       _subjectRepository.Update(subject);
        await _subjectRepository.SaveChangeAsync();
        return Ok(subject);
    }

    [HttpDelete("subjects/{id}")]
    public async Task<IActionResult> DeleteSubject(int id)
    {
        var subject = await _subjectRepository.GetByIdAsync(id);
        if (subject == null) return NotFound();
         _subjectRepository.Delete(subject);
        await _subjectRepository.SaveChangeAsync();
        return Ok("Deleted successfully");
    }

    // ===================== Students =====================
    [HttpGet("students")]
    public async Task<IActionResult> GetAllStudents()
    {
        var centerId = GetCurrentCenterIdAsync();
        var spec = new StudentsByCenterSpec(centerId);
        var students = await _studentRepository.GetAllWithSpecAsync(spec);
        var studentDto = students.Select(P => new StudentDto
        {
            FullName = P.FullName,
            PhoneNumber = P.PhoneNumber,
            IsActive = P.IsActive,
            Subjects = P.Subjects.Select(S => new SubjectIdInfoDto
            {
                Name = S.Name,
                //TeacherName =   S.Teachers.Select(P=>P.Name).ToList()
            }).ToList()

        }).ToList();
        return Ok(students);
    }

    [HttpGet("students/{id}")]
    public async Task<IActionResult> GetStudentById(int id)
    {
        var centerId = GetCurrentCenterIdAsync();
        
        var spec = new StudentByIdSpec(id,centerId);
        var student = await _studentRepository.GetByIdWithSpecAsync(spec);
        if (student == null) return NotFound();
        var studentDto = new StudentDto
        {
            FullName = student.FullName,
            PhoneNumber = student.PhoneNumber,
            IsActive = student.IsActive,
            Subjects = student.Subjects.Select(S => new SubjectIdInfoDto
            {
                Name = S.Name,
               // TeacherName = S.Teachers.Select(P=>P.Name).ToList()
            }).ToList()

        };
        return Ok(studentDto);
       
    }


    [HttpPost("students")]
    public async Task<IActionResult> AddStudent([FromBody] StudentCreateDto dto)
    {
        var centerId = GetCurrentCenterIdAsync();

        // توليد كلمة مرور عشوائية تلبي شروط Identity
        var password = "Student@" + new Random().Next(1000, 9999);

        // إنشاء حساب الطالب في Identity
        var user = new AppUser
        {
            UserName = dto.PhoneNumber, // الهاتف يصبح UserName
            Email = dto.Email,
            FullName = dto.FullName,
            PhoneNumber = dto.PhoneNumber,
            
            Role =UserRole.Student
        };

        var identityResult = await _userManager.CreateAsync(user, password);
        if (!identityResult.Succeeded)
            return BadRequest(identityResult.Errors);

        // إرسال البريد مع كلمة المرور ورقم الهاتف
        await _otpService.SendTemporaryPasswordAsync(user.Email , password,  dto.PhoneNumber);

        // إنشاء كيان Student مرتبط بـ AppUser
        var student = new Student
        {
            FullName = dto.FullName,
            PhoneNumber = dto.PhoneNumber,
            CenterId = centerId,
            UserId = user.Id,
           Email=user.Email
        };

        // ربط المواد ديناميكيًا
        if (dto.SubjectNames != null && dto.SubjectNames.Any())
        {
            var subjects = await _subjectRepository.GetAllWithSpecAsync(new SubjectsByCenterSpec(centerId));
            student.Subjects = subjects.Where(s => dto.SubjectNames.Contains(s.Name)).ToList();
        }

        await _studentRepository.AddAsync(student);
        await _studentRepository.SaveChangeAsync();

        return Ok(new { message = "Student created and credentials sent via email." });
    }


    [HttpPut("students/{id}")]
    public async Task<IActionResult> UpdateStudent(int id, [FromBody] StudentDto dto)
    {
        var student = await _studentRepository.GetByIdAsync(id);
        if (student == null) return NotFound();
        student.FullName = dto.FullName;
        student.PhoneNumber = dto.PhoneNumber;
        student.IsActive = dto.IsActive;
        student.HasPaid = dto.HasPaid;
        _studentRepository.Update(student);
        await _studentRepository.SaveChangeAsync();
        return Ok(student);
    }

    [HttpDelete("students/{id}")]
    public async Task<IActionResult> DeleteStudent(int id)
    {
        var student = await _studentRepository.GetByIdAsync(id);
        if (student == null) return NotFound();
    _studentRepository.Delete (student);
        await _studentRepository.SaveChangeAsync();
        return Ok("Deleted successfully");
    }


    // ===================== Schedules =====================
    [HttpGet("schedules")] public async Task<IActionResult> GetAllSchedules()
    {
        var centerId = GetCurrentCenterIdAsync();
        var schedules = await _scheduleRepository.GetAllWithSpecAsync(new ScheduleByCenter(centerId));
        var students = await _studentRepository.GetAllWithSpecAsync(new StudentsByCenterSpec(centerId));

        var scheduleDtos = schedules.Select(s => new ScheduleDto
        {
            SubjectName = s.Subject?.Name ?? "",
            TeacherName = s.Teacher?.Name ?? "",
            RoomNmae = s.Room?.Name ?? "",
            StartTime = s.StartAt,
            EndTime = s.EndAt,
            Statue = s.Statue,
            StudentCount = students.Count(st => st.Subjects.Any(sub => sub.Id == s.SubjectId))
        }).ToList();

        var today = DateTime.Today;
        var startOfWeek = today.AddDays(-(int)today.DayOfWeek);
        var endOfWeek = startOfWeek.AddDays(6);

        // ========================= إحصائيات اليوم =========================
        var schedulesToday = scheduleDtos.Where(s => s.StartTime.Date == today).ToList();
        var totalSchedulesToday = schedulesToday.Count;

        // كل الغرف الموجودة
        var allRooms = scheduleDtos.Select(s => s.RoomNmae).Distinct().ToList();

        // حساب عدد الحصص لكل غرفة اليوم (ديناميكي)
        var maxSlotsPerRoomToday = allRooms
            .Select(room => schedulesToday.Count(s => s.RoomNmae == room))
            .DefaultIfEmpty(0)
            .Max();

        var totalPossibleSlotsToday = allRooms.Count * maxSlotsPerRoomToday;
        var occupiedSlotsToday = schedulesToday.Count;
        var occupancyRateToday = totalPossibleSlotsToday == 0 ? 0 : (double)occupiedSlotsToday / totalPossibleSlotsToday * 100;

        // ========================= إحصائيات الأسبوع =========================
        var schedulesWeek = scheduleDtos.Where(s => s.StartTime.Date >= startOfWeek && s.StartTime.Date <= endOfWeek).ToList();
        var totalSchedulesWeek = schedulesWeek.Count;

        double occupancyRateWeek = 0;
        if (schedulesWeek.Any())
        {
            var occupiedSlotsPerDay = schedulesWeek
                .GroupBy(s => s.StartTime.Date)
                .Select(g =>
                {
                    var roomsInDay = g.Select(s => s.RoomNmae).Distinct().ToList();
                    var maxSlotsPerRoom = roomsInDay
                        .Select(room => g.Count(s => s.RoomNmae == room))
                        .DefaultIfEmpty(0)
                        .Max();
                    return roomsInDay.Count * maxSlotsPerRoom;
                })
                .Sum();

            occupancyRateWeek = occupiedSlotsPerDay == 0 ? 0 : schedulesWeek.Count / occupiedSlotsPerDay * 100;
        }

        return Ok(new
        {
            TotalSchedulesToday = totalSchedulesToday,
            OccupancyRateToday = occupancyRateToday,
            TotalSchedulesWeek = totalSchedulesWeek,
            OccupancyRateWeek = occupancyRateWeek,
            Schedules = scheduleDtos
        });
    }

    [HttpGet("schedules/{id}")]
    public async Task<IActionResult> GetScheduleById(int id)
    {
        var centerId = GetCurrentCenterIdAsync();
        var schedule = await _scheduleRepository.GetByIdWithSpecAsync(new ScheduleByIdSpec(id, centerId));
        if (schedule == null) return NotFound();

        var students = await _studentRepository.GetAllWithSpecAsync(new StudentsByCenterSpec(centerId));

        var dto = new ScheduleDto
        {
            SubjectName = schedule.Subject?.Name ?? "",
            TeacherName = schedule.Teacher?.Name ?? "",
            RoomNmae = schedule.Room?.Name ?? "",
            StartTime = schedule.StartAt,
            EndTime = schedule.EndAt,
            Statue = schedule.Statue,
            StudentCount = students.Count(st => st.Subjects.Any(sub => sub.Id == schedule.SubjectId))
        };

        return Ok(dto);
    }

    [HttpPost("schedules")]
    public async Task<IActionResult> AddSchedule([FromBody] ScheduleCreateDto dto)
    {
        var centerId = GetCurrentCenterIdAsync();

        var teacher = (await _teacherRepository.GetAllWithSpecAsync(new TeachersByCenterSpec(centerId)))
                    .FirstOrDefault(t => t.Name == dto.TeacherName);
        var subject = (await _subjectRepository.GetAllWithSpecAsync(new SubjectsByCenterSpec(centerId)))
                        .FirstOrDefault(s => s.Name == dto.SubjectName);
        var room = (await _roomRepository.GetAllWithSpecAsync(new RoomByCenterSpec(centerId)))
                        .FirstOrDefault(r => r.Name == dto.RoomNmae);

        if (teacher == null || subject == null || room == null)
                       return BadRequest("Teacher, Subject or Room not found");

        var schedule = new Schedule
        {
            TeacherId = teacher.Id,
            SubjectId = subject.Id,
            RoomId = room.Id,
            StartAt = dto.StartAt,
            EndAt = dto.EndAt,
            Statue = dto.Statues,
            CenterId = centerId
        };

        await _scheduleRepository.AddAsync(schedule);
        await _scheduleRepository.SaveChangeAsync();

        return Ok(schedule);
    }

    [HttpPut("schedules/{id}")]
    public async Task<IActionResult> UpdateSchedule(int id, [FromBody] ScheduleCreateDto dto)
    {
        var schedule = await _scheduleRepository.GetByIdAsync(id);
        if (schedule == null) return NotFound();

        var room = (await _roomRepository.GetAllAsync()).FirstOrDefault(r => r.Name == dto.RoomNmae);
        if (room == null) return BadRequest("Room not found");

        schedule.StartAt = dto.StartAt;
        schedule.EndAt = dto.EndAt;
        schedule.RoomId = room.Id;
        schedule.Statue = dto.Statues;

        _scheduleRepository.Update(schedule);
        await _scheduleRepository.SaveChangeAsync();
        return Ok(schedule);
    }

    [HttpDelete("schedules/{id}")]
    public async Task<IActionResult> DeleteSchedule(int id)
    {
        var schedule = await _scheduleRepository.GetByIdAsync(id);
        if (schedule == null) return NotFound();
        _scheduleRepository.Delete(schedule);
        await _scheduleRepository.SaveChangeAsync();
        return Ok("Deleted successfully");
    }

    // إيجاد المدرس والمادة
    // ===================== Materials =====================
    [HttpGet("materials")]
    public async Task<IActionResult> GetAllMaterials()
    {
        var centerId = GetCurrentCenterIdAsync();
        var materials = await _materialRepository.GetAllWithSpecAsync(new MaterialsByCenterSpec(centerId));

        var materialDtos = materials.Select(m => new LessonMaterialDto
        {
            
            Title = m.Title,
            SubjectName = m.Subject?.Name ?? "",
            TeacherName = m.Teacher?.Name ?? "",
            FilePath = m.FilePath,
            FileSize = m.FilePath != null && System.IO.File.Exists(m.FilePath)
                        ? new FileInfo(m.FilePath).Length
                        : 0,
            UploadDate = m.UploadedAt,
            DownloadCount = m.DownloadCount
        }).ToList();

        // إحصائيات
        var totalMaterials = materialDtos.Count;
        var totalDownloads = materialDtos.Sum(m => m.DownloadCount);
        var mostPopular = materialDtos.OrderByDescending(m => m.DownloadCount).FirstOrDefault();
        var newestMaterial = materialDtos.OrderByDescending(m => m.UploadDate).FirstOrDefault();

        return Ok(new
        {
            TotalMaterials = totalMaterials,
            TotalDownloads = totalDownloads,
            MostPopular = mostPopular,
            NewestMaterial = newestMaterial,
            Materials = materialDtos
        });
    }

    [HttpGet("materials/{id}")]
    public async Task<IActionResult> GetMaterialById(int id)
    {
        var centerId = GetCurrentCenterIdAsync();
        var material = await _materialRepository.GetByIdWithSpecAsync(new MaterialByIdSpec(centerId, id));
        if (material == null) return NotFound();

        var dto = new LessonMaterialDto
        {
            
            Title = material.Title,
            SubjectName = material.Subject?.Name ?? "",
            TeacherName = material.Teacher?.Name ?? "",
            FilePath = material.FilePath,
            FileSize = material.FilePath != null && System.IO.File.Exists(material.FilePath)
                        ? new FileInfo(material.FilePath).Length
                        : 0,
            UploadDate = material.UploadedAt,
            DownloadCount = material.DownloadCount
        };

        return Ok(dto);
    }

    [HttpPost("materials/upload")]
    public async Task<IActionResult> UploadMaterial([FromForm] LessonMaterialCreateDto dto)
    {
        if (dto.File == null || dto.File.Length == 0)
            return BadRequest("No file uploaded");

        // السماح بالامتدادات (PDF, Word, PowerPoint, Video)
        var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".ppt", ".pptx", ".mp4", ".avi", ".mov", ".mkv" };
        var extension = Path.GetExtension(dto.File.FileName).ToLower();

        if (!allowedExtensions.Contains(extension))
            return BadRequest("Only PDF, Word, PowerPoint, and Video files are allowed");

        var fileType = extension switch
        {
            ".pdf" => "PDF",
            ".doc" or ".docx" => "Word",
            ".ppt" or ".pptx" => "PowerPoint",
            ".mp4" or ".avi" or ".mov" or ".mkv" => "Video",
            _ => "Other"
        };

        var fileName = $"{Guid.NewGuid()}{extension}";
        var folderPath = Path.Combine("Uploads", "Materials");
        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);

        var filePath = Path.Combine(folderPath, fileName);
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await dto.File.CopyToAsync(stream);
        }

        var centerId = GetCurrentCenterIdAsync();

        // البحث عن المدرس والمادة بالاسم
        var teacher = (await _teacherRepository.GetAllWithSpecAsync(new TeachersByCenterSpec(centerId)))
                        .FirstOrDefault(t => t.Name == dto.TeacherName);
        if (teacher == null) return BadRequest("Teacher not found");

        var subject = (await _subjectRepository.GetAllWithSpecAsync(new SubjectsByCenterSpec(centerId)))
                        .FirstOrDefault(s => s.Name == dto.SubjectName);
        if (subject == null) return BadRequest("Subject not found");

        var material = new LessonMaterial
        {
            Title = dto.Title,
            SubjectId = subject.Id,
            TeacherId = teacher.Id,
            Description = dto.Description,
            FilePath = filePath,
            CenterId = centerId,
            UploadedAt = DateTime.Now,
            DownloadCount = 0,
            FileType = fileType   // جديد (لتخزين نوع الملف PDF, Video, PowerPoint ...)
        };

        await _materialRepository.AddAsync(material);
        await _materialRepository.SaveChangeAsync();

        return Ok(new
        {
            Message = "Material uploaded successfully",
            Material = new
            {
                material.Id,
                material.Title,
                material.Description,
                material.FileType,
                material.FilePath,
                material.UploadedAt
            }
        });
    }

    
    [HttpPut("materials/{id}")]
    public async Task<IActionResult> UpdateMaterial(int id, [FromForm] LessonMaterialCreateDto dto)
    {
        var material = await _materialRepository.GetByIdAsync(id);
        if (material == null) return NotFound("Material not found");

        // البحث عن المدرس والمادة
        var centerId = GetCurrentCenterIdAsync();
        var teacher = (await _teacherRepository.GetAllWithSpecAsync(new TeachersByCenterSpec(centerId)))
                        .FirstOrDefault(t => t.Name == dto.TeacherName);
        if (teacher == null) return BadRequest("Teacher not found");

        var subject = (await _subjectRepository.GetAllWithSpecAsync(new SubjectsByCenterSpec(centerId)))
                        .FirstOrDefault(s => s.Name == dto.SubjectName);
        if (subject == null) return BadRequest("Subject not found");

        // تحديث القيم الأساسية
        material.Title = dto.Title;
        material.Description = dto.Description;
        material.TeacherId = teacher.Id;
        material.SubjectId = subject.Id;
        material.UploadedAt = DateTime.Now;

        // إذا تم رفع ملف جديد بدل القديم
        if (dto.File != null && dto.File.Length > 0)
        {
            // حذف الملف القديم
            if (System.IO.File.Exists(material.FilePath))
                System.IO.File.Delete(material.FilePath);

            var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".ppt", ".pptx", ".mp4", ".avi", ".mov", ".mkv" };
            var extension = Path.GetExtension(dto.File.FileName).ToLower();
            if (!allowedExtensions.Contains(extension))
                return BadRequest("Invalid file type");

            var fileType = extension switch
            {
                ".pdf" => "PDF",
                ".doc" or ".docx" => "Word",
                ".ppt" or ".pptx" => "PowerPoint",
                ".mp4" or ".avi" or ".mov" or ".mkv" => "Video",
                _ => "Other"
            };

            var fileName = $"{Guid.NewGuid()}{extension}";
            var folderPath = Path.Combine("Uploads", "Materials");
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            var filePath = Path.Combine(folderPath, fileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await dto.File.CopyToAsync(stream);
            }

            material.FilePath = filePath;
            material.FileType = fileType;
        }

        _materialRepository.Update(material);
        await _materialRepository.SaveChangeAsync();

        return Ok(new
        {
            Message = "Material updated successfully",
            Material = new
            {
                material.Id,
                material.Title,
                material.Description,
                material.FileType,
                material.FilePath,
                material.UploadedAt
            }
        });
    }
    [HttpDelete("materials/{id}")]
    public async Task<IActionResult> DeleteMaterial(int id)
    {
        var material = await _materialRepository.GetByIdAsync(id);
        if (material == null) return NotFound("Material not found");

        // حذف الملف من السيرفر
        if (System.IO.File.Exists(material.FilePath))
            System.IO.File.Delete(material.FilePath);

         _materialRepository.Delete(material);
        await _materialRepository.SaveChangeAsync();

        return Ok(new { Message = "Material deleted successfully" });
    }

    // ===================== Messages =====================

    //[HttpGet("messages")]public async Task<IActionResult> GetAllMessages()
    //   {
    //       var centerId = await GetCurrentCenterIdAsync();
    //       var spec = new MessageByCenterSpec(centerId);

    //       var messages = await _messageRepository.GetAllWithSpecAsync(spec);

    //       var messageDtos = messages.Select(m => new MessageDto
    //       {

    //           Title = m.Title,
    //           Type = m.MessageType.ToString(),
    //           Template = m.Template.ToString(),
    //           Body = m.Body,
    //           TeacherName = m.Teacher.Name,
    //           StudentName = m.Student.Name,
    //           SentAt = m.SentDate,
    //           IsSent = m.IsSent
    //       }).ToList();

    //       var totalMessages = messageDtos.Count;
    //       var messagesThisWeek = messageDtos.Count(m =>
    //           m.SentAt.Date >= DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek) &&
    //           m.SentAt.Date <= DateTime.Today.AddDays(6 - (int)DateTime.Today.DayOfWeek));

    //       var activeRecipients = messageDtos.Select(m => m.StudentName)
    //                                         .Distinct()
    //                                         .Count();

    //       return Ok(new
    //       {
    //           TotalMessages = totalMessages,
    //           MessagesThisWeek = messagesThisWeek,
    //           ActiveRecipients = activeRecipients,
    //           Messages = messageDtos
    //       });
    //   }


    // ===================== Messages =====================
    [HttpPost("students/message")]
    public async Task<IActionResult> SendMessage([FromBody] CreateMessageDto dto)
    {
        var centerId = GetCurrentCenterIdAsync();
        var teacherIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

        // تحقق إذا كان المعرف صحيح
        if (!int.TryParse(teacherIdString, out int teacherId))
            return BadRequest("Invalid teacher ID in token.");

        var message = new Message
        {
            Title = dto.Title,
            MessageType = dto.MessageType,
            Template = dto.Template,
            Body = dto.Body,
            TeacherId = teacherId,
            IsSent = true,
            SentDate = DateTime.UtcNow,
            Students = new HashSet<Student>() // تأكد من تهيئة الـ collection
        };

        if (dto.StudentIds != null && dto.StudentIds.Any())
        {
            // جلب الطلاب الذين ينتمون لنفس السنتر
            var students = await _studentRepository.GetAllWithSpecAsync(
                new StudentsByCenterSpec(centerId)
            );

            foreach (var student in students.Where(s => dto.StudentIds.Contains(s.Id)))
            {
                message.Students.Add(student);
            }
        }

        await _messageRepository.AddAsync(message);
        await _messageRepository.SaveChangeAsync();

        return Ok(new MessageDto
        {
            Title = message.Title,
            Type = message.MessageType.ToString(),
            Template = message.Template.ToString(),
            Body = message.Body,
            TeacherName = message.Teacher?.Name,
            RecipientCount = message.Students.Count,
            RecipientNames = message.Students.Select(s => s.FullName).ToList(),
            SentAt = message.SentDate,
            IsSent = message.IsSent
        });
    }

    [HttpGet("messages")]
    public async Task<IActionResult> GetAllMessages()
    {
        var centerId = GetCurrentCenterIdAsync();
        var spec = new MessageByCenterSpec(centerId);
        var messages = await _messageRepository.GetAllWithSpecAsync(spec);

        var dtoList = messages.Select(m => new MessageDto
        {
            
            Title = m.Title,
            Type = m.MessageType.ToString(),
            Template = m.Template.ToString(),
            Body = m.Body,
            TeacherName = m.Teacher?.Name,
            RecipientCount = m.Students.Count,
            RecipientNames = m.Students.Select(s => s.FullName).ToList(),
            SentAt = m.SentDate,
            IsSent = m.IsSent
        }).ToList();

        return Ok(dtoList);
    }

    [HttpGet("messages/{id}")]
    public async Task<IActionResult> GetMessageById(int id)
    {
        var centerId = GetCurrentCenterIdAsync();
        var spec = new MessageByCenterSpec(centerId, id);
        var message = await _messageRepository.GetByIdWithSpecAsync(spec);
        if (message == null) return NotFound();

        var dto = new MessageDto
        {
            
            Title = message.Title,
            Type = message.MessageType.ToString(),
            Template = message.Template.ToString(),
            Body = message.Body,
            TeacherName = message.Teacher?.Name,
            RecipientCount = message.Students.Count,
            RecipientNames = message.Students.Select(s => s.FullName).ToList(),
            SentAt = message.SentDate,
            IsSent = message.IsSent
        };

        return Ok(dto);
    }

    [HttpPut("messages/{id}")]
    public async Task<IActionResult> UpdateMessage(int id, [FromBody] CreateMessageDto dto)
    {
        var centerId = GetCurrentCenterIdAsync();
        var message = await _messageRepository.GetByIdWithSpecAsync(new MessageByCenterSpec(centerId, id));
        if (message == null) return NotFound();

        message.Title = dto.Title;
        message.MessageType = dto.MessageType;
        message.Template = dto.Template;
        message.Body = dto.Body;

        message.Students.Clear();
        if (dto.StudentIds != null && dto.StudentIds.Any())
        {
            var students = await _studentRepository.GetAllWithSpecAsync(
                new StudentsByCenterSpec(centerId)
            );
            foreach (var student in students.Where(s => dto.StudentIds.Contains(s.Id)))
            {
                message.Students.Add(student);
            }
        }

        _messageRepository.Update(message);
        await _messageRepository.SaveChangeAsync();

        return Ok("Updated successfully");
    }
    [HttpDelete("messages/{id}")]
    public async Task<IActionResult> DeleteMessage(int id)
    {
        var centerId = GetCurrentCenterIdAsync();
        var message = await _messageRepository.GetByIdWithSpecAsync(new  MessageByCenterSpec(centerId, id));
        if (message == null) return NotFound();

        _messageRepository.Delete(message);
        await _messageRepository.SaveChangeAsync();

        return Ok("Deleted successfully");
    }

    //// ===================== Payments =====================
    //[HttpGet("students/{id}/payments")]
    //public async Task<IActionResult> GetStudentPayments(int id)
    //{
    //    var centerId = await GetCurrentCenterIdAsync();
    //    var spec = new PaymentsByStudentSpec(centerId, id);
    //    var payments = await _paymentRepository.ListBySpecAsync(spec);
    //    return Ok(payments);
    //}
}

 

