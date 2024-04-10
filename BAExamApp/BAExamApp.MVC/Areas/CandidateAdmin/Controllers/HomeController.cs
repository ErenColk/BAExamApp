using BAExamApp.Core.Utilities.Results.Concrete;
using Microsoft.AspNetCore.Mvc;

namespace BAExamApp.MVC.Areas.CandidateAdmin.Controllers;
public class HomeController : CandidateAdminBaseController
{
    private readonly ICandidateAdminService _candidateAdminService;

    public HomeController(ICandidateAdminService candidateAdminService)
    {
        _candidateAdminService = candidateAdminService;
    }

    public async Task<IActionResult> Index()
    {
        var user = (await _candidateAdminService.GetByIdentityIdAsync(UserIdentityId)).Data;
        ViewBag.UserName = $"{user.FirstName} {user.LastName}";
        if (TempData["Login"] != null)
            NotifySuccess($"Hoş Geldin {user.FirstName} {user.LastName}");

        return View();
    }
}
