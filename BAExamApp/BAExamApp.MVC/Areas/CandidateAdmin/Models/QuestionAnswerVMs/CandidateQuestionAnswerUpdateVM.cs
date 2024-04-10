namespace BAExamApp.MVC.Areas.CandidateAdmin.Models.QuestionAnswerVMs;

public class CandidateQuestionAnswerUpdateVM
{

    public Guid Id { get; set; }
    public string Answer { get; set; }
    public bool IsRightAnswer { get; set; }
    public Guid QuestionId { get; set; }
}
