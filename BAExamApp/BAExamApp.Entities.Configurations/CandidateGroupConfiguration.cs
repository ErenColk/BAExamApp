using BAExamApp.Core.Entities.EntityTypeConfigurations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAExamApp.Entities.Configurations;
public  class CandidateGroupConfiguration : AuditableEntityConfiguration<CandidateGroup>
{
    public override void Configure(EntityTypeBuilder<CandidateGroup> builder)
    {
        base.Configure(builder);

        base.Configure(builder);
        builder.HasOne(x => x.CandidateBranch).WithMany(x => x.CandidateGroups).HasForeignKey(x => x.CandidateBranchId);
    }
}
