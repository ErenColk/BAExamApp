using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAExamApp.DataAccess.EFCore.Repositories;
public class CandidateRepository : EFBaseRepository<Candidate>, ICandidateRepository
{
    public CandidateRepository(BAExamAppDbContext context) : base(context)
    {
    }
}
