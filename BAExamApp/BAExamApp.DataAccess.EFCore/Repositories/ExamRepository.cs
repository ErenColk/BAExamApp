namespace BAExamApp.DataAccess.EFCore.Repositories;

public class ExamRepository : EFBaseRepository<Exam>, IExamRepository
{
    public ExamRepository(BAExamAppDbContext context) : base(context)
    {
    }
}