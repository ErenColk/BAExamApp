namespace BAExamApp.Entities.DbSets;

public class Talent : AuditableEntity
{
    public Talent()
    {
        TrainerTalents = new HashSet<TrainerTalent>();
    }

    public string Name { get; set; } = null!;

    //Navigation Prop.
    public virtual ICollection<TrainerTalent> TrainerTalents { get; set; }
}
