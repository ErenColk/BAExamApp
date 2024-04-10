namespace BAExamApp.Dtos.Trainers;

public class TrainerCreateDto
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public bool Gender { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string Image { get; set; }
    public Guid BranchId { get; set; }
    public Guid TechnicalUnitId { get; set; }
    public List<Guid> ProductIds { get; set; }
    public Guid CityId { get; set; }
    public List<string>? OtherEmails { get; set; }
    public List<Guid> TalentIds { get; set; }
    public Guid TalentId { get; set; }
}