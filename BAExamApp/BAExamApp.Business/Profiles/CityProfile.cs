using BAExamApp.Dtos.Branches;
using BAExamApp.Dtos.Cities;

namespace BAExamApp.Business.Profiles;

public class CityProfile : Profile
{
    public CityProfile()
    {
        CreateMap<City, CityDto>();
        CreateMap<City, CityListDto>();
        CreateMap<CityCreateDto, City>()
            .ForMember(dest => dest.Name,
            config => config.MapFrom(src => src.Name.Trim()));
        CreateMap<CityUpdateDto, City>()
            .ForMember(dest => dest.Name,
            config => config.MapFrom(src => src.Name.Trim()));
    }
}