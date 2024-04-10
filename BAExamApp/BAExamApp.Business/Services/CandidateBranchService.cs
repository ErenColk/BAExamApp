
using BAExamApp.Dtos.CandidateBranches;

namespace BAExamApp.Business.Services;
public class CandidateBranchService : ICandidateBranchService
{
    private readonly ICandidateBranchRepository _candidateBranchRepository;
    private readonly IMapper _mapper;

    public CandidateBranchService(ICandidateBranchRepository candidateBranchRepository, IMapper mapper)
    {
        _candidateBranchRepository = candidateBranchRepository;
        _mapper = mapper;
    }

    /// <summary>
    /// Aday bölümünde şube oluşturur.
    /// </summary>
    /// <param name="candidateBranchCreateDto">CandidateBranchCreateDto tipinde paremetre alır.</param>
    /// <returns>Eklemeye çalıştığınız isimde bir şube var ise error result döner. Hata yok ise success result döner.</returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task<IDataResult<CandidateBranchDto>> CreateBranchAsync(CandidateBranchCreateDto candidateBranchCreateDto)
    {
        bool hasBranch = await _candidateBranchRepository.AnyAsync(c => c.Name.ToLower() == candidateBranchCreateDto.Name.ToLower());

        if (hasBranch)
        {
            return new ErrorDataResult<CandidateBranchDto>(Messages.AddFailAlreadyExists);
        }

        var branch = _mapper.Map<CandidateBranch>(candidateBranchCreateDto);
        await _candidateBranchRepository.AddAsync(branch);
        await _candidateBranchRepository.SaveChangesAsync();

        return new SuccessDataResult<CandidateBranchDto>(_mapper.Map<CandidateBranchDto>(branch), Messages.AddSuccess);
    }

    /// <summary>
    /// Tüm şubeleri getirir.
    /// </summary>
    /// <returns>CandidateBranchListDto list olarak döner.</returns>
    public async Task<IDataResult<List<CandidateBranchListDto>>> GetAllAsync()
    {
        var branches = await _candidateBranchRepository.GetAllAsync();

        if (branches == null )
        {
            return new ErrorDataResult<List<CandidateBranchListDto>>(Messages.BranchNotFound);
        }

        var branchListDtos = _mapper.Map<List<CandidateBranchListDto>>(branches);

        return new SuccessDataResult<List<CandidateBranchListDto>>(branchListDtos, Messages.ListedSuccess);
    }

    /// <summary>
    /// Şube Güncelleme işlemi yapar
    /// </summary>
    /// <param name="branchUpdateDto">CandidateBranchUpdateDto tipinde değer alır</param>
    /// <returns>Güncelleme işlemi yapılan isimde şube var ise error result döner  Hata yok ise success result döner.</returns>
    public async Task<IDataResult<CandidateBranchDto>> UpdateAsync(CandidateBranchUpdateDto branchUpdateDto)
    {
        bool hasBranch = await _candidateBranchRepository.AnyAsync(b => b.Name.ToLower() == branchUpdateDto.Name.ToLower());

        if(hasBranch)
        {
            return new ErrorDataResult<CandidateBranchDto>(Messages.AddFailAlreadyExists);
        }

        var branch = await _candidateBranchRepository.GetByIdAsync(Guid.Parse(branchUpdateDto.Id));
        if(branch == null)
        {
            return new ErrorDataResult<CandidateBranchDto>(Messages.UpdateFail);
        }
            
        var updatedBranch = _mapper.Map(branchUpdateDto, branch);
        await _candidateBranchRepository.UpdateAsync(updatedBranch);
        await _candidateBranchRepository.SaveChangesAsync();

        return new SuccessDataResult<CandidateBranchDto>(_mapper.Map<CandidateBranchDto>(branch), Messages.UpdateSuccess);
    }

    /// <summary>
    /// Şube silme işlemi yapar
    /// </summary>
    /// <param name="id">Şube id gelmelidir.</param>
    /// <returns>Silme işleminde hata ile karşılaşırsa error result döner. Hata yok ise success result döner.</returns>
    public async Task<IDataResult<CandidateBranchDto>> DeleteAsync(Guid id)
    {
        var branch = await _candidateBranchRepository.GetByIdAsync(id);
        if(branch == null)
        {
            return new ErrorDataResult<CandidateBranchDto>(Messages.DeleteFail);
        }

        await _candidateBranchRepository.DeleteAsync(branch);
        await _candidateBranchRepository.SaveChangesAsync();
        return new SuccessDataResult<CandidateBranchDto>(_mapper.Map<CandidateBranchDto>(branch), Messages.DeleteSuccess);
    }
}
