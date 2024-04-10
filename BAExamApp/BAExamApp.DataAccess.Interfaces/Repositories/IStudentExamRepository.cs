namespace BAExamApp.DataAccess.Interfaces.Repositories;

public interface IStudentExamRepository : IRepository, IAsyncRepository, IAsyncQueryableRepository<StudentExam>, IAsyncFindableRepository<StudentExam>, IAsyncUpdateableRepository<StudentExam>, IAsyncInsertableRepository<StudentExam>, IAsyncDeleteableRepository<StudentExam>
{
}