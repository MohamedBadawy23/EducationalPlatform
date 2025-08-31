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
               .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.Name)).ReverseMap();
            CreateMap<AppUser, UserDto>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
                .ReverseMap();
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