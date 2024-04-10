using BAExamApp.MVC.Areas.CandidateAdmin.Models.CandidateAdminVMs;
using BAExamApp.MVC.FluentValidators.CustomValidators;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace BAExamApp.MVC.FluentValidators.CandidateAdminValidators;

public class CandidateAdminUpdateValidator : AbstractValidator<CandidateAdminUpdateVM>
{
    public CandidateAdminUpdateValidator(IStringLocalizer<CandidateAdminUpdateVM> stringLocalizer)
    {
        RuleFor(x => x.FirstName).NotEmpty().WithMessage(stringLocalizer["Name_Can_Not_Be_Empty"])
            .MinimumLength(2).WithMessage(stringLocalizer["Name_Must_Consist_Of_At_Least_2_Character"])
            .Matches(@"^(?!'+$)[a-zA-Z'ğüşöçıİĞÜŞÖÇ]+(?:\s+[a-zA-Z'ğüşöçıİĞÜŞÖÇ]+)*$").WithMessage(stringLocalizer["Name_Can_Not_Consist_Special_Character"]);

        RuleFor(x => x.LastName).NotEmpty().WithMessage(stringLocalizer["LastName_Can_Not_Be_Empty"])
                .MinimumLength(2).WithMessage(stringLocalizer["LastName_Must_Consist_Of_At_Least_2_Character"])
                .Matches(@"^(?!'+$)[a-zA-Z'ğüşöçıİĞÜŞÖÇ]+(?:\s+[a-zA-Z'ğüşöçıİĞÜŞÖÇ]+)*$").WithMessage(stringLocalizer["LastName_Can_Not_Consist_Special_Character"]);

        RuleFor(x => x.Email).NotEmpty().WithMessage(stringLocalizer["Email_Can_Not_Be_Empty"])
                .EmailAddress().WithMessage(stringLocalizer["Please_Enter_A_Valid_Email_Address"]);        


        RuleFor(x => x.DateOfBirth).NotEmpty().WithMessage(stringLocalizer["Date_Of_Birth_Can_Not_Be_Empty"])
                    .LessThanOrEqualTo(DateTime.Now).WithMessage(stringLocalizer["Date_Of_Birth_Cannot_Be_A_Later_Date"]);


        RuleFor(x => x.NewImage).SetValidator(new ProfileImageValidator()!).When(model => model.NewImage != null);
    }
}
    
