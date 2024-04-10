using AutoMapper;
using BAExamApp.Business.Constants;
using BAExamApp.Dtos.ExamRules;
using BAExamApp.Entities.Enums;
using BAExamApp.MVC.Areas.Admin.Models.ExamRuleSubtopicVMs;
using BAExamApp.MVC.Areas.Admin.Models.ExamRuleVMs;
using BAExamApp.MVC.Extensions;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text.Json;

namespace BAExamApp.MVC.Areas.Admin.Controllers;

public class ExamRuleController : AdminBaseController
{
    private readonly IExamRuleService _examRuleManager;
    private readonly ISubjectService _subjectManager;
    private readonly ISubtopicService _subtopicManager;
    private readonly IProductService _productManager;
    private readonly IMapper _mapper;
    private readonly IQuestionDifficultyService _questionDifficultyService;
    public ExamRuleController(IExamRuleService examRuleService, ISubjectService subjectService, IProductService productService, IMapper mapper, ISubtopicService subtopicManager, IQuestionDifficultyService questionDifficultyService)
    {
        _examRuleManager = examRuleService;
        _subjectManager = subjectService;
        _productManager = productService;
        _mapper = mapper;
        _subtopicManager = subtopicManager;
        _questionDifficultyService = questionDifficultyService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        ViewBag.ProductList = await GetProductsAsync();
        ViewBag.ExamTypes = GetExamTypes();

        var getExamListResult = await _examRuleManager.GetAllAsync();
        var examRuleList = _mapper.Map<List<AdminExamRuleListVM>>(getExamListResult.Data);

        return View(examRuleList);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(AdminExamRuleCreateVM viewModel, IFormCollection collection)
    {
        List<AdminExamRuleSubtopicCreateVM> examRuleSubtopicList = JsonSerializer.Deserialize<List<AdminExamRuleSubtopicCreateVM>>(collection["examRuleSubjects"]);
        viewModel.ExamRuleSubtopics = examRuleSubtopicList;

        if (!ModelState.IsValid && (examRuleSubtopicList.Count < 1))
        {
            NotifyErrorLocalized(Messages.PleaseAddExamRuleSubject);
        }
        else
        {
            ExamRuleCreateDto examRuleCreateDto = _mapper.Map<ExamRuleCreateDto>(viewModel);

            examRuleCreateDto.Name = StringExtensions.TitleFormat(viewModel.Name);

            var result = await _examRuleManager.AddAsync(examRuleCreateDto);
            if (!result.IsSuccess)
            {
                NotifyErrorLocalized(result.Message);
            }
            else
            {
                NotifySuccessLocalized(result.Message);
            }
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Update(Guid id)
    {
        var result = await _examRuleManager.GetByIdAsync(id);
        if (!result.IsSuccess)
        {
            NotifyErrorLocalized(result.Message);
            return RedirectToAction(nameof(Index));
        }
        var examRuleUpdateVM = _mapper.Map<AdminExamRuleUpdateVM>(result.Data);

        ViewBag.ProductList = await GetProductsAsync();
        ViewBag.examRuleSubtopicsJSON = JsonSerializer.Serialize(examRuleUpdateVM.ExamRuleSubtopics);

        return View(examRuleUpdateVM);
    }

    [HttpPost]
    public async Task<IActionResult> Update(AdminExamRuleUpdateVM model, IFormCollection collection)
    {
        List<AdminExamRuleSubtopicUpdateVM> examRuleSubtopicList = JsonSerializer.Deserialize<List<AdminExamRuleSubtopicUpdateVM>>(collection["examRuleSubjects"]);
        model.ExamRuleSubtopics = examRuleSubtopicList;

        if (!ModelState.IsValid && (examRuleSubtopicList.Count < 1))
        {
            ViewBag.ProductList = await GetProductsAsync();

            return View(model);
        }

        var examRuleUpdate = _mapper.Map<ExamRuleUpdateDto>(model);

        examRuleUpdate.Name = StringExtensions.TitleFormat(model.Name);

        var updateResult = await _examRuleManager.UpdateAsync(examRuleUpdate);

        if (!updateResult.IsSuccess)
        {
            NotifyErrorLocalized(updateResult.Message);
            return View(model);
        }

        NotifySuccessLocalized(updateResult.Message);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Details(Guid id)
    {
        var getExamRuleResponse = await _examRuleManager.GetDetailsByIdAsync(id);
        if (getExamRuleResponse.IsSuccess)
        {
            return View(_mapper.Map<AdminExamRuleDetailsVM>(getExamRuleResponse.Data));
        }
        NotifyErrorLocalized(getExamRuleResponse.Message);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleteResult = await _examRuleManager.DeleteAsync(id);
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

    [HttpGet]
    private async Task<List<SelectListItem>> GetProductsAsync()
    {
        var getProductResult = await _productManager.GetAllAsync();

        return getProductResult.Data.Select(x => new SelectListItem()
        {
            Value = x.Id.ToString(),
            Text = x.Name
        }).ToList();
    }

    [HttpGet]
    public async Task<List<SelectListItem>> GetSubjectsByProductId(string productId)
    {
        var subjectList = await _subjectManager.GetAllByProductIdAsync(Guid.Parse(productId));
        return subjectList.Data.Select(x => new SelectListItem
        {
            Text = x.Name,
            Value = x.Id.ToString()
        }).ToList();
    }

    [HttpGet]
    public async Task<List<SelectListItem>> GetSubtopicsBySubjectId(string subjectId)
    {
        var subtopicList = await _subtopicManager.GetSubtopicBySubjectId(Guid.Parse(subjectId));
        return subtopicList.Data.Select(x => new SelectListItem
        {
            Text = x.Name,
            Value = x.Id.ToString()
        }).ToList();
    }

    [HttpGet]
    public List<SelectListItem> GetQuestionTypes(string examTypeId)
    {
        if (examTypeId == "1")
        {
            return Enum.GetValues(typeof(QuestionType)).Cast<QuestionType>()
                .Where(x => x != QuestionType.Classic)
                .Select(x => new SelectListItem
                {
                    Text = Localize(x.GetType().GetMember(x.ToString()).First().GetCustomAttribute<DisplayAttribute>()!.GetName()!),
                    Value = ((int)x).ToString()
                }).ToList();
        }

        return Enum.GetValues(typeof(QuestionType)).Cast<QuestionType>().
            Select(x => new SelectListItem
            {
                Text = Localize(x.GetType().GetMember(x.ToString()).First().GetCustomAttribute<DisplayAttribute>()!.GetName()!),
                Value = ((int)x).ToString()
            }).ToList();
    }

    [HttpGet]
    public List<SelectListItem> GetExamTypes()
    {
        return Enum.GetValues(typeof(ExamType)).Cast<ExamType>().
            Select(x => new SelectListItem
            {
                Text = Localize(x.GetType().GetMember(x.ToString()).First().GetCustomAttribute<DisplayAttribute>()!.GetName()!),
                Value = ((int)x).ToString()
            }).ToList();
    }

    [HttpGet]
    public async Task<SelectList> GetQuestionDifficulties()
    {
        return new SelectList((await _questionDifficultyService.GetAllAsync()).Data, "Id", "Name");
    }

    [HttpGet]
    public async Task<string> GetSubjectNameBySubjectId(string subjectId)
    {
        var subjectName = await _subjectManager.GetByIdAsync(Guid.Parse(subjectId));
        return subjectName.Data.Name;
    }
}