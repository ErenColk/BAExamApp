using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BAExamApp.MVC.Areas.Admin.Models.SubjectVMs;

public class AdminSubjectUpdateVM
{
    public Guid Id { get; set; }

    [DisplayName("Name")]
    [Required(ErrorMessage = "Bu alan boş bırakılamaz.")]
    [MinLength(2, ErrorMessage = "Konu adı en az 2 karakterden oluşmalıdır.")]
    public string Name { get; set; }
}
