using AutoMapper;
using BAExamApp.Business.Constants;
using BAExamApp.Core.Enums;
using BAExamApp.Dtos.CandidateQuestionAnswers;
using BAExamApp.Dtos.CandidateQuestions;
using BAExamApp.Dtos.QuestionAnswers;
using BAExamApp.Dtos.QuestionArranges;
using BAExamApp.Dtos.QuestionRevisions;
using BAExamApp.Dtos.Questions;
using BAExamApp.Dtos.QuestionSubtopics;
using BAExamApp.Entities.Enums;
using BAExamApp.MVC.Areas.Admin.Models.QuestionVMs;
using BAExamApp.MVC.Areas.CandidateAdmin.Models.QuestionAnswerVMs;
using BAExamApp.MVC.Areas.CandidateAdmin.Models.QuestionVMs;
using BAExamApp.MVC.Areas.Trainer.Models.QuestionAnswerVMs;
using BAExamApp.MVC.Areas.Trainer.Models.QuestionVMs;
using BAExamApp.MVC.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Caching.Memory;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace BAExamApp.MVC.Areas.CandidateAdmin.Controllers;
public class QuestionController : CandidateAdminBaseController
{
    private readonly ICandidateQuestionService _candidateQuestionService;
    private readonly IMapper _mapper;
    private readonly IMemoryCache _memoryCache;
    private readonly ICandidateQuestionAnswerService _candidateQuestionAnswerService;

    public QuestionController(ICandidateQuestionService candidateQuestionService, IMapper mapper, IMemoryCache memoryCache, ICandidateQuestionAnswerService candidateQuestionAnswerService)
    {
        _candidateQuestionService = candidateQuestionService;
        _mapper = mapper;
        _memoryCache = memoryCache;
        _candidateQuestionAnswerService = candidateQuestionAnswerService;
    }


    [HttpGet]
    public async Task<IActionResult> QuestionList(bool showAllQuestions = false)
    {

        await CompleteCandidateQuestionFilters();
        ViewBag.ShowAllQuestions = showAllQuestions;
        if (showAllQuestions == true)
        {
            var questionListResult = await _candidateQuestionService.GetAllAsync();
            if (questionListResult.IsSuccess)
            {
                try
                {
                    IEnumerable<CandidateQuestionListVM> questionList = _mapper.Map<IEnumerable<CandidateQuestionListVM>>(questionListResult.Data);
                    _memoryCache.Set("filteredCandidateQuestionList", questionList, TimeSpan.FromMinutes(10));
                    NotifySuccessLocalized(questionListResult.Message);
                    return View(questionList);
                }
                catch (Exception)
                {
                    NotifyError("Listelerken bir hata oluştu!");
                }
            }
            else
            {
                NotifyErrorLocalized(questionListResult.Message);
                return View();
            }
        }

        return View();
    }



    [HttpGet]
    public async Task<IActionResult> Create()
    {

        ViewBag.CandidateQuestionType = GetCandidateQuestionTypeList();
        return View();

    }


    [HttpPost]

    public async Task<IActionResult> Create(CandidateQuestionCreateVM candidateQuestionCreateVM, IFormCollection collection)
    {

        List<CandidateQuestionAnswerCreateVM> questionAnswersList = System.Text.Json.JsonSerializer.Deserialize<List<CandidateQuestionAnswerCreateVM>>(collection["questionAnswerChoices"]);

        candidateQuestionCreateVM.QuestionAnswers = questionAnswersList;

        if (!ModelState.IsValid || questionAnswersList.Count < 1)
        {
            if (questionAnswersList.Count < 1)
            {
                NotifyErrorLocalized(Messages.AddAtLeastOneAnswer);
            }

            ViewBag.CandidateQuestionType = GetCandidateQuestionTypeList();
            ViewBag.QuestionAnswersList = System.Text.Json.JsonSerializer.Serialize(questionAnswersList);

            return View(candidateQuestionCreateVM);
        }

        var question = _mapper.Map<CandidateQuestionCreateDto>(candidateQuestionCreateVM);
        if (candidateQuestionCreateVM.Image != null)
        {
            question.Image = await candidateQuestionCreateVM.Image.FileToStringAsync();
        }

        var addResult = await _candidateQuestionService.AddAsync(question);
        if (!addResult.IsSuccess)
        {
            NotifyErrorLocalized(addResult.Message);
            return View(candidateQuestionCreateVM);
        }

        NotifySuccessLocalized(addResult.Message);
        return RedirectToAction("QuestionList", new { state = State.Awaited });

    }





    [HttpGet]
    public async Task<IActionResult> GetCandidateQuestionsByGivenValues()
    {
        _memoryCache.TryGetValue("candidateQuestionTypeList", out List<SelectListItem> candidateQuestionTypeList);
        _memoryCache.TryGetValue("filteredCandidateQuestionList", out var candidateQuestionList);
        _memoryCache.TryGetValue("content", out var content);


        if (candidateQuestionList != null)
        {
            ViewBag.CandidateQuestionTypeList = candidateQuestionTypeList;
            ViewBag.Content = content;

            return View("QuestionList", candidateQuestionList);
        }
        else
        {
            return RedirectToAction("QuestionList", "Question", new { showAllQuestions = true });
        }

    }





    [HttpGet]
    public async Task<IActionResult> Update(Guid id)
    {
        var questionResult = await _candidateQuestionService.GetByIdAsync(id);

        if (!questionResult.IsSuccess)
            return NotFound();

        CandidateQuestionUpdateVM questionUpdateVM = _mapper.Map<CandidateQuestionUpdateVM>(questionResult.Data);
        questionUpdateVM.CandidateQuestionTypeList = GetCandidateQuestionTypeList();
        questionUpdateVM.CandidateQuestionAnswersList = System.Text.Json.JsonSerializer.Serialize(questionUpdateVM.QuestionAnswers);
        questionUpdateVM.Image = questionResult.Data.Image;
        return Json(questionUpdateVM);
    }


    [HttpPost]
    public async Task<IActionResult> Update(CandidateQuestionUpdateVM candidateQuestionUpdateVM, IFormCollection collection)
    {

        List<CandidateQuestionAnswerCreateVM>? questionAnswersList = System.Text.Json.JsonSerializer.Deserialize<List<CandidateQuestionAnswerCreateVM>>(collection["questionAnswerChoices"]);

        if (ModelState.IsValid)
        {
            bool questionRevisionUpdateFailed = false;


            if (candidateQuestionUpdateVM.NewImage is not null)
            {
                candidateQuestionUpdateVM.Image = await candidateQuestionUpdateVM.NewImage.FileToStringAsync();
            }

            if (questionAnswersList.Count > 0)
            {

                foreach (var questionAnswer in questionAnswersList)
                {
                    questionAnswer.QuestionId = candidateQuestionUpdateVM.Id;
                }

                var questionAnswersResult = await _candidateQuestionAnswerService.Update(_mapper.Map<List<CandidateQuestionAnswerDto>>(questionAnswersList));
            }


            var question = _mapper.Map<CandidateQuestionUpdateDto>(candidateQuestionUpdateVM);

            var questionResult = await _candidateQuestionService.UpdateAsync(question);

            if (questionResult.IsSuccess)
            {
                NotifySuccessLocalized(questionResult.Message);
                return RedirectToAction(nameof(QuestionList));

            }
            NotifyErrorLocalized(questionResult.Message);


        }

        ViewBag.CandidateQuestionType = GetCandidateQuestionTypeList();

        return RedirectToAction(nameof(QuestionList));
    }





    [HttpPost]
    public async Task<IActionResult> GetCandidateQuestionsByGivenValues(CandidateQuestionListVM candidateQuestionListVM)
    {


        var getQuestionResponse = await _candidateQuestionService.GetQuestionBySearchValues(candidateQuestionListVM.Content, candidateQuestionListVM.CandidateQuestionType.ToString(), candidateQuestionListVM.CreatedDate.ToShortDateString());

        var questionList = _mapper.Map<List<CandidateQuestionListVM>>(getQuestionResponse.Data);




        var candidateQuestionTypeList = GetCandidateQuestionTypeList();
        var candidateQuestionTypeItem = candidateQuestionListVM.CandidateQuestionType != null ? candidateQuestionTypeList.FirstOrDefault(x => x.Value == ((int)candidateQuestionListVM.CandidateQuestionType).ToString() || x.Text == candidateQuestionListVM.CandidateQuestionType.ToString()) : null;
        if (candidateQuestionTypeItem != null)
        {
            candidateQuestionTypeItem.Selected = true;
        }

        _memoryCache.Set("candidateQuestionTypeList", candidateQuestionTypeList, TimeSpan.FromMinutes(10));
        _memoryCache.Set("content", candidateQuestionListVM.Content, TimeSpan.FromMinutes(10));
        _memoryCache.Set("filteredCandidateQuestionList", questionList, TimeSpan.FromMinutes(10));

        await CompleteCandidateQuestionFilters(candidateQuestionTypeList);

        return RedirectToAction("GetCandidateQuestionsByGivenValues");
    }


    [HttpGet]
    public async Task<IActionResult> Delete(Guid id)
    {
        var questiontDeleteResponse = await _candidateQuestionService.DeleteAsync(id);

        if (questiontDeleteResponse.IsSuccess)
            NotifySuccessLocalized(questiontDeleteResponse.Message);
        else
            NotifyErrorLocalized(questiontDeleteResponse.Message);

        return Json(questiontDeleteResponse);
    }


    [HttpGet]
    public async Task<IActionResult> QuestionDetails(Guid id)
    {

        var questionDetailResult = await _candidateQuestionService.GetDetailsByIdAsync(id);
        if (questionDetailResult.IsSuccess)
        {
            var model = _mapper.Map<CandidateQuestionDetailsVM>(questionDetailResult.Data);
            NotifySuccessLocalized(questionDetailResult.Message);
            return View(model);
        }

        NotifyErrorLocalized(questionDetailResult.Message);
        return RedirectToAction("QuestionList");
    }


    private async Task CompleteCandidateQuestionFilters(List<SelectListItem> candidateQuestionTypeList = null)
    {
        if (candidateQuestionTypeList != null)
        {
            _memoryCache.Set("candidateQuestionTypeList", candidateQuestionTypeList, TimeSpan.FromMinutes(10));
        }
        else
        {
            ViewBag.CandidateQuestionTypeList = GetCandidateQuestionTypeList();
            _memoryCache.Set("candidateQuestionTypeList", GetCandidateQuestionTypeList(), TimeSpan.FromMinutes(10));
        }
    }


    private List<SelectListItem> GetCandidateQuestionTypeList()
    {
        return Enum.GetValues(typeof(CandidateQuestionType)).Cast<CandidateQuestionType>().
             Select(x => new SelectListItem
             {
                 Text = Localize(x.GetType().GetMember(x.ToString()).First().GetCustomAttribute<DisplayAttribute>()!.GetName()!),
                 Value = ((int)x).ToString()
             }).ToList();
    }

}
