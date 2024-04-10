using BAExamApp.Dtos.CandidateQuestionAnswers;
using BAExamApp.Entities.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BAExamApp.Dtos.CandidateQuestions;
public class CandidateQuestionCreateDto
{

    public string? Content { get; set; }
    public string? Image { get; set; }
    public CandidateQuestionType CandidateQuestionType { get; set; }
    public List<CandidateQuestionAnswerCreateDto> QuestionAnswers { get; set; }

}
