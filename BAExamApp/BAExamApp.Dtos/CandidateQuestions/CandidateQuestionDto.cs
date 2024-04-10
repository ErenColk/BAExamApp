using BAExamApp.Dtos.CandidateQuestionAnswers;
using BAExamApp.Dtos.QuestionAnswers;
using BAExamApp.Dtos.QuestionSubtopics;
using BAExamApp.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAExamApp.Dtos.CandidateQuestions;
public  class CandidateQuestionDto
{
    public Guid Id { get; set; }
    public string Content { get; set; }
    public int CandidateQuestionType { get; set; }
    public string? Image { get; set; }
    public List<CandidateQuestionAnswerDto> QuestionAnswers { get; set; }

   
}
