using BAExamApp.Core.Entities.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace BAExamApp.DataAccess.EFCore.Repositories;

public class QuestionRepository : EFBaseRepository<Question>, IQuestionRepository
{
    public QuestionRepository(BAExamAppDbContext context) : base(context) {}
    public async Task<IEnumerable<Question>> GetAllWithIncludeAsync(Expression<Func<Question, bool>> expression, Expression<Func<Question, object>> include, bool tracking = true)
    {
        return await GetAllActives(tracking).Include(include).Where(expression).ToListAsync();
    }
}
