namespace BAExamApp.DataAccess.Interfaces.Repositories;

public interface ICityRepository : IAsyncRepository, IAsyncQueryableRepository<City>, IAsyncFindableRepository<City>, IAsyncInsertableRepository<City>, IAsyncDeleteableRepository<City>, IAsyncUpdateableRepository<City>
{
}
