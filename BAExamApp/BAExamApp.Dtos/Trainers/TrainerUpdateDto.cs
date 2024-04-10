namespace BAExamApp.Dtos.Trainers;

public class TrainerUpdateDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public bool Gender { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string Image { get; set; }
    public Guid CityId { get; set; }
    public List<Guid> TrainerTalentIds { get; set; }
    public List<Guid> ProductIds { get; set; }
    public Guid? TechnicalUnitId { get; set; }
}