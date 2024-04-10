﻿using AutoMapper;
using BAExamApp.Dtos.Subtopics;
using BAExamApp.MVC.Areas.Admin.Models.SubtopicVMs;
using BAExamApp.MVC.Extensions;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BAExamApp.MVC.Areas.Admin.Controllers;
public class SubtopicController : AdminBaseController
{
    private readonly ISubtopicService _subtopicService;
    private readonly ISubjectService _subjectService;
    private readonly IMapper _mapper;

    public SubtopicController(IMapper mapper,ISubtopicService subtopicService, ISubjectService subjectService)
    {
        _mapper = mapper;
        _subtopicService = subtopicService;
        _subjectService = subjectService;
    }

    // GET: SubtopicController
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        ViewBag.SubjectList = await GetSubjectsAsync();

        var getSubtopicListResult = await _subtopicService.GetAllAsync();
        var subtopicList = _mapper.Map<List<AdminSubtopicListVM>>(getSubtopicListResult.Data);
        return View(subtopicList);
    }

    // GET: SubtopicController/Details/5
    [HttpGet]
    public async Task<IActionResult> Details(Guid id)
    {
        ViewBag.SubjectList = await GetSubjectsAsync();
        var subtopic = await _subtopicService.GetDetailsByIdAsync(id);
        if (subtopic.IsSuccess)
        {
            var subtopicDetailVm = _mapper.Map<AdminSubtopicDetailVM>(subtopic.Data);
            subtopicDetailVm.SubjectName = (await _subjectService.GetByIdAsync(subtopic.Data.SubjectId)).Data.Name;
            return View(subtopicDetailVm);
        }
        NotifyErrorLocalized(subtopic.Message);
        return RedirectToAction(nameof(Index));
    }

    // GET: SubtopicController/Create
    [HttpPost]
    public async Task<IActionResult> Create(AdminSubtopicCreateVm viewModel)
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

        SubtopicCreateDto subtopicCreateDto = _mapper.Map<SubtopicCreateDto>(viewModel);

        subtopicCreateDto.Name = StringExtensions.TitleFormat(viewModel.Name);

        var createSubtopicResult = await _subtopicService.AddAsync(subtopicCreateDto);
        if (!createSubtopicResult.IsSuccess)
        {
            NotifyErrorLocalized(createSubtopicResult.Message);
            return RedirectToAction(nameof(Index));
        }
        NotifySuccessLocalized(createSubtopicResult.Message);
        return RedirectToAction(nameof(Index));
    }


    // POST: SubtopicController/Edit/5
    [HttpPost]
    public async Task<IActionResult> Update(AdminSubtopicUpdateVM viewModel)
    {
        if (!ModelState.IsValid)
        {
            return View(viewModel);
        }
        var updateSubtopicDto = _mapper.Map<SubtopicUpdateDto>(viewModel);

        updateSubtopicDto.Name = StringExtensions.TitleFormat(viewModel.Name);

        var updateResult = await _subtopicService.UpdateAsync(updateSubtopicDto);
        if (!updateResult.IsSuccess)
        {
            NotifyErrorLocalized(updateResult.Message);
            return View(viewModel);
        }
        NotifySuccessLocalized(updateResult.Message);
        return RedirectToAction(nameof(Index));
    }

    // GET: SubtopicController/Delete/5
    [HttpGet]
    public async Task<IActionResult> Delete(Guid id)
    {
        var subtopicDeleteResponse = await _subtopicService.DeleteAsync(id);
        if (subtopicDeleteResponse.IsSuccess)
        {
            NotifySuccessLocalized(subtopicDeleteResponse.Message);
        }
        else
        {
            NotifyErrorLocalized(subtopicDeleteResponse.Message);
        }
        return Json(subtopicDeleteResponse);
    }
    private async Task<SelectList> GetSubjectsAsync(Guid? subjectId = null)
    {

        var subjectList = (await _subjectService.GetAllAsync()).Data
            .GroupBy(x => x.Name)
            .Select(x => x.First());
        return new SelectList(subjectList.Select(x => new SelectListItem
        {
            Value = x.Id.ToString(),
            Text = x.Name,
            Selected = (subjectId != null ? x.Id == subjectId.Value : false)
        }).OrderBy(x => x.Text), "Value", "Text");
    }

    /// <summary>
    /// Bu action, update modalı açıldığında verileri sayfaya taşır.
    /// </summary>
    /// <param name="subtopicId"></param>
    /// <returns></returns>
    public async Task<AdminSubtopicUpdateVM> GetSubtopic(Guid subtopicId)
    {
        var subtopicFoundResult = await _subtopicService.GetSubtopicById(subtopicId);
        var subtopicUpdateVM = _mapper.Map<AdminSubtopicUpdateVM>(subtopicFoundResult.Data);
        return subtopicUpdateVM;
    }

}
