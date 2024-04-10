using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace BAExamApp.MVC.Areas.CandidateAdmin.Models.StudentVMs;

public class CandidateAdminCandidateCreateVM
{
    [Display(Name = "First_Name")]
    [Required(ErrorMessage = "{0} alanı boş bırakılamaz.")]
    public string FirstName { get; set; }

    [Display(Name = "Last_Name")]
    [Required(ErrorMessage = "{0} alanı boş bırakılamaz.")]
    public string LastName { get; set; }

    [Display(Name = "Email")]
    [Required(ErrorMessage = "{0} alanı boş bırakılamaz.")]
    public string Email { get; set; } = string.Empty;

    [Display(Name = "Profile_Image")]
    public IFormFile? NewImage { get; set; }

    [Display(Name = "Date_Of_Birth")]
    [Required(ErrorMessage = "{0} alanı boş bırakılamaz.")]
    [DataType(DataType.Date, ErrorMessage = "Lütfen geçerli bir tarih giriniz.")]
    [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
    public DateTime DateOfBirth { get; set; }

    [Display(Name = "Gender")]
    [Required(ErrorMessage = "{0} alanı boş bırakılamaz.")]
    public bool? Gender { get; set; }

    //[Display(Name = "Group")]
    //[Required(ErrorMessage = "{0} alanı boş bırakılamaz seçiniz.")]
    //public Guid GroupId { get; set; }

    //public SelectList? Groups { get; set; }

}
