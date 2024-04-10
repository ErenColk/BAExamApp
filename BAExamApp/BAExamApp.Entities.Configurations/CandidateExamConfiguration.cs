using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAExamApp.Entities.Configurations;
public class CandidateExamConfiguration : AuditableEntityConfiguration<CandidateExam>
{
    public override void Configure(EntityTypeBuilder<CandidateExam> builder)
    {
        base.Configure(builder);


        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
               .HasMaxLength(30)
               .IsRequired();

        builder.ToTable("CandidateExams", "candidate");
    }
}
