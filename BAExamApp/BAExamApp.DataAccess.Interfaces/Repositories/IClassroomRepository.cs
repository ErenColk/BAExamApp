namespace BAExamApp.DataAccess.Interfaces.Repositories;

public interface IClassroomRepository : IAsyncRepository, IAsyncQueryableRepository<Classroom>, IAsyncFindableRepository<Classroom>, IAsyncUpdateableRepository<Classroom>, IAsyncInsertableRepository<Classroom>, IAsyncDeleteableRepository<Classroom>
{
}
