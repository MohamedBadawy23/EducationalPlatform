using Api.PL.DTO;
using AutoMapper;
using Core.Entities;
using CoreProject.Enteties;
using EducationalPlatform.DTO;
// using Core.Enteties; // Commented out because it does not exist

namespace EducationalPlatform.Helper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // User mappings
            // Commented out mappings to missing DTOs
            CreateMap<RegisterDTO, AppUser>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.PhoneNumber))
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName));
            CreateMap<Student, UserDto>()
               .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName)).ReverseMap();
            CreateMap<AppUser, UserDto>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
                .ReverseMap();
            CreateMap<Center, CenterDto>().ForMember(P=>P.Name,P=>P.MapFrom(C=>C.CenterName)).ReverseMap();
            CreateMap<Center,UpdateCenterDto>().ReverseMap();
           // CreateMap<Teacher, TeacherDto>().ForMember(P=>P.SubjectsNames,O=>O.MapFrom(W=>W.Subject.Select(i=>i.Name).ToList())) .ReverseMap();
            CreateMap<TeacherCreateDto, Teacher>().ForMember(P => P.Subject, O => O.Ignore());
            CreateMap<Message, MessageDto>().ForMember(P => P.TeacherName, T => T.MapFrom(src => src.Teacher.Name)).
                ForMember(P => P.RecipientCount, m => m.MapFrom(o => o.Students.Count)).ForMember(P => P.Type, T => T.MapFrom(B => B.MessageType.ToString())).
                ForMember(P => P.Template, T => T.MapFrom(U => U.Template.ToString())).
                ForMember(P => P.RecipientNames, R => R.MapFrom(I => I.Students.Select(M => M.FullName))).ReverseMap();
            CreateMap<LessonMaterial,LessonMaterialDto>().ForMember(P => P.TeacherName, o => o.MapFrom(s => s.Teacher.Name))
                .ForMember(P=>P.SubjectName,o=>o.MapFrom(s=>s.Subject.Name))
                .ForMember(P=>P.Title,O=>O.MapFrom(T=>T.Title)).ReverseMap();
            CreateMap< LessonMaterialCreateDto,LessonMaterial>().ForMember(P => P.Subject, O => O.Ignore()).ReverseMap();
            CreateMap<Student,StudentDto>().ForMember(P=>P.FullName,M=>M.MapFrom(I=>I.FullName)).
                ForMember(P=>P.Subjects,O=>O.MapFrom(T=>T.Subjects.Select((M)=>M.Name).ToList())).ReverseMap();
            CreateMap<Student, StudentCreateDto>().ForMember(P => P.FullName, M => M.MapFrom(I =>I.FullName)).
               ForMember(P => P.SubjectNames, O => O.MapFrom(T => T.Subjects.Select((M) => M.Name).ToList())).ReverseMap();
            CreateMap<Teacher, TeacherCreateDto>().ReverseMap();
          //  CreateMap<Subject, SubjectDto>().ForMember(dest => dest.TeacherName, P => P.MapFrom(M => M.Teachers.Select(P=>P.Name))).ReverseMap();
           // CreateMap<CreateSubjectDto, Subject>().ForMember(P => P.Teachers, opt => opt.Ignore());
            CreateMap<Schedule,ScheduleDto>().ForMember(P=>P.SubjectName, opt => opt.MapFrom(dest=>dest.Subject.Name))
                .ForMember(P=>P.TeacherName,opt=>opt.MapFrom(dest=>dest.Teacher.Name)).ForMember(I=>I.Statue,opt=>opt.MapFrom(dest=>dest.Statue.ToString()))
                .ForMember(P=>P.StudentCount,opt=>opt.MapFrom(dest=>dest.Students.Count)).ForMember(P=>P.RoomNmae,O=>O.MapFrom(U=>U.Room.Name)).ReverseMap();
            CreateMap<ScheduleCreateDto, Schedule>().ForMember(P => P.Subject, opt => opt.Ignore())
                 .ReverseMap();
            CreateMap<CreateMessageDto, Message>().ForMember(P => P.Teacher, opt => opt.Ignore()).ForMember(P => P.Students, opt => opt.Ignore());
            CreateMap<OtpEntry, otpDto>().ForMember(P => P.Email, o => o.MapFrom(I => I.User.Email));
            CreateMap<AppUser, ChangePhoneDto>().ForMember(P => P.NewPhoneNumber, o => o.MapFrom(I => I.PhoneNumber));
            




            // CreateMap<ScheduleLessonDto,Lesson>().ReverseMap();
            // CreateMap<UploadMaterialDto,LessonMaterial>().ForMember(P=>P,opt=>opt.MapFrom(src=>src.Title)).ReverseMap();
            //CreateMap<SendMessageDto,Message>().ReverseMap();
            // CreateMap<Student, UserDto>()
            //     .ForMember(dest => dest.Password, opt => opt.Ignore())
            //     .ForMember(dest => dest.Token, opt => opt.Ignore())
            //     .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.ToString()));

            // Update DTOs mappings
            // CreateMap<UpdateNameDto, Student>()
            //     .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.NewName));

            // CreateMap<UpdateEmailDto, Student>()
            //     .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.NewEmail))
            //     .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.NewEmail));

            // Service Provider and Account Update mappings
            // CreateMap<UpdateServiceProviderAccountDto, Student>()
            //     .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            //     .ForMember(dest => dest.Bio, opt => opt.MapFrom(src => src.Bio));

            // CreateMap<UpdateAccountDto, Student>()
            //     .ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.DisplayName))
            //     .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
            //     .ForMember(dest => dest.Bio, opt => opt.MapFrom(src => src.Bio))
            //     .ForMember(dest => dest.ImgUrl, opt => opt.MapFrom(src => src.ImgUrl))
            //     .ForMember(dest => dest.HasPaid, opt => opt.MapFrom(src => src.HasPaid));

            // Center mappings
            // CreateMap<CreateCenterDto, Student>()
            //     .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.Name))
            //     .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
            //     .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.PhoneNumber))
            //     .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.PhoneNumber + "@center.com")); // Generate email from phone

            // Center entity mappings
            // CreateMap<CreateCenterDto, Center>()
            //     .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name));

            // CreateMap<Center, CenterDto>()
            //     .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()))
            //     .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name));

            // CreateMap<UpdateCenterDto, Center>()
            //     .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name));

            // Subject mappings
            // CreateMap<Subject, SubjectDto>()
            //     .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()))
            //     .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            //     .ForMember(dest => dest.TeacherId, opt => opt.MapFrom(src => src.TeacherId.ToString()));

            // CreateMap<CreateSubjectDto, Subject>()
        }
    }
}