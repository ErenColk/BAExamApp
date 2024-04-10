using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAExamApp.DataAccess.EFCore.Repositories;
public class CandidateGroupRepository : EFBaseRepository<CandidateGroup>, ICandidateGroupRepository
{
    public CandidateGroupRepository(BAExamAppDbContext context) : base(context)
    {
    }
}
