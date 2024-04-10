using BAExamApp.Dtos.CandidateQuestionAnswers;
using BAExamApp.Dtos.QuestionAnswers;
using BAExamApp.Entities.Enums;
using System.ComponentModel.DataAnnotations;

namespace BAExamApp.MVC.Areas.CandidateAdmin.Models.QuestionVMs;

public class CandidateQuestionDetailsVM
{
    [Display(Name = "Id")]
    public Guid Id { get; set; }
    [Display(Name = "Created_Date")]
    public DateTime CreatedDate { get; set; }

    [Display(Name = "Question_Content")]
    public string Content { get; set; }

    [Display(Name = "Question_Type")]
    public CandidateQuestionType CandidateQuestionType { get; set; }

    [Display(Name = "Question_Answers")]
    public List<CandidateQuestionAnswerDto> QuestionAnswers { get; set; }
    public string? Image { get; set; }




}
