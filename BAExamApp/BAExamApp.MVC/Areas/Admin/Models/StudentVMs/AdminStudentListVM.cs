using System.ComponentModel.DataAnnotations;


namespace BAExamApp.MVC.Areas.Admin.Models.StudentVMs;

public class AdminStudentListVM
{
    public Guid Id { get; set; }
    [Display(Name = "FirstName")]
    public string FirstName { get; set; }
    [Display(Name = "LastName")]
    public string LastName { get; set; }
    [Display(Name = "Email")]
    public string Email { get; set; }
    public string Image { get; set; }
}