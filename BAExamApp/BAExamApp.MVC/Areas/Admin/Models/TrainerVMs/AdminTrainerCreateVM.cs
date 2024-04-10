using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace BAExamApp.MVC.Areas.Admin.Models.TrainerVMs;

public class AdminTrainerCreateVM
{
    [Display(Name = "First_Name")]
    [Required(ErrorMessage = "{0} alanı boş bırakılamaz.")]
    public string FirstName { get; set; }

    [Display(Name = "Last_Name")]
    [Required(ErrorMessage = "{0} alanı boş bırakılamaz.")]
    public string LastName { get; set; }

    [Display(Name = "Email")]
    [Required(ErrorMessage = "{0} alanı boş bırakılamaz.")]
    public string Email { get; set; }

    [Display(Name = "Gender")]
    [Required(ErrorMessage = "{0} alanı boş bırakılamaz.")]
    public bool Gender { get; set; }

    [Display(Name = "Date_Of_Birth")]
    [Required(ErrorMessage = "{0} alanı boş bırakılamaz.")]
    [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
    public DateTime DateOfBirth { get; set; }

    [Display(Name = "Profile_Image")]
    public IFormFile? NewImage { get; set; }

    [Display(Name = "TechnicalUnit_Name")]
    [Required(ErrorMessage = "{0} alanı boş bırakılamaz.")]
    public Guid TechnicalUnitId { get; set; }
    public SelectList? TechnicalUnitList { get; set; }
    public List<Guid> ProductIds { get; set; }
    public SelectList? ProductList { get; set; }
    [Display(Name = "City")]
    [Required(ErrorMessage = "{0} alanı boş bırakılamaz.")]
    public Guid CityId { get; set; }
    public SelectList? CityList { get; set; }
    [Display(Name = "OtherEmails")]
    public List<string>? OtherEmails { get; set; }
    [Display(Name = "Talent")]
    public List<Guid>? TalentIds { get; set; }
    [Display(Name = "Talent")]
    //[Required(ErrorMessage = "{0} alanı boş bırakılamaz.")]
    public Guid? TalentId { get; set; }
    public SelectList? TalentList { get; set; }
}