using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BAExamApp.DataAccess.EFCore.Repositories;
public class CandidateQuestionRepository : EFBaseRepository<CandidateQuestion>, ICandidateQuestionRepository
{
    public CandidateQuestionRepository(BAExamAppDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<CandidateQuestion>> GetAllWithIncludeAsync(Expression<Func<CandidateQuestion, bool>> expression, Expression<Func<CandidateQuestion, object>> include, bool tracking = true)
    {
        return await GetAllActives(tracking).Include(include).Where(expression).ToListAsync();
    }
}
