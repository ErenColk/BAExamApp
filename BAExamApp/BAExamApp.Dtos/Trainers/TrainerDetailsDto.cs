using BAExamApp.Dtos.TrainerClassrooms;
using BAExamApp.Dtos.TrainerTalents;

namespace BAExamApp.Dtos.Trainers;

public class TrainerDetailsDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public DateTime DateOfBirth { get; set; }
    public bool Gender { get; set; }
    public string Image { get; set; }
    public string CityName { get; set; }
    public string IdentityId { get; set; }
    public List<TrainerClassroomListForTrainerDetailsDto> TrainerClassrooms { get; set; }
    public List<TrainerTalentListForTrainerDetailsDto> TrainerTalents { get; set; }

}