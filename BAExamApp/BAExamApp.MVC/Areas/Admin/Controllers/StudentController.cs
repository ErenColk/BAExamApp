using AutoMapper;
using BAExamApp.Business.Constants;
using BAExamApp.Dtos.Emails;
using BAExamApp.Dtos.SendMails;
using BAExamApp.Dtos.Students;
using BAExamApp.Entities.DbSets;
using BAExamApp.MVC.Areas.Admin.Models.ExamVMs;
using BAExamApp.MVC.Areas.Admin.Models.SentMailVMs;
using BAExamApp.MVC.Areas.Admin.Models.StudentVMs;
using BAExamApp.MVC.Areas.Admin.Models.TrainerVMs;
using BAExamApp.MVC.Areas.Student.Models.StudentExamVMs;
using BAExamApp.MVC.Extensions;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BAExamApp.MVC.Areas.Admin.Controllers;

public class StudentController : AdminBaseController
{
    private readonly IStudentService _studentService;
    private readonly ICityService _cityService;
    private readonly ISendMailService _sendMailService;
    private readonly IEmailService _emailService;
    private readonly IStudentClassroomService _studentClassroomService;
    private readonly IMapper _mapper;
    private readonly IStudentExamService _studentExamService;
    private readonly ITrainerClassroomService _trainerClassroomService;
    private readonly IClassroomService _classroomService;
    private readonly IExamService _examService;
    private readonly IExamAnalysisService _examAnalysisService;
    private readonly ISentMailService _sentMailService;
    private readonly IRabbitMQPublishService _rabbitMQPublishService;

    public StudentController(IStudentService studentService, ICityService cityService, IMapper mapper, ISendMailService sendMailService, IEmailService emailService, IStudentClassroomService studentClassroomService, IStudentExamService studentExamService, ITrainerClassroomService trainerClassroomService, IClassroomService classroomService, IExamService examService, IExamAnalysisService examAnalysisService, ISentMailService sentMailService,IRabbitMQPublishService rabbitMQPublishService)
    {
        _studentService = studentService;
        _cityService = cityService;
        _mapper = mapper;
        _sendMailService = sendMailService;
        _emailService = emailService;
        _studentClassroomService = studentClassroomService;
        _studentExamService = studentExamService;
        _trainerClassroomService = trainerClassroomService;
        _classroomService = classroomService;
        _examService = examService;
        _examAnalysisService = examAnalysisService;
        _sentMailService = sentMailService;
        _rabbitMQPublishService = rabbitMQPublishService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(bool showAllActiveStudents = false)
    {
        ViewBag.Cities = await GetCitiesAsync();

        List<AdminStudentListVM> students = new List<AdminStudentListVM>();

        if (showAllActiveStudents)
        {
            var activeStudents = await _studentService.GetActiveStudentsAsync();
            students = _mapper.Map<List<AdminStudentListVM>>(activeStudents.Data);
        }

        ViewBag.ShowAllActiveStudents = showAllActiveStudents;

        return View(students);
    }

    [HttpPost]
    public async Task<IActionResult> GetStudents(AdminStudentListVM adminStudentListVM)
    {
        ViewBag.Cities = await GetCitiesAsync();
        var getStudentResponse = await _studentService.GetStudentsByNameOrSurnameOrMailAdressAsync(adminStudentListVM.FirstName, adminStudentListVM.LastName, adminStudentListVM.Email);
        var studentList = _mapper.Map<List<AdminStudentListVM>>(getStudentResponse.Data);
        return View("Index", studentList);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        return View(new AdminStudentCreateVM()
        {
            Cities = await GetCitiesAsync()
        });
    }

    [HttpPost]
    public async Task<IActionResult> Create(AdminStudentCreateVM studentCreateVM, IFormCollection collection)
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

        var studentCreateDto = _mapper.Map<StudentCreateDto>(studentCreateVM);
        if (studentCreateVM.NewImage != null)
        {
            studentCreateDto.Image = await studentCreateVM.NewImage.FileToStringAsync();
        }

        // Servise göndermeden önce düzeltme işleminin yapıldığı yer

        studentCreateDto.FirstName = StringExtensions.TitleFormat(studentCreateVM.FirstName);
        studentCreateDto.LastName = StringExtensions.TitleFormat(studentCreateVM.LastName);

        var addSutdentresult = await _studentService.AddAsync(studentCreateDto);
        if (!addSutdentresult.IsSuccess)
        {
            NotifyErrorLocalized(addSutdentresult.Message);
            return RedirectToAction(nameof(Index));
        }

        var studentOtherEmailList = new List<EmailCreateDto>();
        var otherEmailsList = collection["otherEmails"].ToList();
        

        foreach (var studentOtherEmail in otherEmailsList)
        {
            studentOtherEmailList.Add(new EmailCreateDto() { EmailAddress = studentOtherEmail });
        }

        var addEmailResult = await _emailService.AddRangeAsync(studentOtherEmailList);

        if (!addEmailResult.IsSuccess)
        {
            NotifyErrorLocalized(addEmailResult.Message);
            return RedirectToAction(nameof(Index));
        }
        string link = Url.Action("index", "login", new { Area = "" }, Request.Scheme);

        if(addSutdentresult.IsSuccess)
        {
            NotifySuccessLocalized(addSutdentresult.Message);
            return RedirectToAction("Index");
        }
        else
        {
            NotifyErrorLocalized(addSutdentresult.Message);
            return RedirectToAction("Index");
        }
    }

    [HttpGet]
    public async Task<IActionResult> Update(Guid id)
    {
        var getStudentResult = await _studentService.GetByIdAsync(id);

        if (!getStudentResult.IsSuccess)
        {
            NotifyErrorLocalized(getStudentResult.Message);
            return RedirectToAction(nameof(Index));
        }
        var studentDto = getStudentResult.Data;
        var studentUpdateVM = _mapper.Map<AdminStudentUpdateVM>(studentDto);
        studentUpdateVM.OtherEmails = (await _emailService.GetEmailAddressesByIdentityIdAsync(getStudentResult.Data.IdentityId)).Data;
        studentUpdateVM.Cities = await GetCitiesAsync(studentUpdateVM.CityId);
        return PartialView("Update", studentUpdateVM);
    }

    [HttpPost]
    public async Task<IActionResult> Update(AdminStudentUpdateVM model, IFormCollection collection)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var updateStudent = _mapper.Map<StudentUpdateDto>(model);

        if (model.NewImage != null)
        {
            updateStudent.Image = await model.NewImage.FileToStringAsync();
        }


        updateStudent.FirstName = StringExtensions.TitleFormat(model.FirstName);
        updateStudent.LastName = StringExtensions.TitleFormat(model.LastName);
        var updateStudentResult = await _studentService.UpdateAsync(updateStudent);

        if (!updateStudentResult.IsSuccess)
        {
            NotifyErrorLocalized(updateStudentResult.Message);
            return RedirectToAction(nameof(Update));
        }

        var otherEmailsList = collection["otherEmails"].ToList();
        var studentOtherEmailList = new List<EmailCreateDto>();
        var identityId = updateStudentResult.Data.IdentityId;

        foreach (var studentOtherEmail in otherEmailsList)
        {
            studentOtherEmailList.Add(new EmailCreateDto() { EmailAddress = studentOtherEmail, IdentityId = identityId });
        }
        var addEmailResult = await _emailService.UpdateRangeAsync(studentOtherEmailList, identityId);

        NotifySuccessLocalized(updateStudentResult.Message);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Details(Guid id)
    {
        ViewBag.Cities = await GetCitiesAsync();

        var getStudent = await _studentService.GetStudentDetailsByIdAsync(id);
        if (getStudent.IsSuccess)
        {
            var studentDetailsVM = _mapper.Map<AdminStudentDetailVM>(getStudent.Data);
            studentDetailsVM.OtherEmails = (await _emailService.GetEmailAddressesByIdentityIdAsync(getStudent.Data.IdentityId)).Data;
            studentDetailsVM.Classrooms = (await _studentClassroomService.GetStudetClassroomIdentityIdAsync(getStudent.Data.Id)).Data;
            return View(studentDetailsVM);
        }
        NotifyErrorLocalized(getStudent.Message);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Delete([FromQuery(Name = "id")] Guid id)
    {
        var deleteResult = await _studentService.DeleteAsync(id);
        if (deleteResult.IsSuccess)
            NotifySuccessLocalized(deleteResult.Message);
        else
            NotifyErrorLocalized(deleteResult.Message);

        return Json(deleteResult);

    }

    /// <summary>
    /// Şehirleri liste olarak getirir
    /// </summary>
    /// <param name="cityId"></param>
    /// <returns>Parametre ile kullanılırsa parametre verisine göre seçili dönüş yapar, para metre yok ise seçilmeden dönüş yapar </returns>
    private async Task<SelectList> GetCitiesAsync(Guid? cityId = null)
    {
        var cityList = (await _cityService.GetAllAsync()).Data;
        return new SelectList(cityList.Select(x => new SelectListItem
        {
            Value = x.Id.ToString(),
            Text = x.Name,
            Selected = x.Id == (cityId != null ? cityId.Value : cityId)
        }).OrderBy(x => x.Text), "Value", "Text");
    }


    [HttpGet]
    public async Task<IActionResult> StudentExams(Guid id)
    {
        var getStudentExams = await _studentExamService.GetAllExamsWithDetailsByIdAsync(id);

        if (getStudentExams.IsSuccess)
        {
            var studentExamsVM = _mapper.Map<IEnumerable<StudentExamsForAdminVM>>(getStudentExams.Data);

            return View(studentExamsVM);
        }
        NotifyErrorLocalized(getStudentExams.Message);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> StudentSentMails(string email, bool notifyShow = true)
    {
        var getStudentSentMails = await _sentMailService.GetAllSentMailWithDetailsByEmailAsync(email);

        if (getStudentSentMails.IsSuccess)
        {
            var studentSentMailVM = _mapper.Map<IEnumerable<AdminStudentMailListVM>>(getStudentSentMails.Data);
            if (notifyShow)
            {
                NotifySuccessLocalized(getStudentSentMails.Message);
            }
            return View(studentSentMailVM);
        }
        if (notifyShow)
        {
            NotifyErrorLocalized(getStudentSentMails.Message);

        }
        return RedirectToAction(nameof(Index));
    }


    [HttpPost]
    public async Task<IActionResult> ResendStudentEmail(Guid sentMailId)
    {
        var studentEmail = _sendMailService.GetStudentEmailById(sentMailId);
        try
        {
            _rabbitMQPublishService.EnqueueModel<ResendStudentEmailDto>(new ResendStudentEmailDto() { SentMailId = sentMailId},     RabbitMQQueueNames.ResendStudentEmail);
            NotifySuccessLocalized(Messages.EmailSendSuccess);
            return RedirectToAction(nameof(StudentSentMails), new { email = studentEmail, notifyShow = false });
        }
        catch (Exception)
        {
            NotifyErrorLocalized(Messages.EmailSendError);
            return RedirectToAction(nameof(StudentSentMails), new { email = studentEmail, notifyShow = false });
        }
    }


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
                TimeSpan examDuration = examResult.Data.ExamDuration;
                var model = _mapper.Map<StudentStudentExamReportVM>(studentExam);
                model = _mapper.Map(exam, model);

                var studentResult = await _studentService.GetByIdAsync(studentExam.StudentId);
                if (studentResult.IsSuccess)
                {
                    model.StudentFullname = studentResult.Data.FirstName + " " + studentResult.Data.LastName;
                    if (examDuration.Hours > 0 & examDuration.Minutes > 0)
                    {
                        string formattedExamDuration = examDuration.Hours + " saat " + examDuration.Minutes + " dakika";
                        model.FormattedExamDuration = formattedExamDuration;
                    }
                    else if (examDuration.Hours > 0 & examDuration.Minutes == 0)
                    {
                        string formattedExamDuration = examDuration.Hours + " saat";
                        model.FormattedExamDuration = formattedExamDuration;
                    }
                    else if (examDuration.Hours == 0 & examDuration.Minutes > 0)
                    {
                        string formattedExamDuration =  examDuration.Minutes + " dakika";
                        model.FormattedExamDuration = formattedExamDuration;
                    }



                    try
                    {
                        var performance = await _examAnalysisService.AnalysisStudentPerformanceAsync(studentExam.StudentId, studentExam.ExamId);

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
        return RedirectToAction("StudentExams", new { id = studentExamResult.Data.ExamId });
    }




    [HttpGet]
    public async Task<IActionResult> StudentExamDetails(Guid id)
    {
        var getStudentExam = await _studentExamService.GetExamStrudentQuestionDetailsByIdAsync(id);
        if (getStudentExam.IsSuccess)
        {
            var studentExamDetailVM = _mapper.Map<AdminExamStudentQuestionDetailsVM>(getStudentExam.Data);
            return View(studentExamDetailVM);
        }
        NotifyErrorLocalized(getStudentExam.Message);
        return RedirectToAction(nameof(Index));
    }

    public async Task<AdminStudentUpdateVM> GetStudent(Guid studentId)
    {
        var studentFoundResult = await _studentService.GetByIdAsync(studentId);

        var studentUpdateVM = _mapper.Map<AdminStudentUpdateVM>(studentFoundResult.Data);
        studentUpdateVM.Cities = await GetCitiesAsync(studentUpdateVM.CityId);
        studentUpdateVM.OtherEmails = (await _emailService.GetEmailAddressesByIdentityIdAsync(studentFoundResult.Data.IdentityId)).Data;

        return studentUpdateVM;
    }


}