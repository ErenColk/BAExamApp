using BAExamApp.Dtos.CandidateQuestionAnswers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAExamApp.Dtos.CandidateQuestions;
public class CandidateQuestionDetailsDto
{
    public Guid Id { get; set; }
    public string Content { get; set; }
    public DateTime CreatedDate { get; set; }
    public int CandidateQuestionType { get; set; }
    public string? Image { get; set; }
    public List<CandidateQuestionAnswerDto> QuestionAnswers { get; set; }
}
