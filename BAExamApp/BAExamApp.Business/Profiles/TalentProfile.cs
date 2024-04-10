using BAExamApp.Dtos.Talents;

namespace BAExamApp.Business.Profiles;
public class TalentProfile : Profile
{
    public TalentProfile()
    {
        CreateMap<Talent, TalentListDto>();
        CreateMap<Talent, TalentDto>();
        CreateMap<TalentCreateDto, Talent>()
            .ForMember(dest => dest.Name,
            config => config.MapFrom(src => src.Name.Trim()));

    }
}
