using System.ComponentModel.DataAnnotations;

namespace BAExamApp.MVC.Areas.CandidateAdmin.Models.Branch;

public class CandidateBranchUpdateVM
{
    public string Id { get; set; }

    [Display(Name = "Branch_Name")]
    public string Name { get; set; }
}
