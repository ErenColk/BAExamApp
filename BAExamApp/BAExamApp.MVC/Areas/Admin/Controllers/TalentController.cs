using AutoMapper;
using BAExamApp.Dtos.Talents;
using BAExamApp.MVC.Areas.Admin.Models.TalentVMs;

namespace BAExamApp.MVC.Areas.Admin.Controllers;
public class TalentController : AdminBaseController
{
    private readonly ITalentService _talentService;
    private readonly IMapper _mapper;

    public TalentController(ITalentService talentService, IMapper mapper)
    {
        _talentService = talentService;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var result = await _talentService.GetAllAsync();
        var talentList = _mapper.Map<IEnumerable<AdminTalentListVM>>(result.Data.OrderBy(x => x.Name));

        return View(talentList);
    }

    [HttpGet]
    public async Task<IActionResult> Delete(Guid id)
    {
        var talentResult = await _talentService.DeleteAsync(id);

        if (!talentResult.IsSuccess)
        {            
            return Content("<script>setTimeout(function() { location.reload(); }, 2000);</script>");
        }
        else
        {            
            return Json(talentResult);
        }

    }

    [HttpPost]
    public async Task<IActionResult> Create(AdminTalentAddVM model)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(x => x.Errors);
            string errorMessages = null!;
            foreach (var error in errors)
            {
                errorMessages += " ," + error.ErrorMessage;
            }
            NotifyError(errorMessages);
            return RedirectToAction(nameof(Index));
        }

        var talent = _mapper.Map<TalentCreateDto>(model);
        var addResult = await _talentService.AddAsync(talent);
        if (!addResult.IsSuccess)
        {
            NotifyErrorLocalized(addResult.Message);
            return RedirectToAction(nameof(Index));
        }

        NotifySuccessLocalized(addResult.Message);
        return RedirectToAction(nameof(Index));
    }
}
