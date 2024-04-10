using BAExamApp.Entities.Enums;
using Microsoft.AspNetCore.Identity;

namespace BAExamApp.MVC.Areas.Trainer.Controllers;
public class HomeController : TrainerBaseController
{
    private readonly ITrainerService _trainerManager;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly IQuestionService _questionService;

    public HomeController(ITrainerService trainerManager, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, IQuestionService questionService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _trainerManager = trainerManager;
        _questionService = questionService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var user = (await _trainerManager.GetByIdentityIdAsync(UserIdentityId)).Data;

        if (TempData["Login"] != null)
            NotifySuccess($"Hoş Geldin {user.FirstName} {user.LastName}");

        return View();
    }
    public async Task<IActionResult> LoginAsAdmin(string adminId)
    {
        var infoOfAdmin = await _userManager.FindByIdAsync(adminId);

        HttpContext.Session.Remove("changeSession");
        HttpContext.Session.Remove("adminId");

        await _signInManager.SignInAsync(infoOfAdmin!, isPersistent: false);

        return RedirectToAction("Index", "Home", new { area = "Admin" });
    }
}