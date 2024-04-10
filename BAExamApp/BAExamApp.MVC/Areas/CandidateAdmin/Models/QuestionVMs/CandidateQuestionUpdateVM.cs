using BAExamApp.Entities.Enums;
using BAExamApp.MVC.Areas.CandidateAdmin.Models.QuestionAnswerVMs;
using BAExamApp.MVC.Areas.Trainer.Models.QuestionAnswerVMs;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace BAExamApp.MVC.Areas.CandidateAdmin.Models.QuestionVMs;

public class CandidateQuestionUpdateVM
{

    [Display(Name = "Id")]
    public Guid Id { get; set; }

    [Display(Name = "Question_Content")]
    public string Content { get; set; }

    [Required(ErrorMessage = "zorunlu")]
    [Display(Name = "Candidate_Question_Type")]
    public CandidateQuestionType CandidateQuestionType { get; set; }
    public List<SelectListItem>? CandidateQuestionTypeList { get; set; }

    [Display(Name = "Profile_Image")]
    public IFormFile? NewImage { get; set; }
    public string? Image { get; set; }

    [BindProperty]
    public List<CandidateQuestionAnswerUpdateVM> QuestionAnswers { get; set; }
    public string CandidateQuestionAnswersList { get; set; }
}
