using Microsoft.EntityFrameworkCore;
namespace BAExamApp.DataAccess.EFCore.Repositories;
public class CandidateAdminRepository : EFBaseRepository<CandidateAdmin>, ICandidateAdminRepository
{
    public CandidateAdminRepository(BAExamAppDbContext context) : base(context) { }
    public Task<CandidateAdmin?> GetByIdentityIdAsync(string identityId)
    {
        return _table.FirstOrDefaultAsync(x => x.IdentityId == identityId);
    }
}
