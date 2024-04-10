using AutoMapper;
using BAExamApp.Dtos.Candidates;
using BAExamApp.Dtos.Emails;
using BAExamApp.Dtos.Students;
using BAExamApp.MVC.Areas.Admin.Models.StudentVMs;
using BAExamApp.MVC.Areas.CandidateAdmin.Models.StudentVMs;
using BAExamApp.MVC.Extensions;

namespace BAExamApp.MVC.Areas.CandidateAdmin.Controllers;
public class CandidateController : CandidateAdminBaseController
{
    private readonly ICandidateService _candidateService;
    private readonly IMapper _mapper;

    public CandidateController(ICandidateService candidateService,IMapper mapper)
    {
        _candidateService = candidateService;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> Index(bool showAllActiveStudents = false)
    {
        List<CandidateAdminCandidateListVM> students = new List<CandidateAdminCandidateListVM>();

        if (showAllActiveStudents)
        {
            var activeStudents = await _candidateService.GetActiveCandidatesAsync();
            students = _mapper.Map<List<CandidateAdminCandidateListVM>>(activeStudents.Data);
        }

        ViewBag.ShowAllActiveStudents = showAllActiveStudents;

        return View(students);
    }

    [HttpPost]
    public async Task<IActionResult> GetCandidates(CandidateAdminCandidateListVM candidateAdminStudentListVM)
    {

        var getStudentResponse = await _candidateService.GetCandidatesByNameOrSurnameOrMailAdressAsync(candidateAdminStudentListVM.FirstName, candidateAdminStudentListVM.LastName, candidateAdminStudentListVM.Email);

        var studentList = _mapper.Map<List<CandidateAdminCandidateListVM>>(getStudentResponse.Data);
        return View("Index", studentList);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CandidateAdminCandidateCreateVM studentCreateVM)
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

        var studentCreateDto = _mapper.Map<CandidateCreateDto>(studentCreateVM);
        if (studentCreateVM.NewImage != null)
        {
            studentCreateDto.Image = await studentCreateVM.NewImage.FileToStringAsync();
        }

        studentCreateDto.FirstName = StringExtensions.TitleFormat(studentCreateVM.FirstName);
        studentCreateDto.LastName = StringExtensions.TitleFormat(studentCreateVM.LastName);

        var addCandidateStudentResult = await _candidateService.AddAsync(studentCreateDto);


        if (addCandidateStudentResult.IsSuccess)
        {
            NotifySuccessLocalized(addCandidateStudentResult.Message);
        }
        else
        {
            NotifyErrorLocalized(addCandidateStudentResult.Message);
        }
        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> Update(CandidateAdminCandidateUpdateVM model)
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

        var updateStudent = _mapper.Map<CandidateUpdateDto>(model);

        if (model.NewImage != null)
        {
            updateStudent.Image = await model.NewImage.FileToStringAsync();
        }


        updateStudent.FirstName = StringExtensions.TitleFormat(model.FirstName);
        updateStudent.LastName = StringExtensions.TitleFormat(model.LastName);
        var updateStudentResult = await _candidateService.UpdateAsync(updateStudent);

        if (!updateStudentResult.IsSuccess)
        {
            NotifyErrorLocalized(updateStudentResult.Message);
        }
        else
        {
            NotifySuccessLocalized(updateStudentResult.Message);
        }

        return RedirectToAction(nameof(Index));
    }

    public async Task<CandidateAdminCandidateUpdateVM> GetCandidate(Guid candidateId)
    {
        var studentFoundResult = await _candidateService.GetByIdAsync(candidateId);

        var studentUpdateVM = _mapper.Map<CandidateAdminCandidateUpdateVM>(studentFoundResult.Data);

        return studentUpdateVM;
    }

    [HttpGet]
    public async Task<IActionResult> Details(Guid id)
    {

        var getStudent = await _candidateService.GetCandidateDetailsByIdAsync(id);
        if (getStudent.IsSuccess)
        {
            var studentDetailsVM = _mapper.Map<CandidateAdminCandidateDetailVM>(getStudent.Data);
            return View(studentDetailsVM);
        }
        NotifyErrorLocalized(getStudent.Message);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Delete([FromQuery(Name = "id")] Guid id)
    {
        var deleteResult = await _candidateService.DeleteAsync(id);
        if (deleteResult.IsSuccess)
            NotifySuccessLocalized(deleteResult.Message);
        else
            NotifyErrorLocalized(deleteResult.Message);

        return Json(deleteResult);

    }
}
