namespace BAExamApp.DataAccess.EFCore.Repositories;
public class CandidateCandidateQuestionRepository : EFBaseRepository<CandidateCandidateQuestion>, ICandidateCandidateQuestionRepository
{
    public CandidateCandidateQuestionRepository(BAExamAppDbContext context) : base(context) { }
}
