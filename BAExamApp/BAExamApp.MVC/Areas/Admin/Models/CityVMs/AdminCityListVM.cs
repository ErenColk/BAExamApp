using System.ComponentModel.DataAnnotations;

namespace BAExamApp.MVC.Areas.Admin.Models.CityVMs;

public class AdminCityListVM
{
    public Guid Id { get; set; }

    [Display(Name = "City_Name")]
    public string Name { get; set; }
}