using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAExamApp.Entities.Configurations;
public class CandidateAdminConfiguration : BaseUserConfiguration<CandidateAdmin>
{
    public override void Configure(EntityTypeBuilder<CandidateAdmin> builder)
    {
        base.Configure(builder);        
        builder.ToTable("Admins", "candidate");
    }
}
