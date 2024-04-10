using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAExamApp.Entities.Configurations;
public class CandidateQuestionConfiguration : AuditableEntityConfiguration<CandidateQuestion>
{
    public override void Configure(EntityTypeBuilder<CandidateQuestion> builder)
    {
        base.Configure(builder);

        builder.ToTable("Questions", "candidate");

        builder.Property(x => x.Content).IsRequired().HasColumnType("nvarchar(max)");
        builder.Property(x=> x.CandidateQuestionType).IsRequired();
        builder.Property(x => x.Image).IsRequired(false).HasColumnType("nvarchar(max)");
        builder.Property(x => x.IsActive).IsRequired();

    }
}
