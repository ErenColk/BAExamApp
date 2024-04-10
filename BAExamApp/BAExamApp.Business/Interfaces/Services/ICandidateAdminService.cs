
using BAExamApp.Dtos.Admins;
using BAExamApp.Dtos.CandidateAdmins;

namespace BAExamApp.Business.Interfaces.Services;
public interface ICandidateAdminService
{
    /// <summary>
    /// Id'ye göre aday admini çağırma işlemi
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<IDataResult<CandidateAdminDto>> GetByIdAsync(Guid id);
    /// <summary>
    /// Aday admin'inin identityId'sine göre çağırılma işlemi. 
    /// </summary>
    /// <param name="identityId"></param>
    /// <returns></returns>
    Task<IDataResult<CandidateAdminDto>> GetByIdentityIdAsync(string identityId);
    /// <summary>
    /// Aday admini olarak tüm aday adminlerini çağırma işlemi
    /// </summary>
    /// <returns></returns>
    Task<IDataResult<List<CandidateAdminListDto>>> GetAllAsync();
    /// <summary>
    /// Aday admini olarak aday admini ekleme işlemi.
    /// </summary>
    /// <param name="candidateAdminCreateDto"></param>
    /// <returns></returns>
    Task<IDataResult<CandidateAdminDto>> AddAsync(CandidateAdminCreateDto candidateAdminCreateDto);
    /// <summary>
    /// Aday yöneticisi olarak, aday yöneticisi güncelleme işlemi.
    /// </summary>
    /// <param name="candidateAdminUpdateDto"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    Task<IDataResult<CandidateAdminDto>> UpdateAsync(CandidateAdminUpdateDto candidateAdminUpdateDto);

    /// <summary>
    /// Aday admini olarak aday admini silme işlemi.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<IResult> DeleteAsync(Guid id);
    /// <summary>
    /// Aday admin'inin id sine göre çağırılma işlemi.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<IDataResult<CandidateAdminDetailsDto>> GetDetailsByIdAsync(Guid id);
}
