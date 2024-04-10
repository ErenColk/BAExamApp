using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAExamApp.Entities.DbSets;
public class CandidateQuestionAnswer : AuditableEntity
{
    public string Answer { get; set; }
    public bool IsRightAnswer { get; set; }

    //Navigation prop.
    public Guid QuestionId { get; set; }
    public virtual CandidateQuestion? Question { get; set; }
}
