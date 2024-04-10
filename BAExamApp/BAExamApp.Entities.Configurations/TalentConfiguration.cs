namespace BAExamApp.Entities.Configurations;

public class TalentConfiguration : AuditableEntityConfiguration<Talent>
{
    public override void Configure(EntityTypeBuilder<Talent> builder)
    {
        base.Configure(builder);

        builder.Property(x => x.Name).HasMaxLength(512).IsRequired();
    }
}
