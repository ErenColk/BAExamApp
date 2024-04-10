using AutoMapper;
using BAExamApp.Dtos.GroupTypes;
using BAExamApp.MVC.Areas.Admin.Models.GroupTypeVMs;
using System.Text;

namespace BAExamApp.MVC.Areas.Admin.Controllers;

public class GroupTypeController : AdminBaseController
{
    private readonly IGroupTypeService _groupTypeService;
    private readonly IMapper _mapper;
    public GroupTypeController(IGroupTypeService groupTypeService, IMapper mapper)
    {
        _groupTypeService = groupTypeService;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var groupTypeGetResult = await _groupTypeService.GetAllAsync();
        var groupTypeList = _mapper.Map<List<AdminGroupTypeListVM>>(groupTypeGetResult.Data);
        return View(groupTypeList);
    }

    [HttpPost]
    public async Task<IActionResult> Create(AdminGroupTypeCreateVM model)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(x => x.Errors);
            var errorMessages = new StringBuilder();

            foreach (var error in errors)
            {
                if (errorMessages.Length > 0)
                {
                    errorMessages.Append(", ");

                    if (errorMessages.ToString().Contains(error.ErrorMessage))
                    {
                        continue;
                    }
                }

                errorMessages.Append(error.ErrorMessage);
            }

            NotifyError(errorMessages.ToString());
            return RedirectToAction(nameof(Index));
        }

        var addResult = await _groupTypeService.AddAsync(_mapper.Map<GroupTypeCreateDto>(model));
        if (!addResult.IsSuccess)
        {
            NotifyErrorLocalized(addResult.Message);
            return RedirectToAction(nameof(Index));
        }

        NotifySuccessLocalized(addResult.Message);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Details(Guid id)
    {
        var getGroupType = await _groupTypeService.GetByIdAsync(id);
        if (!getGroupType.IsSuccess)
        {
            NotifyErrorLocalized(getGroupType.Message);
            return RedirectToAction(nameof(Index));
        }

        return View(_mapper.Map<AdminGroupTypeDetailVM>(getGroupType.Data));
    }

    [HttpPost]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _groupTypeService.DeleteAsync(id);
        if (result.IsSuccess)
        {
            NotifySuccessLocalized(result.Message);
        }
        else
        {
            NotifyErrorLocalized(result.Message);
        }

        return Json(result);
    }

    [HttpPost]
    public async Task<IActionResult> Update(AdminGroupTypeUpdateVM groupTypeUpdateVM)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(x => x.Errors);
            var errorMessages = new StringBuilder();

            foreach (var error in errors)
            {
                if (errorMessages.Length > 0)
                {
                    errorMessages.Append(", ");

                    if (errorMessages.ToString().Contains(error.ErrorMessage))
                    {
                        continue;
                    }
                }

                errorMessages.Append(error.ErrorMessage);
            }

            NotifyError(errorMessages.ToString());
            return RedirectToAction(nameof(Index));
        }

        var groupTypeUpdateDto = _mapper.Map<GroupTypeUpdateDto>(groupTypeUpdateVM);
        var groupUpdateResponse = await _groupTypeService.UpdateAsync(groupTypeUpdateDto);
        if (!groupUpdateResponse.IsSuccess)
        {
            NotifyErrorLocalized(groupUpdateResponse.Message);
            return View(nameof(Index));
        }
        else
        {
            NotifySuccessLocalized(groupUpdateResponse.Message);
        }

        return RedirectToAction(nameof(Index));
    }
}