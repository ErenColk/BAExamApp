using Microsoft.EntityFrameworkCore;

namespace BAExamApp.DataAccess.EFCore.Repositories;

public class TrainerRepository : EFBaseRepository<Trainer>, ITrainerRepository
{
    public TrainerRepository(BAExamAppDbContext context) : base(context) { }

    public Task<Trainer?> GetByIdentityIdAsync(string identityId)
    {
        return _table.FirstOrDefaultAsync(x => x.IdentityId == identityId);
    }

    public Task<List<Trainer>> GetAllTrainers()
    {
        return _table.ToListAsync();
    }
}