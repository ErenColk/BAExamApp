using BAExamApp.Dtos.Trainers;

namespace BAExamApp.Business.Profiles;

public class TrainerProfile : Profile
{
    public TrainerProfile()
    {
        CreateMap<Trainer, TrainerDto>()
            .ForMember(dest => dest.CityName, opt => opt.MapFrom(src => src.City.Name))
            .ForMember(dest => dest.ProductIds, opt => opt.MapFrom(src => src.TrainerProducts.Select(x => x.ProductId)))
            .ForMember(dest => dest.TrainerTalentIds, opt => opt.MapFrom(src => src.TrainerTalents.Select(x => x.TalentId)));
        CreateMap<Trainer, TrainerListDto>();
        CreateMap<Trainer, TrainerDetailsDto>()
            .ForMember(dest => dest.CityName, opt => opt.MapFrom(src => src.City.Name));
        CreateMap<Trainer, TrainerDetailsForTrainerDto>()
            .ForMember(dest => dest.CityName, opt => opt.MapFrom(src => src.City.Name));
        CreateMap<TrainerDto, Trainer>();
        CreateMap<TrainerCreateDto, Trainer>();
        CreateMap<TrainerUpdateDto, Trainer>();
        CreateMap<TrainerListDto, TrainerDto>();
    }
}
