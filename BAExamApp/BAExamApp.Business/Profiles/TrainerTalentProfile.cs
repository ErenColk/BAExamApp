using BAExamApp.Dtos.Trainers;
using BAExamApp.Dtos.TrainerTalents;

namespace BAExamApp.Business.Profiles;
public class TrainerTalentProfile : Profile
{
    public TrainerTalentProfile()
    {
        CreateMap<TrainerListDto, Talent>();
        CreateMap<TrainerTalent, TrainerTalentListForTrainerDetailsDto>()
            .ForMember(dest=>dest.TalentName, opt=>opt.MapFrom(src=>src.Talent.Name));
    }
}
