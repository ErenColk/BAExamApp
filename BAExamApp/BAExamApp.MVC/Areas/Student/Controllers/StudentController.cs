using AutoMapper;
using BAExamApp.Dtos.Students;
using BAExamApp.MVC.Areas.Student.Models.StudentVMs;
using BAExamApp.MVC.Extensions;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BAExamApp.MVC.Areas.Student.Controllers;

public class StudentController : StudentBaseController
{
    private readonly IStudentService _studentService;
    private readonly IMapper _mapper;
    private readonly ICityService _cityService;

    public StudentController(IStudentService studentService, IMapper mapper, ICityService cityService)
    {
        _studentService = studentService;
        _mapper = mapper;
        _cityService = cityService;
    }

    [HttpGet]
    public async Task<IActionResult> Details()
    {
        var getStudent = await _studentService.GetByIdentityIdAsync(UserIdentityId);
        if (getStudent.IsSuccess)
        {
            return View(_mapper.Map<StudentStudentDetailVM>(getStudent.Data));
        }
        NotifyErrorLocalized(getStudent.Message);
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public async Task<IActionResult> Update()
    {
        var result = await _studentService.GetByIdentityIdAsync(UserIdentityId);
        if (result.IsSuccess)
        {
            var studentUpdateVM = _mapper.Map<StudentStudentUpdateVM>(result.Data);
            studentUpdateVM.CityList = await GetCitiesAsync(studentUpdateVM.CityId);
            return View(studentUpdateVM);
        }

        NotifyErrorLocalized(result.Message);
        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    public async Task<IActionResult> Update(StudentStudentUpdateVM updateStudent)
    {
        if (!ModelState.IsValid)
        {
            return View(updateStudent);
        }

        var studentUpdateDto = _mapper.Map<StudentUpdateDto>(updateStudent);

        if (updateStudent.NewImage != null)
        {
            studentUpdateDto.Image = await updateStudent.NewImage.FileToStringAsync();
        }

        var result = await _studentService.UpdateAsync(studentUpdateDto);
        if (result.IsSuccess)
        {
            NotifySuccessLocalized(result.Message);
            return RedirectToAction("Details");
        }

        NotifyErrorLocalized(result.Message);
        return View(updateStudent);
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
}