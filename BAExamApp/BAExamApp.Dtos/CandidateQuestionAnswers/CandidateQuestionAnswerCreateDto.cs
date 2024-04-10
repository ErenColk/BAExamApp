using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAExamApp.Dtos.CandidateQuestionAnswers;
public class CandidateQuestionAnswerCreateDto
{
    public string Answer { get; set; }
    public bool IsRightAnswer { get; set; }
    public Guid QuestionId { get; set; }

}
