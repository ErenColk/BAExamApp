namespace BAExamApp.DataAccess.EFCore.Repositories;
public class CandidateAnswerRepository : EFBaseRepository<CandidateAnswer>, ICandidateAnswerRepository
{
    public CandidateAnswerRepository(BAExamAppDbContext context) : base(context) { }
}
