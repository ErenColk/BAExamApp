using BAExamApp.Dtos.Students;

namespace BAExamApp.Business.Profiles;

public class StudentProfile : Profile
{
    public StudentProfile()
    {
        CreateMap<Student, StudentDto>();
        CreateMap<Student, StudentListDto>();
        CreateMap<Student, StudentDetailsDto>().ForMember(dest => dest.CityName, config => config.MapFrom(src => src.City.Name));
        CreateMap<Student, StudentDetailsForTrainerDto>()
            .ForMember(dest => dest.StudentExams, config => config.MapFrom(src => src.StudentExams.Where(x => x.IsFinished)))
            .ForMember(dest => dest.StudentName, config => config.MapFrom(src => src.FullName));
        CreateMap<StudentCreateDto, Student>();
        CreateMap<StudentUpdateDto, Student>();
    }
}
