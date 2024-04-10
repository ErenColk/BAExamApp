using BAExamApp.Dtos.Admins;

namespace BAExamApp.Business.Profiles;

public class AdminProfile : Profile
{
    public AdminProfile()
    {
        CreateMap<Admin, AdminDto>();
        CreateMap<Admin, AdminListDto>();
        CreateMap<AdminCreateDto, Admin>();
        CreateMap<AdminUpdateDto, Admin>();
        CreateMap<Admin, AdminDetailsDto>().ForMember(dest => dest.CityName,config => config.MapFrom(src => src.City.Name));

    }
}