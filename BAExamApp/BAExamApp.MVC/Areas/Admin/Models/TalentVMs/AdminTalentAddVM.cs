using BAExamApp.Entities.DbSets;
using System.ComponentModel.DataAnnotations;

namespace BAExamApp.MVC.Areas.Admin.Models.TalentVMs;

public class AdminTalentAddVM
{
    [Display(Name = "Talent")]
    [Required(ErrorMessage ="Bu alan boş bırakılamaz.")]
    public string Name { get; set; }
    public List<Talent> Talents { get; set; }
}
