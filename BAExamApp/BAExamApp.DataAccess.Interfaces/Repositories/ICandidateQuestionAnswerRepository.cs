using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAExamApp.DataAccess.Interfaces.Repositories;
public interface ICandidateQuestionAnswerRepository : IAsyncRepository, IAsyncInsertableRepository<CandidateQuestionAnswer>, IAsyncQueryableRepository<CandidateQuestionAnswer>, IAsyncDeleteableRepository<CandidateQuestionAnswer>, IAsyncFindableRepository<CandidateQuestionAnswer>, IRepository, IAsyncUpdateableRepository<CandidateQuestionAnswer>, IAsyncTransactionRepository
{
}
