namespace BAExamApp.Entities.DbSets;

public class City : BaseEntity
{
    public City()
    {
        Students = new HashSet<Student>();
        Trainers = new HashSet<Trainer>();
        Admins = new HashSet<Admin>();
    }

    public string Name { get; set; } = null!;

    //Navigation Prop.
    public virtual ICollection<Student> Students { get; set; }
    public virtual ICollection<Trainer> Trainers { get; set; }
    public virtual ICollection<Admin> Admins { get; set; }
}