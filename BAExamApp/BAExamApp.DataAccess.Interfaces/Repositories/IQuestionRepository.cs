using System.Linq.Expressions;

namespace BAExamApp.DataAccess.Interfaces.Repositories;

public interface IQuestionRepository : IAsyncRepository, IAsyncInsertableRepository<Question>, IAsyncQueryableRepository<Question>, IAsyncFindableRepository<Question>, IAsyncUpdateableRepository<Question>, IAsyncDeleteableRepository<Question>
{
    Task<IEnumerable<Question>> GetAllWithIncludeAsync(Expression<Func<Question, bool>> expression, Expression<Func<Question, object>> include, bool tracking = true);
}
