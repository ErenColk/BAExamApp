using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAExamApp.Entities.DbSets;
public class CandidateBranch : AuditableEntity
{
    public CandidateBranch()
    {
       
        //Candidates = new HashSet<CandidateStudent>();
        CandidateGroups = new HashSet<CandidateGroup>();
    }

    public string Name { get; set; } = null!;

    //Navigation Prop.
   
    public virtual ICollection<CandidateGroup> CandidateGroups { get; set; }
    //public virtual ICollection<CandidateStudent> Candidates { get; set; }
}
