namespace BAExamApp.Entities.DbSets;
public class CandidateAnswer : BaseEntity
{
    public string? WrittenAnswer { get; set; }
    public bool? IsSelected { get; set; }

    //Navigation Prop.
    public Guid CandidateQuestionAnswerId { get; set; }
    public virtual CandidateQuestionAnswer? CandidateQuestionAnswer { get; set; }
    public Guid CandidateQuestionId { get; set; }
    public virtual CandidateQuestion? CandidateQuestion { get; set; }
}
