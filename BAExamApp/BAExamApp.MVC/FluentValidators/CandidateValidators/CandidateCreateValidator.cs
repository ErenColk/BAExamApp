using BAExamApp.MVC.Areas.CandidateAdmin.Models.StudentVMs;
using BAExamApp.MVC.FluentValidators.CustomValidators;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace BAExamApp.MVC.FluentValidators.CandidateStudentValidators;

public class CandidateCreateValidator : AbstractValidator<CandidateAdminCandidateCreateVM>
{
    public CandidateCreateValidator(IStringLocalizer<CandidateAdminCandidateCreateVM> stringLocalizer)
    {
        RuleFor(x => x.FirstName).NotEmpty().WithMessage(stringLocalizer["Name_Can_Not_Be_Empty"])
                           .MinimumLength(2).WithMessage(stringLocalizer["Name_Must_Consist_Of_At_Least_2_Character"])
                           .Matches(@"^(?!'+$)[a-zA-Z'ğüşöçıİĞÜŞÖÇ]+(?:\s+[a-zA-Z'ğüşöçıİĞÜŞÖÇ]+)*$").WithMessage(stringLocalizer["Name_Can_Not_Consist_Special_Character"])
                           .MaximumLength(256).WithMessage(stringLocalizer["Candidate_Name_Must_Contain_Maximum_Of_256_Characters"]);

        RuleFor(x => x.LastName).NotEmpty().WithMessage(stringLocalizer["LastName_Can_Not_Be_Empty"])
                            .MinimumLength(2).WithMessage(stringLocalizer["LastName_Must_Consist_Of_At_Least_2_Character"])
                            .Matches(@"^(?!'+$)[a-zA-Z'ğüşöçıİĞÜŞÖÇ]+(?:\s+[a-zA-Z'ğüşöçıİĞÜŞÖÇ]+)*$").WithMessage(stringLocalizer["LastName_Can_Not_Consist_Special_Character"])
                            .MaximumLength(256).WithMessage(stringLocalizer["Candidate_LastName_Must_Contain_Maximum_Of_256_Characters"]);

        RuleFor(x => x.Email).NotEmpty().WithMessage(stringLocalizer["Email_Can_Not_Be_Empty"])
                             .EmailAddress().WithMessage(stringLocalizer["Please_Enter_A_Valid_Email_Address"]);

        RuleFor(x => x.DateOfBirth).NotEmpty().WithMessage(stringLocalizer["Date_Of_Birth_Can_Not_Be_Empty"])
        .LessThan(x => DateTime.Today.AddYears(-15))
        .WithMessage(stringLocalizer["Candidate_Must_Be_Over_15_Years_Old"]);

        RuleFor(x => x.NewImage).SetValidator(new ProfileImageValidator()).When(model => model.NewImage != null);
    }
}
