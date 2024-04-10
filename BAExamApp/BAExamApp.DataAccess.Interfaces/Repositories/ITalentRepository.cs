using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAExamApp.DataAccess.Interfaces.Repositories;
public interface ITalentRepository : IAsyncRepository, IAsyncQueryableRepository<Talent>, IAsyncFindableRepository<Talent>, IAsyncInsertableRepository<Talent>, IAsyncDeleteableRepository<Talent>
{
}
