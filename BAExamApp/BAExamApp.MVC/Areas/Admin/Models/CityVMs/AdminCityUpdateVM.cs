using System.ComponentModel.DataAnnotations;

namespace BAExamApp.MVC.Areas.Admin.Models.CityVMs;

public class AdminCityUpdateVM
{
    public Guid Id { get; set; }

    [Display(Name = "City")]
    [Required(ErrorMessage = "Şehir boş bırakılamaz.")]
    [MinLength(3, ErrorMessage = "Şehir en az 3 karakterden oluşmalıdır.")]
    [RegularExpression(@"^(?!'+$)[a-zA-Z'ğüşöçİĞÜŞÖÇ]+(?:\s+[a-zA-Z'ğüşöçİĞÜŞÖÇ]+)*$", ErrorMessage = "Şehir adı özel karakter ya da sayı içeremez.")]
    public string Name { get; set; }
}
