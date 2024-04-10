using Microsoft.EntityFrameworkCore;

namespace BAExamApp.DataAccess.EFCore.Repositories;

public class StudentRepository : EFBaseRepository<Student>, IStudentRepository
{
    public StudentRepository(BAExamAppDbContext context) : base(context) { }

    public Task<Student?> GetByIdentityIdAsync(string identityId)
    {
        return _table.FirstOrDefaultAsync(x => x.IdentityId == identityId);
    }   
}