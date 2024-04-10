
namespace BAExamApp.DataAccess.Interfaces.Repositories;
public interface ICandidateAdminRepository : IAsyncRepository, IAsyncFindableRepository<CandidateAdmin>, IAsyncInsertableRepository<CandidateAdmin>, IAsyncDeleteableRepository<CandidateAdmin>, IAsyncUpdateableRepository<CandidateAdmin>, IAsyncTransactionRepository
{
    Task<CandidateAdmin?> GetByIdentityIdAsync(string identityId);
}
