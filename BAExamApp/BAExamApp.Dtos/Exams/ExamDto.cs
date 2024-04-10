using BAExamApp.Entities.Enums;

namespace BAExamApp.Dtos.Exams;

public class ExamDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public DateTime ExamDateTime { get; set; }
    public TimeSpan ExamDuration { get; set; }
    public string? Description { get; set; }
    public decimal MaxScore { get; set; }
    public decimal BonusScore { get; set; }
    public bool? IsStarted { get; set; } = false;

    public Guid ExamRuleId { get; set; }
    public Guid? ClassroomId { get; set; }
    public string? TrainerExplanation { get; set; }
    public ExamType ExamType { get; set; }
    public ExamCreationType ExamCreationType { get; set; }
}