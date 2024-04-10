using System.ComponentModel.DataAnnotations;

namespace BAExamApp.MVC.Areas.Admin.Models.AdminVMs;

public class AdminAdminDetailsVM
{
    public Guid Id { get; set; }
   
    [Display(Name = "Full_Name")]
    public string FullName { get; set; }

    [Display(Name = "Email")]
    public string Email { get; set; }

    [Display(Name = "Gender")]
    public bool Gender { get; set; }

    [Display(Name = "Date_Of_Birth")]
    public DateTime DateOfBirth { get; set; }

    [Display(Name = "Profile_Image")]
    public string Image { get; set; }

    [Display(Name = "City")]
    public string CityName { get; set; }

    [Display(Name = "OtherEmails")]
    public List<string>? OtherEmails { get; set; }
}