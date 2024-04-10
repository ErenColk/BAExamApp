using BAExamApp.MVC.Areas.CandidateAdmin.Models.QuestionVMs;
using BAExamApp.MVC.Areas.Trainer.Models.QuestionVMs;
using FluentValidation;
using Microsoft.Extensions.Localization;


namespace BAExamApp.MVC.FluentValidators.CandidateQuestionValidators;

public class QuestionCreateValidator : AbstractValidator<CandidateQuestionCreateVM>
{



    public QuestionCreateValidator(IStringLocalizer<CandidateQuestionCreateVM> stringLocalizer)
    {
     
        RuleFor(t0 => t0.CandidateQuestionType).Must((rootObject, list) => rootObject.CandidateQuestionType > 0).WithMessage(stringLocalizer["Please_Choose_QuestionType"]);
        RuleFor(t0 => t0.Content).NotEmpty().WithMessage(stringLocalizer["Please_Do_Not_Leave_QuestionContent_Blank"]);
    }
}
