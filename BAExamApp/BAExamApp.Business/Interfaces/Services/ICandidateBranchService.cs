
using BAExamApp.Dtos.CandidateBranches;

namespace BAExamApp.Business.Interfaces.Services;
public interface ICandidateBranchService
{
    /// <summary>
    /// Aday bölümünde şube oluşturur.
    /// </summary>
    /// <param name="branchCreateDto">CandidateBranchCreateDto tipinde paremetre alır.</param>
    /// <returns>Eklemeye çalıştığınız isimde bir şube var ise error result döner. Hata yok ise success result döner.</returns>
    /// <exception cref="NotImplementedException"></exception>
    Task<IDataResult<CandidateBranchDto>> CreateBranchAsync(CandidateBranchCreateDto branchCreateDto);

    /// <summary>
    /// Tüm şubeleri getirir.
    /// </summary>
    /// <returns>CandidateBranchListDto list olarak döner.</returns>
    Task<IDataResult<List<CandidateBranchListDto>>> GetAllAsync();

    /// <summary>
    /// Şube Güncelleme işlemi yapar
    /// </summary>
    /// <param name="branchUpdateDto">CandidateBranchUpdateDto tipinde değer alır</param>
    /// <returns>Güncelleme işlemi yapılan isimde şube var ise error result döner  Hata yok ise success result döner.</returns>
    Task<IDataResult<CandidateBranchDto>> UpdateAsync(CandidateBranchUpdateDto branchUpdateDto);

    /// <summary>
    /// Şube silme işlemi yapar
    /// </summary>
    /// <param name="id">Şube id gelmelidir.</param>
    /// <returns>Silme işleminde hata ile karşılaşırsa error result döner. Hata yok ise success result döner.</returns>
    Task<IDataResult<CandidateBranchDto>> DeleteAsync(Guid id);
}
