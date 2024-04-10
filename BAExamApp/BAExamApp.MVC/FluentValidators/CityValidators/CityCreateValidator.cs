using BAExamApp.MVC.Areas.Admin.Models.CityVMs;
using FluentValidation;

namespace BAExamApp.MVC.FluentValidators.CityValidators;

public class CityCreateValidator : AbstractValidator<AdminCityCreateVM>
{
    public CityCreateValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("Şehir alanı boş bırakılamaz.")
            .MinimumLength(3).WithMessage("Şehir alanı minimum 3 harften oluşabilir.")
            .Matches(@"^(?!'+$)[a-zA-Z'ğüşöçıİĞÜŞÖÇ]+(?:\s+[a-zA-Z'ğüşöçıİĞÜŞÖÇ]+)*$").WithMessage("Şehir alanı özel karakter ya da sayı içeremez");
           
    }
}
