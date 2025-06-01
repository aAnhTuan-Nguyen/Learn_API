using AutoMapper;
using TodoWeb.Application.Dtos.CourseDTO;
using TodoWeb.Application.Dtos.StudentDTO;
using TodoWeb.Domain.Entities;

namespace TodoWeb.Application.MapperProfiles
{
    public class TodoProfile : Profile
    {
        public TodoProfile()
        {
            // Map Course to CourseViewModel // để map thì dùng CreateMap
            // Nếu mà tên giống thì không sao nhưng mà khác thì ForMember
            CreateMap<Course, CourseViewModel>()
                .ForMember(dest => dest.CourseId, config => config.MapFrom(src => src.Id))
                .ForMember(dest => dest.CourseName, config => config.MapFrom(src => src.Name));
            // minfh dang phai map tung cai bay h anh muon chung dung mapper
            // Vif la post nen phai ngc lai
            // ReverseMap giups map ngc lai neu cai dau ko dc
            CreateMap<CourseCreateModel, Course>()
                .ForMember(dest => dest.Name, config => config.MapFrom(src => src.CourseName));
            
            CreateMap<CourseUpdateModel, Course>()
                .ForMember(dest => dest.Name, config =>
                {
                    config.PreCondition(src => !string.IsNullOrEmpty(src.CourseName));
                    config.MapFrom(src => src.CourseName);
                });
            CreateMap<Student, StudentViewModel>()
                //.ForMember(dest => dest.Id, config => config.Ignore())
                .ForMember(dest => dest.FullName,
                    config => config.MapFrom(src => src.FirstName + " " + src.LastName))
                .ForMember(dest => dest.SchoolName, config => config.MapFrom(src => src.School.Name));
            
            CreateMap<StudentCourse, CourseViewModel>()
                .ForMember(dest => dest.CourseName, config => config.MapFrom(src => src.Course.Name))
                .ForMember(dest => dest.StartDate, config => config.MapFrom(src => src.Course.StartDate));
            

            CreateMap<Student, StudentDetailViewModel>()
                .ForMember(dest => dest.FullName,
                    config => config.MapFrom(src => src.FirstName + " " + src.LastName))
                .ForMember(dest => dest.Courses,
                    config => config.MapFrom(src => src.StudentCourses));

        }
    }
}
