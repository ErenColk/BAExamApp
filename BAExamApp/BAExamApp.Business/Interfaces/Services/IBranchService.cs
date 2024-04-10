using BAExamApp.Dtos.Branches;

namespace BAExamApp.Business.Interfaces.Services;

public interface IBranchService
{
    Task<IDataResult<BranchDto>> GetByIdAsync(Guid id);
    Task<IDataResult<List<BranchListDto>>> GetAllAsync();
    /// <summary>
    /// Gelen Id'ye göre şube detaylarını döndürür.
    /// Şube detayları şunları içerir; Name.
    /// </summary>
    /// <param name="id">İlgili şube ve detaylarını bulmak için şubenin Id'si gereklidir.</param>
    /// <returns>BranchDetailsDto</returns>
    Task<IDataResult<BranchDetailsDto>> GetDetailsByIdAsync(Guid id);
    Task<IDataResult<BranchDto>> AddAsync(BranchCreateDto branchCreateDto);
    Task<IDataResult<BranchDto>> UpdateAsync(BranchUpdateDto branchUpdateDto);
    Task<IResult> DeleteAsync(Guid id);
}
