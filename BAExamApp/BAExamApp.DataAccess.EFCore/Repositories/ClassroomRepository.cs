namespace BAExamApp.DataAccess.EFCore.Repositories;

public class ClassroomRepository : EFBaseRepository<Classroom>, IClassroomRepository
{
    public ClassroomRepository(BAExamAppDbContext context) : base(context)
    {
    }
}
