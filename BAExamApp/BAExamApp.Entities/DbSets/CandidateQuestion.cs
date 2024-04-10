using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAExamApp.Entities.DbSets;
public class CandidateQuestion : AuditableEntity
{
    public CandidateQuestion()
    {
        QuestionAnswers = new HashSet<CandidateQuestionAnswer>();
    }

    public string Content { get; set; }
    public CandidateQuestionType CandidateQuestionType { get; set; }
    public string Image { get; set; }
    public bool IsActive { get; set; } = true;

    //Navigation prop.
    public virtual ICollection <CandidateQuestionAnswer> QuestionAnswers { get; set;}
}
