using BAExamApp.Dtos.Talents;

namespace BAExamApp.Business.Services;
public class TrainerTalentService : ITrainerTalentService
{
    private readonly ITalentRepository _talentRepository;
    private readonly IMapper _mapper;

    public TrainerTalentService(ITalentRepository talentRepository, IMapper mapper)
    {
        _talentRepository = talentRepository;
        _mapper = mapper;
    }

    
    public async Task<IDataResult<List<TalentListDto>>> GetTrainersTalents(Guid trainerId)
    {
        var talents = await _talentRepository.GetAllAsync(x => x.TrainerTalents.Any(x => x.TrainerId == trainerId));

        var mappedTalentList = _mapper.Map<List<TalentListDto>>(talents);

        return new SuccessDataResult<List<TalentListDto>>(mappedTalentList, Messages.ListedSuccess);
    }
}
