using BAExamApp.Dtos.Talents;

namespace BAExamApp.Business.Services;
public class TalentService : ITalentService
{
    private readonly ITalentRepository _talentRepository;
    private readonly IMapper _mapper;
    private readonly ITrainerTalentRepository _trainerTalentRepository;

    public TalentService(ITalentRepository talentRepository, IMapper mapper,ITrainerTalentRepository trainerTalentRepository)
    {
        _talentRepository = talentRepository;
        _mapper = mapper;
        _trainerTalentRepository = trainerTalentRepository;
    }
      
    public async Task<IDataResult<TalentDto>> AddAsync(TalentCreateDto talentCreateDto)
    {
        var hasTalent = await _talentRepository.AnyAsync(talent => talent.Name.ToLower() == talentCreateDto.Name.Trim().ToLower());
        if (hasTalent)
        {
            return new ErrorDataResult<TalentDto>(Messages.AddFailAlreadyExists);
        }

        var talent = _mapper.Map<Talent>(talentCreateDto);
        try
        {
            await _talentRepository.AddAsync(talent);
        }
        catch (Exception)
        {
            return new ErrorDataResult<TalentDto>(Messages.DuplicateTalent);
        }
        await _talentRepository.SaveChangesAsync();

        var talentDto = _mapper.Map<TalentDto>(talent);

        return new SuccessDataResult<TalentDto>(talentDto, Messages.AddSuccess);
    }

    public async Task<IDataResult<List<TalentListDto>>> GetAllAsync()
    {
        var talents = await _talentRepository.GetAllAsync(false);
        var mappedTalents = _mapper.Map<List<TalentListDto>>(talents);
        return new SuccessDataResult<List<TalentListDto>>(mappedTalents, Messages.ListedSuccess);
    }
    
    public async Task<IDataResult<TalentDto>> GetByIdAsync(Guid id)
    {
        var talent = await _talentRepository.GetByIdAsync(id);
        if (talent is null)
        {
            return new ErrorDataResult<TalentDto>(Messages.TalentNotFound);
        }
        var talentDto = _mapper.Map<TalentDto>(talent);

        return new SuccessDataResult<TalentDto>(talentDto, Messages.FoundSuccess);
    }
    
    public async Task<IResult> DeleteAsync(Guid id)
    {
        var talentIsActive = await _trainerTalentRepository.GetAllAsync(x => x.TalentId == id);
        if (talentIsActive.Count()==0)
        {
            var talent = await _talentRepository.GetByIdAsync(id);

            await _talentRepository.DeleteAsync(talent);
            await _talentRepository.SaveChangesAsync();
            return new SuccessResult();
        }
        else
        {
            return new ErrorResult();
        }
    }

    
}
