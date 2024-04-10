using BAExamApp.Entities.Enums;
using BAExamApp.MVC.Areas.CandidateAdmin.Models.QuestionAnswerVMs;
using BAExamApp.MVC.Areas.Trainer.Models.QuestionAnswerVMs;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BAExamApp.MVC.Areas.CandidateAdmin.Models.QuestionVMs;

public class CandidateQuestionCreateVM
{


    [Display(Name = "Question_Content")]
    public string? Content { get; set; }
    [JsonIgnore]
    public IFormFile? Image { get; set; }
    [Display(Name = "Candidate_QuestionType")]
    public CandidateQuestionType CandidateQuestionType { get; set; }
    public List<CandidateQuestionAnswerCreateVM> QuestionAnswers { get; set; }
}
