using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BAExamApp.DataAccess.EFCore.Repositories;
public class CandidateExamRepository : EFBaseRepository<CandidateExam>, ICandidateExamRepository
{
    public CandidateExamRepository(BAExamAppDbContext context) : base(context)
    {

    }

}
