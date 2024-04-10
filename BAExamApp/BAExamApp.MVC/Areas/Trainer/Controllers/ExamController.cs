using AutoMapper;
using BAExamApp.Business.Constants;
using BAExamApp.Core.Enums;
using BAExamApp.Dtos.ExamClassrooms;
using BAExamApp.Dtos.Exams;
using BAExamApp.Dtos.SendMails;
using BAExamApp.Dtos.StudentExams;
using BAExamApp.Entities.DbSets;
using BAExamApp.Entities.Enums;
using BAExamApp.MVC.Areas.Admin.Models.ExamVMs;
using BAExamApp.MVC.Areas.Student.Models.StudentExamVMs;
using BAExamApp.MVC.Areas.Trainer.Models.ExamEvaluatorVMs;
using BAExamApp.MVC.Areas.Trainer.Models.ExamVMs;
using BAExamApp.MVC.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Caching.Memory;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Reflection;
using System.Security.Claims;

namespace BAExamApp.MVC.Areas.Trainer.Controllers;

public class ExamController : TrainerBaseController
{

    private readonly ITrainerService _trainerService;
    private readonly IExamService _examService;
    private readonly IExamRuleService _examRuleService;
    private readonly IExamRuleSubtopicService _examRuleSubjectService;
    private readonly IExamEvaluatorService _examEvaluatorService;
    private readonly IMemoryCache _memoryCache;
    private readonly IExamAnalysisService _examAnalysisService;
    private readonly IRabbitMQPublishService _rabbitMQPublishService;
    private readonly IClassroomService _classroomService;
    private readonly IQuestionService _questionService;
    private readonly IMapper _mapper;
    private readonly IStudentExamService _examStudentService;
    private readonly IStudentService _studentService;
    private readonly IStudentClassroomService _studentClassroomService;
    private readonly IStudentQuestionService _studentQuestionService;
    private readonly IStudentExamService _studentExamService;
    private readonly ISendMailService _sendMailService;
    private readonly IHttpContextAccessor? _contextAccessor;
    private readonly IUserService _userService;

    public ExamController(ITrainerService trainerService, IMapper mapper, IExamService examService, IExamRuleSubtopicService examRuleSubjectService, IExamRuleService examRuleService, IClassroomService classroomService, IQuestionService questionService, IStudentExamService examStudentService, IStudentService studentService, IStudentClassroomService studentClassroomService, IStudentQuestionService studentQuestionService, ISendMailService sendMailService, IStudentExamService studentExamService, IHttpContextAccessor? contextAccessor, IUserService userService, IExamEvaluatorService examEvaluatorService, IMemoryCache memoryCache, IExamAnalysisService examAnalysisService, IRabbitMQPublishService rabbitMQPublishService)
    {
        _trainerService = trainerService;
        _examService = examService;
        _examRuleService = examRuleService;
        _examRuleSubjectService = examRuleSubjectService;
        _mapper = mapper;
        _classroomService = classroomService;
        _questionService = questionService;
        _examStudentService = examStudentService;
        _studentService = studentService;
        _studentQuestionService = studentQuestionService;
        _sendMailService = sendMailService;
        _studentExamService = studentExamService;
        _contextAccessor = contextAccessor;
        _userService = userService;
        _examEvaluatorService = examEvaluatorService;
        _memoryCache = memoryCache;
        _examAnalysisService = examAnalysisService;
        _rabbitMQPublishService = rabbitMQPublishService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        if (_memoryCache.TryGetValue("ExamClassroomId", out var classroomId))
        {
            ViewBag.defaultClassroomId = classroomId.ToString();
        }

        var classes = await _classroomService.GetAllByIdentityIdAsync(UserIdentityId);
        var rules = await _examRuleService.GetAllAsync();
        ViewBag.className = classes.Data.Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).ToList();
        ViewBag.ruleName = rules.Data.Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).ToList();

        return View(new List<TrainerExamListVM>());
    }

    /// <summary>
    /// Bu action, getir butonuna basıldığı zaman gerekli filtrelendirmeleri yapar.
    /// </summary>
    /// <param name="className"></param>
    /// <param name="ruleName"></param>
    /// <param name="startDate"></param>
    /// <param name="endDate"></param>
    /// <param name="isActiveExams"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> GetExamsByStatus(string className, string ruleName, string startDate, string endDate, bool isActiveExams)
    {
        _memoryCache.Remove("ExamClassroomId");
        var exams = await _examService.GetExamsByFilteredByTrainer(className, ruleName, startDate, endDate, isActiveExams, UserIdentityId);

        var sortedExams = exams.Data.OrderByDescending(e => e.ExamDateTime.Year)
                              .ThenByDescending(e => e.ExamDateTime.Month)
                              .ThenByDescending(e => e.ExamDateTime.Day)
                              .Take(100)
                              .ToList();

        var examList = _mapper.Map<List<TrainerExamListVM>>(sortedExams);
        var x = await _examService.GetExamsByExamIdAsync(examList.Select(x => x.Id).ToList());
        examList = _mapper.Map<List<TrainerExamListVM>>(x.Data);
        var cacheEntryOptions = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(TimeSpan.FromMinutes(5));

        _memoryCache.Set("trainerExamList", examList, cacheEntryOptions);

        return RedirectToAction("GetExamsByStatus");
    }

    [HttpGet]
    public async Task<IActionResult> GetExamsByStatus()
    {
        if (_memoryCache.TryGetValue("trainerExamList", out var examList))
        {
            _memoryCache.TryGetValue("examStarted", out var examStartedId);

            var classes = await _classroomService.GetAllByIdentityIdAsync(UserIdentityId);
            var rules = await _examRuleService.GetAllAsync();
            ViewBag.className = classes.Data.Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).ToList();
            ViewBag.ruleName = rules.Data.Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).ToList();
            ViewBag.ExamTypes = GetExamTypes();
            ViewBag.ExamRuleList = await GetExamRules();
            ViewBag.ExamCreationTypes = GetExamCreationTypes();

            if (examStartedId != null)
            {
                var examStarted = await _examService.GetByIdAsync((Guid)examStartedId);

                foreach (var item in (List<TrainerExamListVM>)examList)
                {
                    if (item.Id.ToString() == examStarted.Data.Id.ToString())
                    {
                        item.IsStarted = true;
                    }
                }
            }
          

            return View("Index", examList);
        }
        else
        {
            return RedirectToAction("Index");
        }
    }

    /// <summary>
    /// Bu action, Index sayfasındaki seçeneklerden sınavı başlat butonuna basıldığı zaman çalışır. 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> StartExam(Guid id)
    {
        string link = Url.Action("StartExam", "exam", new { Area = "Student" }, Request.Scheme);

        var examResult = await _examService.GetStudentsInfosByExamIdAsync(id, link);
        var examStarted = await _examService.GetByIdAsync(id);

        if (!examResult.IsSuccess)
        {
            NotifyWarningLocalized(examResult.Message);
            return Json(new { success = true, message = Messages.ExamStartedMailError });
        }
        NotifySuccessLocalized(Messages.ExamStartedSuccessfully);
        _memoryCache.Set("examStarted", examStarted.Data.Id, TimeSpan.FromMinutes(10));
        return Json(new { success = true, message = Messages.ExamStartedSuccessfully });

    }



    [HttpGet]
    public async Task<IActionResult> Details(Guid id)
    {
        var examResult = await _examService.GetDetailsByIdAsync(id);

        if (!examResult.IsSuccess)
        {

            NotifyErrorLocalized(examResult.Message);
            return RedirectToAction(nameof(Index));

        }

        var examDetailVM = _mapper.Map<TrainerExamDetailVM>(examResult.Data);

        var studentExam = await _studentExamService.GetAllByExamIdAsync(id);


        if (!studentExam.IsSuccess)
        {
            NotifyError(studentExam.Message);
            return RedirectToAction(nameof(Index));
        }

        var studentExamVm = _mapper.Map<IEnumerable<StudentExamDetailForTrainerVM>>(studentExam.Data);

        foreach (var item in studentExamVm)
        {
            item.ExamType = examResult.Data.ExamType;
        }

        var combinedExamDetailsVM = new TrainerCombinedExamDetailsVM
        {
            ExamDetail = examDetailVM,
            StudentExamDetails = studentExamVm
        };

        return View(combinedExamDetailsVM);

    }

    [HttpPost]
    public async Task<IActionResult> TrainerAssessmentStudent(StudentExamAssessmentVM studentExamAssessmentVM)
    {
        if (!ModelState.IsValid)
        {
            NotifyErrorLocalized(Messages.TrainerAssessmentStudentFail);
        }
        else
        {
            var student = await _studentService.GetByIdAsync(studentExamAssessmentVM.StudentId);
            var exam = await _examService.GetByIdAsync(studentExamAssessmentVM.ExamId);

            if (!student.IsSuccess)
            {
                NotifyErrorLocalized(student.Message);
            }
            else if (!exam.IsSuccess)
            {
                NotifyErrorLocalized(exam.Message);
            }
            else
            {
                try
                {
                    _rabbitMQPublishService.EnqueueModel(new StudentAssesmentMailDto()
                    {
                        Assessment = studentExamAssessmentVM.Content,
                        ExamName = exam.Data.Name,
                        StudentEmailAddress = student.Data.Email,
                        TrainerName = studentExamAssessmentVM.TrainerName
                    }, RabbitMQQueueNames.EmailToStudentAssessment);
                    NotifySuccessLocalized(Messages.EmailSendSuccess);

                }
                catch (Exception)
                {

                    NotifyWarningLocalized(Messages.EmailSendError);
                }
            }

        }
        return RedirectToAction("Details", new { id = studentExamAssessmentVM.ExamId });
    }


    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> GetExamResult(Guid studentExamId)
    {
        var studentExamResult = await _studentExamService.GetByIdAsync(studentExamId);

        if (studentExamResult.IsSuccess)
        {
            var studentExam = studentExamResult.Data;
            var examResult = await _examService.GetByIdAsync(studentExam.ExamId);


            if (examResult.IsSuccess)
            {
                var exam = examResult.Data;
                var model = _mapper.Map<StudentStudentExamReportVM>(studentExam);
                model = _mapper.Map(exam, model);

                var studentResult = await _studentService.GetByIdAsync(studentExam.StudentId);
                if (studentResult.IsSuccess)
                {
                    model.StudentFullname = studentResult.Data.FirstName + " " + studentResult.Data.LastName;



                    try
                    {
                        StudentExamResultDto performance = await _examAnalysisService.AnalysisStudentPerformanceAsync(studentExam.StudentId, studentExam.ExamId);

                        model.SubtopicPerformances = performance.Score;
                        model.SubtopicRightAnswers = performance.RightAnswer;
                        model.SubtopicWrongAnswers = performance.WrongAnswer;
                        model.SubtopicEmptyAnswers = performance.EmptyAnswer;
                    }
                    catch (InvalidOperationException ex)
                    {

                        ModelState.AddModelError(string.Empty, ex.Message);
                        return RedirectToAction("ErrorPage");
                    }

                }

                return View(model);
            }
        }

        //Soru bulunamazsa, sınav daha önce tamamlanmış ise, sınav süresi gelmemişse veya sınav süresi geçtiyse StartExam Ekranına yönlendirir.
        return RedirectToAction("StartExam", new { id = studentExamId });
    }


    [HttpGet]
    public async Task<IActionResult> Delete(Guid id)
    {
        var examDeleteResponse = await _examService.DeleteAsync(id);

        if (examDeleteResponse.IsSuccess)
            NotifySuccessLocalized(examDeleteResponse.Message);
        else
            NotifyErrorLocalized(examDeleteResponse.Message);

        return Json(examDeleteResponse);
    }

    [HttpPost]
    public async Task<IActionResult> Update(TrainerExamUpdateVM examUpdateVM)
    {
        if (!ModelState.IsValid)
        {
            //NotifyErrorLocalized(Messages.ExamUpdateFailed);
            RedirectToAction(nameof(Index));
        }

        var examUpdateDto = _mapper.Map<ExamUpdateDto>(examUpdateVM);
        var updateExamResult = await _examService.UpdateAsync(examUpdateDto);

        if (!updateExamResult.IsSuccess)
            NotifyErrorLocalized(updateExamResult.Message);
        else
            NotifySuccessLocalized(updateExamResult.Message);

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var examRules = await GetExamRules();

        if (examRules.Count > 1)
        {
            ViewBag.ExamRuleList = examRules;
            ViewBag.ClassroomList = await GetClassroomByTrainerId(UserIdentityId);
            ViewBag.ExamCreationTypes = GetExamCreationTypes();
            ViewBag.ExamTypes = GetExamTypes();
            var trainerExamCreateVM = new TrainerExamCreateVM();
            trainerExamCreateVM.ExamDateTime = DateTime.Parse(DateTime.Now.ToString("dd.MM.yyyy HH:mm"));

            return View(trainerExamCreateVM);
        }

        NotifyWarningLocalized(Messages.PleaseAddExamRuleBefore);

        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TrainerExamCreateVM examCreateVM)
    {
        if (ModelState.IsValid)
        {
            var uniqueStudentExamIds = new HashSet<Guid>();
            var studentExams = new List<StudentExamCreateDto>();
            var examClassrooms = new List<ExamClassroomsCreateDto>();
            var examCreateDto = _mapper.Map<ExamCreateDto>(examCreateVM);

            if (examCreateVM.forClassroom && examCreateVM.ExamClassroomsIds != null)
            {
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(15));

                _memoryCache.Set("ExamClassroomId", examCreateVM.ExamClassroomsIds.First(), cacheEntryOptions);
                foreach (var classroomId in examCreateVM.ExamClassroomsIds)
                {
                    var students = await _studentService.GetAllByClassroomIdAsync(classroomId);
                    examClassrooms.Add(new() { ClassroomId = classroomId });
                    if (students.Data != null && students.Data.Count > 0)
                    {
                        foreach (var student in students.Data)
                        {
                            var exam = await _studentExamService.GetAllExamsByStudentIdAsync(student.Id);

                            if (uniqueStudentExamIds.Add(student.Id))
                            {
                                studentExams.Add(new() { StudentId = student.Id });
                            }
                        }
                    }
                    else
                        NotifyErrorLocalized(students.Message);
                }
            }
            else
            {
                if (examCreateVM.StudentIds != null && examCreateVM.StudentIds.Count > 0)
                {
                    foreach (var studentId in examCreateVM.StudentIds)
                    {
                        if (uniqueStudentExamIds.Add(studentId))
                        {
                            studentExams.Add(new() { StudentId = studentId });
                        }
                    }
                    examClassrooms = examCreateVM.ExamClassroomsIds.Select(id => new ExamClassroomsCreateDto { ClassroomId = id }).ToList();
                }
                else
                {
                    NotifyErrorLocalized(Messages.NoAvailableStudent);
                    return RedirectToAction(nameof(Create));
                }
            }
            examCreateDto.StudentExams = studentExams;
            examCreateDto.ExamClassroomsIds = examClassrooms;
            var examRule = (await _examRuleService.GetByIdAsync(examCreateVM.ExamRuleId)).Data;
            var questionListResult = await _studentQuestionService.CreateQuestionPoolForExamRuleSubtopicsAsync(examRule.ExamRuleSubtopics);

            var trainerId = _contextAccessor?.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

            var trainerEmail = await _userService.GetEmailByUserId(trainerId, Roles.Trainer);

            List<string> examDetail = new List<string>();

            if (questionListResult.IsSuccess)
            {
                var examResult = await _examService.AddAsync(examCreateDto);

                if (examResult.IsSuccess)
                {
                    var studentQuestionResult = await _studentQuestionService.AddRangeForExamIdAsync(examResult.Data.Id);

                    if (studentQuestionResult.IsSuccess)
                    {
                        var studentExamResult = await _studentExamService.GetAllByExamIdAsync(examResult.Data.Id);

                        string link = Url.Action("StartExam", "exam", new { Area = "Student" }, Request.Scheme);

                        if (studentExamResult.IsSuccess)
                        {
                            foreach (var studentExam in studentExamResult.Data)
                            {
                                var studentResult = await _studentService.GetByIdAsync(studentExam.StudentId);
                                if (studentResult.IsSuccess)
                                {
                                    var email = await _sendMailService.GetStudentEmailById(studentExam.StudentId);

                                    var classRoomResult = await _classroomService.GetAsync(x => examCreateVM.ExamClassroomsIds.Contains(x.Id) && studentExam.ClassroomNames.Contains(x.Name));

                                    examDetail.Add($"{studentExam.ExamName}" + "*?*" + $"{classRoomResult.Data.Name}" + "*?*" + $"{studentExam.ExamDateTime}" + "*?*" + $"{email}" + "*?*" + $"{link}/{studentExam.Id}");
                                }

                            }
                            _rabbitMQPublishService.EnqueueModel(new TrainerNewExamMailDto() { TrainerEmailAdress = trainerEmail, StudentContents = examDetail }, RabbitMQQueueNames.EmailToTrainerNewExam);


                        }
                        NotifySuccessLocalized(examResult.Message);
                        return RedirectToAction(nameof(Index));
                    }

                    NotifyErrorLocalized(studentQuestionResult.Message);
                }
                else
                {
                    NotifyErrorLocalized(examResult.Message);
                }
            }
            else
            {
                NotifyErrorLocalized(questionListResult.Message);
            }

            NotifySuccess(Messages.ExamCreatedSuccessfully);
        }
        ViewBag.ExamRuleList = await GetExamRules();
        ViewBag.ClassroomList = await GetClassroomByTrainerId(UserIdentityId);
        ViewBag.ExamTypes = GetExamTypes();
        ViewBag.ExamCreationTypes = GetExamCreationTypes();
        return View(examCreateVM);
    }

    private async Task<List<SelectListItem>> GetExamRules()
    {
        var getExamRulesResult = await _examRuleService.GetAllAsync();

        var examRulesList = getExamRulesResult.Data.Select(x => new SelectListItem()
        {
            Value = x.Id.ToString(),
            Text = x.Name
        }).ToList();
        return examRulesList;
    }

    private async Task<SelectList> GetClassroomByTrainerId(string id)
    {
        var getClassrooms = await _classroomService.GetAllByIdentityIdAsync(id);

        return new SelectList(getClassrooms.Data.Select(x => new SelectListItem
        {
            Value = x.Id.ToString(),
            Text = x.Name
        }), "Value", "Text");
    }

    [HttpGet]
    public async Task<SelectList> GetStudentsByClassroom(Guid classroomId)
    {
        var filteredStudents = await _studentService.GetAllByClassroomIdAsync(classroomId);
        return new SelectList(filteredStudents.Data.Select(x => new SelectListItem
        {
            Value = x.Id.ToString(),
            Text = x.FirstName + " " + x.LastName
        }), "Value", "Text");
    }

    private List<SelectListItem> GetExamCreationTypes()
    {
        return Enum.GetValues(typeof(ExamCreationType)).Cast<ExamCreationType>().
             Select(x => new SelectListItem
             {
                 Text = x.GetType().GetMember(x.ToString()).First().GetCustomAttribute<DisplayAttribute>().GetName(),
                 Value = ((int)x).ToString()
             }).ToList();
    }

    [HttpGet]
    public async Task<SelectList> GetExamRulesByExamType(string examTypeId)
    {
        return new SelectList((await _examRuleService.GetExamRulesByExamTypeAsync(examTypeId)).Data, "Id", "Name", null, "Description");
    }

    [HttpGet]
    private List<SelectListItem> GetExamTypes()
    {
        return Enum.GetValues(typeof(ExamType)).Cast<ExamType>().
            Select(x => new SelectListItem
            {
                Text = x.GetType().GetMember(x.ToString()).First().GetCustomAttribute<DisplayAttribute>()!.GetName(),
                Value = ((int)x).ToString()
            }).ToList();
    }

    [HttpGet]
    public async Task<IActionResult> TrainerEvaluatorExamList()
    {
        if (UserIdentityId != null)
        {
            var trainer = await _trainerService.GetByIdentityIdAsync(UserIdentityId);
            var exams = await _examEvaluatorService.GetAllByTrainerIdAsync(trainer.Data.Id);
            var examList = _mapper.Map<List<TrainerExamEvaluatorListVM>>(exams.Data);
            var classes = await _classroomService.GetAllByIdentityIdAsync(UserIdentityId);
            var rules = await _examRuleService.GetAllAsync();
            ViewBag.className = classes.Data.Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).ToList();
            ViewBag.ruleName = rules.Data.Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).ToList();
            var sortedExams = examList.OrderByDescending(e => e.ExamDateTime.Year)
                             .ThenByDescending(e => e.ExamDateTime.Month)
                             .ThenByDescending(e => e.ExamDateTime.Day)
                             .Take(100)
                             .ToList();
            return View(sortedExams);
        }
        else
        {
            return RedirectToAction("Index", "Home");
        }
    }
    [HttpPost]
    public async Task<IActionResult> TrainerEvaluatorExamListFilter(string className, string ruleName, string startDate, string endDate)
    {
        var classes = await _classroomService.GetAllByIdentityIdAsync(UserIdentityId);
        var rules = await _examRuleService.GetAllAsync();
        ViewBag.className = classes.Data.Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).ToList();
        ViewBag.ruleName = rules.Data.Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).ToList();

        var exams = await _examEvaluatorService.GetExamsByFiltered(UserIdentityId, className, ruleName, startDate, endDate);

        var sortedExams = exams.Data.OrderByDescending(e => e.ExamDateTime.Year)
                              .ThenByDescending(e => e.ExamDateTime.Month)
                              .ThenByDescending(e => e.ExamDateTime.Day)
                              .Take(100)
                              .ToList();

        var examList = _mapper.Map<List<TrainerExamEvaluatorListVM>>(sortedExams);
        return View("TrainerEvaluatorExamList", examList);
    }

    public async Task<TrainerExamUpdateVM> GetExam(Guid examId)
    {
        var exam = await _examService.GetByIdAsync(examId);
        var examUpdateVM = _mapper.Map<TrainerExamUpdateVM>(exam.Data);

        return examUpdateVM;
    }
}