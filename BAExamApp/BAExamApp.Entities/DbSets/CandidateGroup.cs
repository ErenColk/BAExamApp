using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAExamApp.Entities.DbSets;
public class CandidateGroup : AuditableEntity
{
    // Navigation Prop.
    public CandidateGroup()
    {
        Candidates = new HashSet<Candidate>();
    }
    public string Name { get; set; }
    public virtual ICollection<Candidate> Candidates { get; set; }
    public Guid CandidateBranchId { get; set; }
    public virtual CandidateBranch CandidateBranch { get; set; }
}
