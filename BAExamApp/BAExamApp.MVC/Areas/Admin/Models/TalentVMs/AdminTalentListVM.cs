using System.ComponentModel.DataAnnotations;

namespace BAExamApp.MVC.Areas.Admin.Models.TalentVMs;

public class AdminTalentListVM
{
    public Guid Id { get; set; }

    [Display(Name = "Talent_Name")]
    public string Name { get; set; }
}
