using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAExamApp.DataAccess.EFCore.Repositories;
public class TrainerTalentRepository : EFBaseRepository<TrainerTalent>, ITrainerTalentRepository
{
    public TrainerTalentRepository(BAExamAppDbContext context) : base(context)
    {

    }
}
