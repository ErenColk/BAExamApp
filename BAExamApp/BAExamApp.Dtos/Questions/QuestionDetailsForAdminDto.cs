using BAExamApp.Dtos.QuestionAnswers;

namespace BAExamApp.Dtos.Questions;

public class QuestionDetailsForAdminDto
{
    public Guid Id { get; set; }
    public string CreatedBy { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime ModifiedDate { get; set; }
    public string Content { get; set; }
    public int State { get; set; }
    public int QuestionType { get; set; }
    public string Image { get; set; }
    public List<string> SubtopicName { get; set; }
    public string QuestionDifficultyName { get; set; }
    public string ReviewComment { get; set; }
    public TimeSpan AverageAnswerTime { get; set; }
    public TimeSpan LongestAnswerTime { get; set; }
    public TimeSpan ShortestAnswerTime { get; set; }
    public List<QuestionAnswerDto> QuestionAnswers { get; set; }
}
