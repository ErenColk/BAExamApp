namespace BAExamApp.Entities.DbSets;

public class TrainerTalent:BaseEntity
{
    //Navigation Prop.
    public Guid TrainerId { get; set; }
    public virtual Trainer? Trainer { get; set; }
    public Guid TalentId { get; set; }
    public virtual Talent? Talent { get; set; }
}
