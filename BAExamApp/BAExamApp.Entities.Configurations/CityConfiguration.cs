namespace BAExamApp.Entities.Configurations;

public class CityConfiguration : BaseEntityConfiguration<City>
{
    public override void Configure(EntityTypeBuilder<City> builder)
    {
        base.Configure(builder);

        builder.Property(x => x.Name).HasMaxLength(256).IsRequired();
    }
}
