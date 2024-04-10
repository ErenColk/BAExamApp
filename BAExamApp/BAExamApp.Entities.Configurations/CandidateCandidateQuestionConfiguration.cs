using Microsoft.EntityFrameworkCore;

namespace BAExamApp.Entities.Configurations;
public class CandidateCandidateQuestionConfiguration : AuditableEntityConfiguration<CandidateCandidateQuestion>
{
    public override void Configure(EntityTypeBuilder<CandidateCandidateQuestion> builder)
    {
        base.Configure(builder);

        builder.Property(x => x.MaxScore).IsRequired();
        builder.Property(x => x.BonusScore).IsRequired();
        builder.Property(x => x.Score).IsRequired(false);
        builder.Property(x => x.QuestionOrder).IsRequired();
        builder.Property(x => x.TimeStarted).IsRequired(false);
        builder.Property(x => x.TimeFinished).IsRequired(false);
        builder.ToTable("Candidates_Questions", "candidate");

        //builder.HasOne(x => x.CandidateExam).WithMany(x => x.CandidateQuestions).HasForeignKey(x => x.CandidateExamId);
        //builder.HasOne(x => x.CandidateQuestion).WithMany(x => x.CandidateStudentQuestion).HasForeignKey(x => x.CandidateQuestionId);

    }
}
