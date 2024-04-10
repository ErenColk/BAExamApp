using BAExamApp.MVC.Areas.CandidateAdmin.Models.Branch;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace BAExamApp.MVC.FluentValidators.CandidateBranchValidators;

public class CandidateBranchCreateValidator : AbstractValidator<CandidateBranchCreateVM>
{
    public CandidateBranchCreateValidator(IStringLocalizer<CandidateBranchCreateVM> stringLocalizer)
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage(stringLocalizer["Branch_Name_Cannot_Be_Left-Empty"])
            .MinimumLength(3).WithMessage(stringLocalizer["Branch_Name_Minimum_3_Character"])
            .Matches(@"^(?!'+$)[a-zA-Z'ğüşöçİĞÜŞÖÇ]+(?:\s+[a-zA-Z'ğüşöçİĞÜŞÖÇ]+)*$")
            .WithMessage(stringLocalizer["Branch_Name_Can_Not_Special_Character"]);
    }
}
