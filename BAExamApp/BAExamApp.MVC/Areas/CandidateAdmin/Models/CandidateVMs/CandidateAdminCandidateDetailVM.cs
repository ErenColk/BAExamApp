using System.ComponentModel.DataAnnotations;

namespace BAExamApp.MVC.Areas.CandidateAdmin.Models.StudentVMs;

public class CandidateAdminCandidateDetailVM
{
    public Guid Id { get; set; }

    [Display(Name = "First_Name")]
    public string FirstName { get; set; }

    [Display(Name = "Last_Name")]
    public string LastName { get; set; }

    [Display(Name = "Email")]
    public string Email { get; set; }

    [Display(Name = "Date_Of_Birth")]
    public DateTime DateOfBirth { get; set; }

    [Display(Name = "Gender")]
    public bool Gender { get; set; }

    [Display(Name = "Profile_Image")]
    public string Image { get; set; }

    [Display(Name = "Group_Name")]
    public string GroupName { get; set; }
}
