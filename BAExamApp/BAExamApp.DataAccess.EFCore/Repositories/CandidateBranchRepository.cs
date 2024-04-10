using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAExamApp.DataAccess.EFCore.Repositories;
public class CandidateBranchRepository : EFBaseRepository<CandidateBranch>, ICandidateBranchRepository
{
    public CandidateBranchRepository(BAExamAppDbContext context) : base(context)
    {

    }

}
