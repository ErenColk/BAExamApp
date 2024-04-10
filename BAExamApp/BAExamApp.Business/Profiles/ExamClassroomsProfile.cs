using BAExamApp.Dtos.ExamClassrooms;

namespace BAExamApp.Business.Profiles
{
    public class ExamClassroomsProfile : Profile
    {
        public ExamClassroomsProfile()
        {
            CreateMap<ExamClassrooms, ExamClassroomsDto>();
            CreateMap<ExamClassrooms, ExamClassroomsListDto>()
                .ForMember(dest => dest.ClassroomName, opt => opt.MapFrom(src => src.Classroom.Name))
                .ForMember(dest => dest.ExamName, opt => opt.MapFrom(src => src.Exam.Name));
            CreateMap<Guid, ExamClassroomsCreateDto>();
            CreateMap<ExamClassroomsCreateDto, ExamClassrooms>();

        }
    }
}
