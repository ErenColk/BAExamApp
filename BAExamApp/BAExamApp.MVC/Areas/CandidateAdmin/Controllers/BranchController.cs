using AutoMapper;
using BAExamApp.Dtos.Branches;
using BAExamApp.Dtos.CandidateBranches;
using BAExamApp.MVC.Areas.CandidateAdmin.Models.Branch;
using Microsoft.Build.Framework;

namespace BAExamApp.MVC.Areas.CandidateAdmin.Controllers;
public class BranchController : CandidateAdminBaseController
{
    private readonly IMapper _mapper;
    private readonly ICandidateBranchService _candidateBranchService;

    public BranchController(IMapper mapper, ICandidateBranchService candidateBranchService)
    {
        _mapper = mapper;
        _candidateBranchService = candidateBranchService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var branchListDto = await _candidateBranchService.GetAllAsync();

        if (!branchListDto.IsSuccess)
        {
            NotifyErrorLocalized(branchListDto.Message);
        }

        var branchListVM = _mapper.Map<IEnumerable<CandidateBranchListVM>>(branchListDto.Data);
        return View(branchListVM);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CandidateBranchCreateVM branchCreateVM)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors);
            foreach (var error in errors)
            {
                var errorMessage = error.ErrorMessage;
                NotifyErrorLocalized(errorMessage);
            }
            return RedirectToAction(nameof(Index));
        }

        var addResult = await _candidateBranchService.CreateBranchAsync(_mapper.Map<CandidateBranchCreateDto>(branchCreateVM));
        if (!addResult.IsSuccess)
        {
            NotifyErrorLocalized(addResult.Message);
            return RedirectToAction(nameof(Index));
        }

        NotifySuccessLocalized(addResult.Message);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Update(CandidateBranchUpdateVM branchUpdateVM)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors);
            foreach (var error in errors)
            {
                var errorMessage = error.ErrorMessage;
                NotifyErrorLocalized(errorMessage);
            }
            return RedirectToAction(nameof(Index));
        }

        var addResult = await _candidateBranchService.UpdateAsync(_mapper.Map<CandidateBranchUpdateDto>(branchUpdateVM));
        if (!addResult.IsSuccess)
        {
            NotifyErrorLocalized(addResult.Message);
            return RedirectToAction(nameof(Index));
        }

        NotifySuccessLocalized(addResult.Message);
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete([FromQuery(Name = "id")] Guid id)
    {
        var deleteResult = await _candidateBranchService.DeleteAsync(id);
        if (!deleteResult.IsSuccess)
        {
            NotifyErrorLocalized(deleteResult.Message);
        }
        else
        {
            NotifySuccessLocalized(deleteResult.Message);
        }
            return Json(deleteResult);

    }
}
