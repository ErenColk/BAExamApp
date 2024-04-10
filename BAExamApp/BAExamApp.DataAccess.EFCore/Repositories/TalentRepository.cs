using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAExamApp.DataAccess.EFCore.Repositories;
public class TalentRepository : EFBaseRepository<Talent>, ITalentRepository
{
    public TalentRepository(BAExamAppDbContext context) : base(context) { }
}
