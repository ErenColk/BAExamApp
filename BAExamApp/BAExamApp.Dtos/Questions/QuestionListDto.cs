namespace BAExamApp.Dtos.Questions;

public class QuestionListDto
{
    public Guid Id { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime ModifiedDate { get; set; }
    public string ModifiedBy { get; set; }
    public string Content { get; set; }
    public int QuestionType { get; set; }
    public string SubjectName { get; set; }
    public List<string> SubtopicName { get; set; }
    public Guid QuestionDifficultyId { get; set; }
    public string QuestionDifficultyName { get; set; }
    public TimeSpan TimeGiven { get; set; }
}