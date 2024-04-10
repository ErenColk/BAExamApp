using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BAExamApp.DataAccess.Interfaces.Repositories;
public interface ICandidateExamRepository: IAsyncRepository, IAsyncInsertableRepository<CandidateExam>, IAsyncQueryableRepository<CandidateExam>, IAsyncFindableRepository<CandidateExam>, IAsyncUpdateableRepository<CandidateExam>, IAsyncDeleteableRepository<CandidateExam>
{
}
