using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BAExamApp.DataAccess.Interfaces.Repositories;
public interface ICandidateRepository : IAsyncRepository, IAsyncQueryableRepository<Candidate>, IAsyncFindableRepository<Candidate>, IAsyncInsertableRepository<Candidate>, IAsyncUpdateableRepository<Candidate>, IAsyncDeleteableRepository<Candidate>
{
   
}
