using AutoMapper;
using BAExamApp.Business.Constants;
using BAExamApp.Core.Enums;
using BAExamApp.Dtos.QuestionAnswers;
using BAExamApp.Dtos.QuestionRevisions;
using BAExamApp.Dtos.Questions;
using BAExamApp.Dtos.QuestionSubtopics;
using BAExamApp.Dtos.Trainers;
using BAExamApp.Entities.DbSets;
using BAExamApp.Entities.Enums;
using BAExamApp.MVC.Areas.Admin.Models.QuestionRevisionVMs;
using BAExamApp.MVC.Areas.Trainer.Models.QuestionAnswerVMs;
using BAExamApp.MVC.Areas.Trainer.Models.QuestionRevisionVMs;
using BAExamApp.MVC.Areas.Trainer.Models.QuestionVMs;
using BAExamApp.MVC.Extensions;
using ExcelDataReader;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace BAExamApp.MVC.Areas.Trainer.Controllers;

public class QuestionController : TrainerBaseController
{
    private readonly IQuestionService _questionService;
    private readonly IMapper _mapper;
    private readonly ISubjectService _subjectManager;
    private readonly ITrainerService _trainerService;
    private readonly IAdminService _adminService;
    private readonly IQuestionRevisionService _questionRevisionService;
    private readonly IQuestionDifficultyService _questionDifficultyManager;
    private readonly IProductService _productManager;
    private readonly ISubtopicService _subtopicManager;
    private readonly IQuestionAnswerService _questionAnswerService;

    public QuestionController(IMapper mapper, IQuestionService questionService, ISubjectService subjectManager, ITrainerService trainerService, IAdminService adminService, IQuestionRevisionService questionRevisionService, IProductService productManager, ISubtopicService subtopicManager, IQuestionDifficultyService questionDifficultyManager, IQuestionAnswerService questionAnswerService)
    {
        _mapper = mapper;
        _trainerService = trainerService;
        _adminService = adminService;
        _productManager = productManager;
        _subjectManager = subjectManager;
        _subtopicManager = subtopicManager;
        _questionService = questionService;
        _questionRevisionService = questionRevisionService;
        _questionAnswerService = questionAnswerService;
        _questionDifficultyManager = questionDifficultyManager;
    }

    [HttpGet]
    public async Task<IActionResult> QuestionList(State state)
    {
        ViewBag.QuestionState = state;
        var questionListResult = await _questionService.GetAllByStateAndTrainerIdAsync(UserIdentityId!, UserId!, state);

        var questionList = _mapper.Map<IEnumerable<TrainerQuestionListVM>>(questionListResult.Data);

        if (state == State.Reviewed)
        {
            foreach (var question in questionList)
            {
                question.RequestDescription = (await _questionRevisionService.GetActiveByQuestionId(question.Id)).RequestDescription;
            }
        }

        return View(questionList);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        ViewBag.ProductList = await GetProductsAsync();
        ViewBag.QuestionTypes = GetQuestionTypes();

        TrainerQuestionCreateVM trainerQuestionCreateVm = new TrainerQuestionCreateVM();

        return View(trainerQuestionCreateVm);
    }

    [HttpPost]
    public async Task<IActionResult> Create(TrainerQuestionCreateVM createQuestionVM, IFormCollection collection)
    {
        List<TrainerQuestionAnswerCreateVM> questionAnswersList = System.Text.Json.JsonSerializer.Deserialize<List<TrainerQuestionAnswerCreateVM>>(collection["questionAnswerChoices"]);
        createQuestionVM.QuestionAnswers = questionAnswersList;

        if (!ModelState.IsValid || questionAnswersList.Count < 1)
        {
            if (questionAnswersList.Count < 1)
            {
                NotifyErrorLocalized(Messages.AddAtLeastOneAnswer);
            }

            ViewBag.ProductList = await GetProductsAsync();
            ViewBag.QuestionTypes = GetQuestionTypes();
            ViewBag.SubjectId = createQuestionVM.SubjectId;
            ViewBag.SubtopicId = createQuestionVM.SubtopicId;
            ViewBag.QuestionAnswersList = System.Text.Json.JsonSerializer.Serialize(questionAnswersList);

            return View(createQuestionVM);
        }

        var question = _mapper.Map<QuestionCreateDto>(createQuestionVM);
        if (createQuestionVM.Image != null)
        {
            question.Image = await createQuestionVM.Image.FileToStringAsync();
        }
        question.SubtopicId = createQuestionVM.SubtopicId.Select(subtopicId => new QuestionSubtopicsCreateDto { SubtopicId = subtopicId }).ToList();

        var addResult = await _questionService.AddAsync(question);
        if (!addResult.IsSuccess)
        {
            NotifyErrorLocalized(addResult.Message);
            return View(createQuestionVM);
        }

        NotifySuccessLocalized(addResult.Message);
        return RedirectToAction("QuestionList", new { state = State.Awaited });
    }

    [HttpGet]
    public async Task<IActionResult> Update(Guid id)
    {
        var questionResult = await _questionService.GetByIdAsync(id);

        if (!questionResult.IsSuccess)
            return NotFound();

        TrainerQuestionUpdateVM questionUpdateVM = _mapper.Map<TrainerQuestionUpdateVM>(questionResult.Data);

        if (await _questionRevisionService.AnyActive(id))
            questionUpdateVM.ReviewComment = (await _questionRevisionService.GetActiveByQuestionId(id)).RequestDescription;

        var productList = (await _productManager.GetAllBySubjectIdAsync(questionUpdateVM.SubjectId)).Data.ToList();
        questionUpdateVM.ProductId = productList.FirstOrDefault().Id;

        questionUpdateVM.ProductList = await GetProductsAsync();
        questionUpdateVM.QuestionTypeList = GetQuestionTypes();
        questionUpdateVM.SubjectList = await GetSubjectsByProductId(questionUpdateVM.ProductId.ToString());
        questionUpdateVM.SubtopicList = await GetSubtopicsBySubjectId(questionUpdateVM.SubjectId.ToString());
        questionUpdateVM.QuestionDifficultyList = await GetQuestionDifficulties();
        questionUpdateVM.QuestionAnswersList = System.Text.Json.JsonSerializer.Serialize(questionUpdateVM.QuestionAnswers);
        var timeGiven = await GetQuestionTimeByDifficultyId(questionUpdateVM.QuestionDifficultyId.ToString());
        questionUpdateVM.TimeGiven = timeGiven;
        questionUpdateVM.Image = questionResult.Data.Image;
        ViewBag.QuestionDifficultiesUpdate = await GetQuestionDifficultiesUpdateAsync();
        return Json(questionUpdateVM);
    }

    [HttpPost]
    public async Task<IActionResult> Update(TrainerQuestionUpdateVM updateQuestionVM, IFormCollection collection)
    {

        List<TrainerQuestionAnswerCreateVM>? questionAnswersList = System.Text.Json.JsonSerializer.Deserialize<List<TrainerQuestionAnswerCreateVM>>(collection["questionAnswerChoices"]);

        if (ModelState.IsValid)
        {

            if (questionAnswersList.Count >= 1)
            {
                bool questionRevisionUpdateFailed = false;


                if (updateQuestionVM.NewImage is not null)
                {
                    updateQuestionVM.Image = await updateQuestionVM.NewImage.FileToStringAsync();
                }


                if (updateQuestionVM.State == State.Reviewed)
                {
                    var questionRevision = await _questionRevisionService.GetActiveByQuestionId(updateQuestionVM.Id);

                    questionRevision.RevisionConclusion = updateQuestionVM.ReviseComment;
                    questionRevision.Status = Status.Modified;

                    var questionRevisionResult = await _questionRevisionService.UpdateAsync(_mapper.Map<QuestionRevisionUpdateDto>(questionRevision));

                    if (!questionRevisionResult.IsSuccess)
                    {
                        questionRevisionUpdateFailed = true;
                        NotifyErrorLocalized(questionRevisionResult.Message);
                    }
                }
                if (!questionRevisionUpdateFailed)
                {
                    updateQuestionVM.State = State.Awaited;
                    var question = _mapper.Map<QuestionUpdateDto>(updateQuestionVM);

                    question.SubtopicId = updateQuestionVM.SubtopicId.Select(subtopicId => new QuestionSubtopicsUpdateDto { SubtopicId = subtopicId }).ToList();
                    var questionResult = await _questionService.UpdateAsync(question);

                    if (questionResult.IsSuccess)
                    {
                        foreach (var questionAnswer in questionAnswersList)
                        {
                            questionAnswer.QuestionId = updateQuestionVM.Id;
                        }

                        var questionAnswersResult = await _questionAnswerService.UpdateRangeAsync(_mapper.Map<List<QuestionAnswerCreateDto>>(questionAnswersList));

                        if (questionAnswersResult.IsSuccess)
                        {
                            NotifySuccessLocalized(questionResult.Message);
                            return RedirectToAction("QuestionList", new { state = State.Awaited });
                        }
                        NotifySuccessLocalized(questionAnswersResult.Message);
                    }
                    NotifyErrorLocalized(questionResult.Message);
                }
            }
            else
                NotifyErrorLocalized(Messages.AddAtLeastOneAnswer);
        }

        ViewBag.ProductList = await GetProductsAsync();
        ViewBag.QuestionTypes = GetQuestionTypes();
        ViewBag.SubjectId = updateQuestionVM.SubjectId;
        ViewBag.SubtopicId = updateQuestionVM.SubtopicId;
        ViewBag.QuestionAnswersList = System.Text.Json.JsonSerializer.Serialize(questionAnswersList);

        return RedirectToAction(nameof(QuestionList), new { state = updateQuestionVM.State });
    }

    [HttpGet]
    public async Task<IActionResult> UpdateReviewed(Guid id)
    {
        var questionResult = await _questionService.GetByIdAsync(id);

        if (!questionResult.IsSuccess)
            return NotFound();

        TrainerQuestionUpdateVM questionUpdateVM = _mapper.Map<TrainerQuestionUpdateVM>(questionResult.Data);

        if (await _questionRevisionService.AnyActive(id))
            questionUpdateVM.ReviewComment = (await _questionRevisionService.GetActiveByQuestionId(id)).RequestDescription;

        var productList = (await _productManager.GetAllBySubjectIdAsync(questionUpdateVM.SubjectId)).Data.ToList();
        questionUpdateVM.ProductId = productList.FirstOrDefault().Id;

        questionUpdateVM.ProductList = await GetProductsAsync();
        questionUpdateVM.QuestionTypeList = GetQuestionTypes();
        questionUpdateVM.SubjectList = await GetSubjectsByProductId(questionUpdateVM.ProductId.ToString());
        questionUpdateVM.SubtopicList = await GetSubtopicsBySubjectId(questionUpdateVM.SubjectId.ToString());
        questionUpdateVM.QuestionDifficultyList = await GetQuestionDifficulties();
        questionUpdateVM.QuestionAnswersList = System.Text.Json.JsonSerializer.Serialize(questionUpdateVM.QuestionAnswers);
        var timeGiven = await GetQuestionTimeByDifficultyId(questionUpdateVM.QuestionDifficultyId.ToString());
        questionUpdateVM.TimeGiven = timeGiven;
        questionUpdateVM.Image = questionResult.Data.Image;
        ViewBag.QuestionDifficultiesUpdate = await GetQuestionDifficultiesUpdateAsync();
        return Json(questionUpdateVM);
    }

    [HttpPost]
    public async Task<IActionResult> UpdateReviewed(TrainerQuestionUpdateVM updateQuestionVM, IFormCollection collection)
    {

        List<TrainerQuestionAnswerCreateVM>? questionAnswersList = System.Text.Json.JsonSerializer.Deserialize<List<TrainerQuestionAnswerCreateVM>>(collection["questionAnswerChoicesReviewed"]);

        if (ModelState.IsValid)
        {

            if (questionAnswersList.Count >= 1)
            {
                bool questionRevisionUpdateFailed = false;


                if (updateQuestionVM.NewImage is not null)
                {
                    updateQuestionVM.Image = await updateQuestionVM.NewImage.FileToStringAsync();
                }


                if (updateQuestionVM.State == State.Reviewed)
                {
                    var questionRevision = await _questionRevisionService.GetActiveByQuestionId(updateQuestionVM.Id);

                    questionRevision.RevisionConclusion = updateQuestionVM.ReviseComment;
                    questionRevision.Status = Status.Modified;

                    var questionRevisionResult = await _questionRevisionService.UpdateAsync(_mapper.Map<QuestionRevisionUpdateDto>(questionRevision));

                    if (!questionRevisionResult.IsSuccess)
                    {
                        questionRevisionUpdateFailed = true;
                        NotifyErrorLocalized(questionRevisionResult.Message);
                    }
                }
                if (!questionRevisionUpdateFailed)
                {
                    updateQuestionVM.State = State.Awaited;
                    var question = _mapper.Map<QuestionUpdateDto>(updateQuestionVM);

                    question.SubtopicId = updateQuestionVM.SubtopicId.Select(subtopicId => new QuestionSubtopicsUpdateDto { SubtopicId = subtopicId }).ToList();
                    var questionResult = await _questionService.UpdateAsync(question);

                    if (questionResult.IsSuccess)
                    {
                        foreach (var questionAnswer in questionAnswersList)
                        {
                            questionAnswer.QuestionId = updateQuestionVM.Id;
                        }

                        var questionAnswersResult = await _questionAnswerService.UpdateRangeAsync(_mapper.Map<List<QuestionAnswerCreateDto>>(questionAnswersList));

                        if (questionAnswersResult.IsSuccess)
                        {
                            NotifySuccessLocalized(questionResult.Message);
                            return RedirectToAction("QuestionList", new { state = State.Awaited });
                        }
                        NotifySuccessLocalized(questionAnswersResult.Message);
                    }
                    NotifyErrorLocalized(questionResult.Message);
                }
            }
            else
                NotifyErrorLocalized(Messages.AddAtLeastOneAnswer);
        }

        foreach (var modelState in ModelState.Values)
        {
            foreach (var error in modelState.Errors)
            {
                Console.WriteLine(error.ErrorMessage);
            }
        }


        ViewBag.ProductList = await GetProductsAsync();
        ViewBag.QuestionTypes = GetQuestionTypes();
        ViewBag.SubjectId = updateQuestionVM.SubjectId;
        ViewBag.SubtopicId = updateQuestionVM.SubtopicId;
        ViewBag.QuestionAnswersList = System.Text.Json.JsonSerializer.Serialize(questionAnswersList);

        return RedirectToAction(nameof(QuestionList), new { state = updateQuestionVM.State });
    }

    [HttpGet]
    public async Task<IActionResult> CopyQuestion(Guid id)
    {
        var questionResult = await _questionService.GetByIdAsync(id);

        if (!questionResult.IsSuccess)
            return NotFound();

        TrainerQuestionCreateVM questionCreateVM = _mapper.Map<TrainerQuestionCreateVM>(questionResult.Data);

        var productList = (await _productManager.GetAllBySubjectIdAsync(questionCreateVM.SubjectId)).Data.ToList();
        questionCreateVM.ProductId = productList.FirstOrDefault().Id;

        ViewBag.ProductList = await GetProductsAsync();
        ViewBag.QuestionTypes = GetQuestionTypes();
        ViewBag.QuestionDifficulty = await GetQuestionDifficulties();
        ViewBag.SubjectId = questionCreateVM.SubjectId;
        ViewBag.SubtopicId = questionCreateVM.SubtopicId;
        ViewBag.QuestionAnswersList = System.Text.Json.JsonSerializer.Serialize(questionCreateVM.QuestionAnswers);

        return View("Create", questionCreateVM);
    }

    [HttpGet]
    public async Task<IActionResult> Details(Guid id)
    {
        var questionDetailResult = await _questionService.GetDetailsByIdAsync(id);

        if (!questionDetailResult.IsSuccess)
            return NotFound();

        var questionDetails = _mapper.Map<TrainerQuestionDetailsVM>(questionDetailResult.Data);

        if (await _questionRevisionService.AnyActive(id))
            questionDetails.ReviewComment = (await _questionRevisionService.GetActiveByQuestionId(id)).RequestDescription;

        return View(questionDetails);
    }

    [HttpGet]
    public async Task<IActionResult> Delete(Guid id)
    {
        var questiontDeleteResponse = await _questionService.DeleteAsync(id);

        if (questiontDeleteResponse.IsSuccess)
            NotifySuccessLocalized(questiontDeleteResponse.Message);
        else
            NotifyErrorLocalized(questiontDeleteResponse.Message);

        return Json(questiontDeleteResponse);
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
    public List<SelectListItem> GetQuestionTypes()
    {
        return Enum.GetValues(typeof(QuestionType)).Cast<QuestionType>().
             Select(x => new SelectListItem
             {
                 Text = Localize(x.GetType().GetMember(x.ToString()).First().GetCustomAttribute<DisplayAttribute>()!.GetName()!),
                 Value = ((int)x).ToString()
             }).ToList();
    }

    [HttpGet]
    public async Task<SelectList> GetQuestionDifficulties()
    {
        return new SelectList((await _questionDifficultyManager.GetAllAsync()).Data, "Id", "Name");
    }

    [HttpGet]
    public async Task<List<SelectListItem>> GetQuestionDifficultiesUpdateAsync()
    {
        var getQuestionDifficultyResult = await _questionDifficultyManager.GetAllAsync();

        return getQuestionDifficultyResult.Data.Select(x => new SelectListItem()
        {
            Value = x.Id.ToString(),
            Text = x.Name
        }).ToList();
    }


    [HttpGet]
    public async Task<TimeSpan> GetQuestionTimeByDifficultyId(string difficultyId)
    {
        return (await _questionDifficultyManager.GetDetailsByIdAsync(Guid.Parse(difficultyId))).Data.TimeGiven;
    }

    //public IActionResult DownloadExcel(string fileName)
    //{
    //    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Uploads");

    //    var filePath = Path.Combine(uploadsFolder, fileName);

    //    if (System.IO.File.Exists(filePath))
    //    {
    //        byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
    //        return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
    //    }

    //    return NotFound();
    //}

    //[HttpPost]
    //public async Task<IActionResult> ReadExcel(IFormFile file)
    //{
    //    var questionsList = new List<TrainerQuestionsCreateVM>();
    //    //upload için
    //    if (file != null && file.Length > 0)
    //    {
    //        var uploadsFolder = $"{Directory.GetCurrentDirectory()}\\wwwroot\\Uploads\\";

    //        if (!Directory.Exists(uploadsFolder))
    //        {
    //            Directory.CreateDirectory(uploadsFolder);
    //        }

    //        var filePath = Path.Combine(uploadsFolder, file.FileName);

    //        using (var stream = new FileStream(filePath, FileMode.Create))
    //        {
    //            await file.CopyToAsync(stream);
    //        }

    //        //table için
    //        using (var stream = System.IO.File.Open(filePath, FileMode.Open, FileAccess.Read))
    //        {
    //            using (var reader = ExcelReaderFactory.CreateReader(stream))
    //            {
    //                // İlk satırı atlamak için
    //                reader.Read();

    //                while (reader.Read())
    //                {

    //                    TrainerQuestionsCreateVM trainerQuestionsCreateVm = new TrainerQuestionsCreateVM();

    //                    trainerQuestionsCreateVm.QuestionType = reader.GetValue(0).ToString();
    //                    trainerQuestionsCreateVm.Content = reader.GetValue(1).ToString();
    //                    trainerQuestionsCreateVm.Target = reader.GetValue(2).ToString();
    //                    trainerQuestionsCreateVm.Gains = reader.GetValue(3).ToString();
    //                    trainerQuestionsCreateVm.ProductId = Guid.Parse(reader.GetValue(4).ToString());
    //                    trainerQuestionsCreateVm.SubjectId = Guid.Parse(reader.GetValue(5).ToString());
    //                    trainerQuestionsCreateVm.SubtopicId = Guid.Parse(reader.GetValue(6).ToString());
    //                    trainerQuestionsCreateVm.QuestionDifficultyId = Guid.Parse(reader.GetValue(7).ToString());
    //                    trainerQuestionsCreateVm.TimeGiven = TimeSpan.Parse(reader.GetValue(8).ToString());

    //                    for (int column = 9; column < 19; column += 2)
    //                    {
    //                        if (reader.GetValue(column) != null)
    //                        {
    //                            TrainerQuestionAnswerCreateVM trainerQuestionAnswerCreateVM = new TrainerQuestionAnswerCreateVM()
    //                            {
    //                                IsRightAnswer = Convert.ToBoolean(reader.GetValue(column).ToString()),
    //                                Answer = reader.GetValue(column + 1).ToString(),
    //                            };
    //                            trainerQuestionsCreateVm.QuestionAnswers.Add(trainerQuestionAnswerCreateVM);
    //                        }
    //                    }
    //                    questionsList.Add(trainerQuestionsCreateVm);
    //                }
    //                while (reader.NextResult()) ;
    //            }
    //            var serializedData = JsonConvert.SerializeObject(questionsList);
    //            TempData["QuestionList"] = serializedData;

    //        }
    //        System.IO.File.Delete(filePath);
    //    }
    //    return RedirectToAction("QuestionTable");
    //}

    //[HttpGet]
    //public async Task<IActionResult> QuestionTable()
    //{
    //    var serializedData = TempData["QuestionList"] as string;
    //    var questionsList = JsonConvert.DeserializeObject<List<TrainerQuestionsCreateVM>>(serializedData);
    //    TempData["QuestionList"] = serializedData;
    //    return View(questionsList);
    //}

    //[HttpPost]
    //public async Task<IActionResult> SaveExcelList([FromBody]object formData)
    //{
    //    var serializedData = formData.ToString();
    //    var questionsList = JsonConvert.DeserializeObject<List<TrainerQuestionsCreateVM>>(serializedData);

    //    foreach (var question in questionsList)
    //    {
    //        FormFileExtensions.TryParseQuestionType(question.QuestionType, out QuestionType questionType);
    //        var questionDto = _mapper.Map<QuestionCreateDto>(question);
    //        questionDto.QuestionType = questionType;

    //        var addResult = await _questionService.AddAsync(questionDto);

    //        if (!addResult.IsSuccess)
    //        {
    //            NotifyErrorLocalized(addResult.Message);
    //            return View(questionsList);
    //        }
    //        NotifySuccessLocalized(addResult.Message);
    //    }
    //    return RedirectToAction("QuestionList", new { state = State.Awaited });
    //}    

    [HttpGet]
    public async Task<IActionResult> QuestionRevisionList(Guid id)
    {
        var questionRevisions = await _questionRevisionService.GetAllByQuestionId(id);

        if (questionRevisions.Any())
        {
            var questionRevisionsVM = _mapper.Map<IEnumerable<TrainerQuestionRevisionListVM>>(questionRevisions);
            return View(questionRevisionsVM);
        }
        else
        {
            // Revize işlemi yoksa, bir bilgi mesajı döndür
            return Content("Revize işlemi yoktur");
        }
    }
}