using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAExamApp.Entities.Configurations;
public class CandidateBranchConfiguration : AuditableEntityConfiguration<CandidateBranch>
{
    public override void Configure(EntityTypeBuilder<CandidateBranch> builder)
    {
        base.Configure(builder);       
        //builder.HasMany(x => x.Candidates).WithOne(x => x.CandidateBranch).HasForeignKey(x => x.CandidateBranchId);
        builder.ToTable("Branches", "candidate");
    }
}
