using Api.PL.DTO;
using AutoMapper;
using Core.Entities;

namespace Api.PL.Helper
{
    public class TeacherProfile:Profile
    {
        public TeacherProfile()
        {
            // من AddTeacherDto إلى Teacher (عشان الحفظ في الداتا بيز)
            CreateMap<TeacherCreateDto, Teacher>()
                    .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true))  // نخليه نشط افتراضياً
                    .ForMember(dest => dest.Rating, opt => opt.MapFrom(src => 0.0))    // يبدأ بدون تقييم
                    .ForMember(dest => dest.CenterId, opt => opt.Ignore())             // يتحدد من الكونتكست مش من الفورم
                    .ForMember(dest => dest.Center, opt => opt.Ignore())
                    .ForMember(dest => dest.Subject, opt => opt.Ignore());

            // من Teacher إلى TeacherDto (عشان العرض)
            CreateMap<Teacher, TeacherDto>()
                    .ForMember(dest => dest.SubjectName, opt => opt.MapFrom(src => src.Subject.Name))
                    .ForMember(P=>P.StudentCount,opt=>opt.MapFrom(T=>T.Students.Count));

            CreateMap<TeacherCreateDto, Teacher>()
                   .ForMember(dest => dest.Subject, opt => opt.Ignore()).ForMember(P => P.SubjectId, opt => opt.Ignore());
        }

    }
}
