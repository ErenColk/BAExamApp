using BAExamApp.Dtos.Branches;

namespace BAExamApp.Business.Services;

public class BranchService : IBranchService
{
    private readonly IBranchRepository _branchRepository;
    private readonly IMapper _mapper;

    public BranchService(IBranchRepository branchRepository, IMapper mapper)
    {
        _branchRepository = branchRepository;
        _mapper = mapper;
    }

    public async Task<IDataResult<BranchDto>> GetByIdAsync(Guid id)
    {
        var branch = await _branchRepository.GetByIdAsync(id);

        if (branch is null)
        {
            return new ErrorDataResult<BranchDto>(Messages.BranchNotFound);
        }

        return new SuccessDataResult<BranchDto>(_mapper.Map<BranchDto>(branch), Messages.FoundSuccess);
    }

    public async Task<IDataResult<List<BranchListDto>>> GetAllAsync()
    {
        var branches = await _branchRepository.GetAllAsync(false);

        return new SuccessDataResult<List<BranchListDto>>(_mapper.Map<List<BranchListDto>>(branches), Messages.ListedSuccess);
    }

    public async Task<IDataResult<BranchDetailsDto>> GetDetailsByIdAsync(Guid id)
    {
        var branch = await _branchRepository.GetByIdAsync(id);

        if (branch is null)
        {
            return new ErrorDataResult<BranchDetailsDto>(Messages.BranchNotFound);
        }

        return new SuccessDataResult<BranchDetailsDto>(_mapper.Map<BranchDetailsDto>(branch), Messages.FoundSuccess);

    }

    public async Task<IDataResult<BranchDto>> AddAsync(BranchCreateDto branchCreateDto)
    {
        var hasBranch = await _branchRepository.AnyAsync(branch => branch.Name.ToLower() == branchCreateDto.Name.ToLower());

        if (hasBranch)
        {
            return new ErrorDataResult<BranchDto>(Messages.AddFailAlreadyExists);
        }

        var branch = _mapper.Map<Branch>(branchCreateDto);

        await _branchRepository.AddAsync(branch);
        await _branchRepository.SaveChangesAsync();

        return new SuccessDataResult<BranchDto>(_mapper.Map<BranchDto>(branch), Messages.AddSuccess);
    }

    public async Task<IDataResult<BranchDto>> UpdateAsync(BranchUpdateDto branchUpdateDto)
    {
        var hasBranch = await _branchRepository.AnyAsync(branch => branch.Id != branchUpdateDto.Id && branch.Name.ToLower() == branchUpdateDto.Name.ToLower());

        if (hasBranch)
        {
            return new ErrorDataResult<BranchDto>(Messages.AddFailAlreadyExists);
        }

        var branch = await _branchRepository.GetByIdAsync(branchUpdateDto.Id);

        if (branch is null)
        {
            return new ErrorDataResult<BranchDto>(Messages.BranchNotFound);
        }

        var updatedBranch = _mapper.Map(branchUpdateDto, branch);

        await _branchRepository.UpdateAsync(updatedBranch);
        await _branchRepository.SaveChangesAsync();

        return new SuccessDataResult<BranchDto>(_mapper.Map<BranchDto>(updatedBranch), Messages.UpdateSuccess);
    }

    public async Task<IResult> DeleteAsync(Guid id)
    {
        var branch = await _branchRepository.GetByIdAsync(id);

        if (branch is null)
        {
            return new ErrorDataResult<BranchDto>(Messages.BranchNotFound);
        }

        await _branchRepository.DeleteAsync(branch);
        await _branchRepository.SaveChangesAsync();

        return new SuccessResult(Messages.DeleteSuccess);
    }
}
