namespace BAExamApp.Business.Services;
public class CandidateCandidateQuestionService : ICandidateStudentQuestionService
{
    private readonly ICandidateCandidateQuestionRepository _candidateStudentQuestionRepository;

    public CandidateCandidateQuestionService(ICandidateCandidateQuestionRepository candidateStudentQuestionRepository)
    {
        _candidateStudentQuestionRepository = candidateStudentQuestionRepository;
    }
}
