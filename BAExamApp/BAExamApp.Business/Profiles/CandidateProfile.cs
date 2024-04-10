using BAExamApp.Dtos.Candidates;

namespace BAExamApp.Business.Profiles;
public class CandidateProfile : Profile
{
    public CandidateProfile()
    {
        CreateMap<Candidate, CandidateListDto>();
        CreateMap<CandidateCreateDto, Candidate>();
        CreateMap<CandidateUpdateDto, Candidate>();
        CreateMap<Candidate, CandidateDto>();
        CreateMap<Candidate, CandidateDetailsDto>().ForMember(dest => dest.GroupName, config => config.MapFrom(src => src.CandidateGroup.Name));
    }
}
