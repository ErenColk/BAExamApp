using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAExamApp.Entities.DbSets;
public class Candidate : BaseUser
{
    // Navigation Prop.
    public Guid? CandidateGroupId { get; set; }
    public virtual CandidateGroup? CandidateGroup { get; set; }
}
