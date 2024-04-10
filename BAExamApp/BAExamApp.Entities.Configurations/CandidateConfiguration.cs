using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAExamApp.Entities.Configurations;
public class CandidateConfiguration : BaseUserConfiguration<Candidate>
{
    public override void Configure(EntityTypeBuilder<Candidate> builder)
    {
        base.Configure(builder);
        //builder.HasOne(x => x.CandidateBranch)
        //    .WithMany(x => x.Candidates)
        //    .HasForeignKey(x => x.CandidateBranchId)
        //    .OnDelete(DeleteBehavior.NoAction);
        builder.Property(x => x.IdentityId).IsRequired(false);
        builder.ToTable("Candidates", "candidate");
    }
}
