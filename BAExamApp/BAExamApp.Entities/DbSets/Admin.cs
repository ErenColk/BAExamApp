namespace BAExamApp.Entities.DbSets;

public class Admin : BaseUser
{
    //Navigation Prop.
    public Guid CityId { get; set; }
    public virtual City? City { get; set; }

    public virtual ICollection<QuestionRevision> QuestionRevisions { get; set; }
}
