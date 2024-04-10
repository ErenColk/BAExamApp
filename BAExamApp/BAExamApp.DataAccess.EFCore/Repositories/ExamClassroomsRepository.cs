namespace BAExamApp.DataAccess.EFCore.Repositories;

public class ExamClassroomsRepository : EFBaseRepository<ExamClassrooms>, IExamClassroomsRepository
{
    public ExamClassroomsRepository(BAExamAppDbContext context) : base(context) { }
}

