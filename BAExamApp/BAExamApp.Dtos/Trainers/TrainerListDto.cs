using BAExamApp.Core.Enums;

namespace BAExamApp.Dtos.Trainers;

public class TrainerListDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
    public bool Gender { get; set; }
    public DateTime ModifiedDate { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string? Image { get; set; }
    public string IdentityId { get; set; }
    public Guid CityId { get; set; }
    //public bool Selected { get; set; }
    public Status Status { get; set; }
}