using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BAExamApp.DataAccess.Interfaces.Repositories;
public interface ICandidateQuestionRepository : IAsyncRepository, IAsyncInsertableRepository<CandidateQuestion>, IAsyncQueryableRepository<CandidateQuestion>, IAsyncFindableRepository<CandidateQuestion>, IAsyncUpdateableRepository<CandidateQuestion>, IAsyncDeleteableRepository<CandidateQuestion>
{
    Task<IEnumerable<CandidateQuestion>> GetAllWithIncludeAsync(Expression<Func<CandidateQuestion, bool>> expression, Expression<Func<CandidateQuestion, object>> include, bool tracking = true);
}
