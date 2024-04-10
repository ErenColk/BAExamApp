using System.ComponentModel.DataAnnotations;

namespace BAExamApp.MVC.Areas.Admin.Models.StudentVMs;

public class AdminStudentDetailVM
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
    public List<string> Classrooms { get; set; }

    [Display(Name = "OtherEmails")]
    public List<string>? OtherEmails { get; set; }
}