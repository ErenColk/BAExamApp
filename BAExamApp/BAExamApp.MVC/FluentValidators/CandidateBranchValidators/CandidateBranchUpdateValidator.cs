using BAExamApp.MVC.Areas.CandidateAdmin.Models.Branch;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace BAExamApp.MVC.FluentValidators.CandidateBranchValidators;

public class CandidateBranchUpdateValidator : AbstractValidator<CandidateBranchUpdateVM>
{
    public CandidateBranchUpdateValidator(IStringLocalizer<CandidateBranchUpdateVM> stringLocalizer)
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage(stringLocalizer["Branch_Name_Not_Empty"])
           .MinimumLength(3).WithMessage(stringLocalizer["Branch_Name_Minimum_3_Character"])
           .Matches(@"^(?!'+$)[a-zA-Z'ğüşöçıİĞÜŞÖÇ]+(?:\s+[a-zA-Z'ğüşöçıİĞÜŞÖÇ]+)*$").WithMessage(stringLocalizer["Branch_Name_Minimum_3_Character"]);
    }
}
