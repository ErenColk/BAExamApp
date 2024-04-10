using Microsoft.EntityFrameworkCore;

namespace BAExamApp.Entities.Configurations;
public class CandidateAnswerConfiguration : BaseEntityConfiguration<CandidateAnswer>
{
    public override void Configure(EntityTypeBuilder<CandidateAnswer> builder)
    {
        base.Configure(builder);
        builder.ToTable("CandidateAnswer", "candidate");

        //builder.HasOne(x => x.CandidateQuestion).WithMany(x => x.CandidateAnswers).HasForeignKey(x => x.CandidateQuestionId);
        //builder.HasOne(x => x.CandidateQuestionAnswer).WithMany(x => x.CandidateAnswers).HasForeignKey(x => x.CandidateQuestionAnswerId);

        //builder.HasAlternateKey(x => new { x.CandidateQuestionId, x.CandidateQuestionAnswerId });
    }
}
