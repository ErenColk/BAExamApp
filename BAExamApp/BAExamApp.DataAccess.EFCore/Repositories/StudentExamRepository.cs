namespace BAExamApp.DataAccess.EFCore.Repositories;

public class StudentExamRepository : EFBaseRepository<StudentExam>, IStudentExamRepository
{
    public StudentExamRepository(BAExamAppDbContext context) : base(context) { }
}