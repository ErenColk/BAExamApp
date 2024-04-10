using AutoMapper;
using BAExamApp.Dtos.Subjects;
using BAExamApp.MVC.Areas.Admin.Models.SubjectVMs;
using BAExamApp.MVC.Extensions;

namespace BAExamApp.MVC.Areas.Admin.Controllers;

public class SubjectController : AdminBaseController
{
    private readonly ISubjectService _subjectService;
    private readonly IMapper _mapper;
    public SubjectController(ISubjectService subjectService, IMapper mapper)
    {
        _subjectService = subjectService;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var getSubjectListResult = await _subjectService.GetAllAsync();
        var subjectList = _mapper.Map<List<AdminSubjectListVM>>(getSubjectListResult.Data);
        return View(subjectList);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(AdminSubjectCreateVM viewModel)
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

        SubjectCreateDto subjectCreateDto = _mapper.Map<SubjectCreateDto>(viewModel);

        subjectCreateDto.Name = StringExtensions.TitleFormat(viewModel.Name);

        var createSubjectResult = await _subjectService.AddAsync(subjectCreateDto);
        if (!createSubjectResult.IsSuccess)
        {
            NotifyErrorLocalized(createSubjectResult.Message);
            return RedirectToAction(nameof(Index));
        }
        NotifySuccessLocalized(createSubjectResult.Message);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Details(Guid id)
    {
        var subject = await _subjectService.GetDetailsByIdAsync(id);
        if (subject.IsSuccess)
        {
            return View(_mapper.Map<AdminSubjectDetailVM>(subject.Data));
        }
        NotifyErrorLocalized(subject.Message);
        return RedirectToAction(nameof(Index));
    }

    //[HttpGet]
    //public async Task<IActionResult> Update(Guid id)
    //{
    //    var getResult = await _subjectService.GetByIdAsync(id);
    //    if (!getResult.IsSuccess)
    //    {
    //        NotifyErrorLocalized(getResult.Message);
    //        return RedirectToAction(nameof(Index));
    //    }

    //    var updateSubjectResult = _mapper.Map<AdminSubjectUpdateVM>(getResult.Data);

    //    return View(updateSubjectResult);
    //}

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(AdminSubjectUpdateVM viewModel)
    {
        if (!ModelState.IsValid)
        {
            return View(nameof(Index));
        }

        var updateSubjectDto = _mapper.Map<SubjectUpdateDto>(viewModel);

        updateSubjectDto.Name = StringExtensions.TitleFormat(viewModel.Name);

        var updateResult = await _subjectService.UpdateAsync(updateSubjectDto);
        if (!updateResult.IsSuccess)
        {
            NotifyErrorLocalized(updateResult.Message);
        }
        else
        {
            NotifySuccessLocalized(updateResult.Message);
        }
        return RedirectToAction(nameof(Index));
    }


    [HttpGet]
    public async Task<IActionResult> Delete(Guid id)
    {
        var subjectDeleteResponse = await _subjectService.DeleteAsync(id);
        if (subjectDeleteResponse.IsSuccess)
            NotifySuccessLocalized(subjectDeleteResponse.Message);
        else
            NotifyErrorLocalized(subjectDeleteResponse.Message);

        return Json(subjectDeleteResponse);
    }

}