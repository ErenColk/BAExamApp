using BAExamApp.Entities.Enums;
using System.ComponentModel.DataAnnotations;

namespace BAExamApp.MVC.Areas.CandidateAdmin.Models.QuestionVMs;

public class CandidateQuestionListVM
{

    [Display(Name = "Id")]
    public Guid Id { get; set; }
    [Display(Name = "Question_Content")]
    public string? Content { get; set; }
    [Display(Name = "Candidate_QuestionType")]
    public CandidateQuestionType? CandidateQuestionType { get; set; }
    [Display(Name = "Created_Date")]
    public DateTime CreatedDate { get; set; }
    [Display(Name = "Modified_Date")]
    public DateTime ModifiedDate { get; set; }
    [Display(Name = "Modified_By")]
    public string ModifiedBy { get; set; }


}
