using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAExamApp.Entities.Configurations;
public class CandidateQuestionAnswerConfiguration : AuditableEntityConfiguration<CandidateQuestionAnswer>
{
    public override void Configure(EntityTypeBuilder<CandidateQuestionAnswer> builder)
    {
        base.Configure(builder);

        builder.ToTable("QuestionAnswers", "candidate");

        builder.Property(x => x.Answer).IsRequired().HasColumnType("nvarchar(max)");

        builder.HasOne(aq=> aq.Question).WithMany(q => q.QuestionAnswers).HasForeignKey(q => q.QuestionId);
    }
}
