namespace BAExamApp.Entities.DbSets;
public class CandidateCandidateQuestion : AuditableEntity
{
    public CandidateCandidateQuestion()
    {
        CandidateStudentAnswers = new HashSet<CandidateAnswer>();
    }
    public int MaxScore { get; set; }
    public int BonusScore { get; set; } = 0;
    public int? Score { get; set; }
    public int QuestionOrder { get; set; }
    public DateTime? TimeStarted { get; set; }
    public DateTime? TimeFinished { get; set; }

    //Navigation Prop.
    //public Guid CandidateExamId { get; set; }
    //public virtual CandidateExam? CandidateExam { get; set; }
    public Guid CandidateQuestionId { get; set; }
    public virtual CandidateQuestion? CandidateQuestion { get; set; }

    public virtual ICollection<CandidateAnswer> CandidateStudentAnswers { get; set; }
}
