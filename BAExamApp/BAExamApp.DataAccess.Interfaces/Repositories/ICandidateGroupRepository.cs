using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAExamApp.DataAccess.Interfaces.Repositories;
public interface ICandidateGroupRepository : IAsyncRepository, IAsyncQueryableRepository<CandidateGroup>, IAsyncFindableRepository<CandidateGroup>, IAsyncInsertableRepository<CandidateGroup>, IAsyncUpdateableRepository<CandidateGroup>, IAsyncDeleteableRepository<CandidateGroup>
{

}

