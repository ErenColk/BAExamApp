namespace BAExamApp.Entities.Configurations;

public class TrainerTalentConfiguration : BaseEntityConfiguration<TrainerTalent>
{
    public override void Configure(EntityTypeBuilder<TrainerTalent> builder)
    {
        base.Configure(builder);

        builder.HasOne(x => x.Trainer).WithMany(x => x.TrainerTalents).HasForeignKey(x => x.TrainerId);
        builder.HasOne(x => x.Talent).WithMany(x => x.TrainerTalents).HasForeignKey(x => x.TalentId);

        builder.HasAlternateKey(x => new { x.TalentId, x.TrainerId });
    }
}
