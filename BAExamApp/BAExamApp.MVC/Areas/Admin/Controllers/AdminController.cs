using AutoMapper;
using BAExamApp.Business.Constants;
using BAExamApp.Dtos.Admins;
using BAExamApp.Dtos.Emails;
using BAExamApp.Dtos.SendMails;
using BAExamApp.MVC.Areas.Admin.Models.AdminVMs;
using BAExamApp.MVC.Extensions;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BAExamApp.MVC.Areas.Admin.Controllers;

public class AdminController : AdminBaseController
{
    private readonly IAdminService _adminService;
    private readonly ICityService _cityService;
    private readonly ISendMailService _sendMailService;
    private readonly IEmailService _emailService;
    private readonly IRabbitMQPublishService _rabbitMQPublishService;
    private readonly IMapper _mapper;
    public AdminController(IAdminService adminService, IMapper mapper, ICityService cityService, ISendMailService sendMailService, IEmailService emailService,IRabbitMQPublishService rabbitMQPublishService)
    {
        _adminService = adminService;
        _mapper = mapper;
        _cityService = cityService;
        _sendMailService = sendMailService;
        _emailService = emailService;
        _rabbitMQPublishService = rabbitMQPublishService;
    }
    public async Task<IActionResult> Index()
    {
        ViewBag.Cities = await GetCitiesAsync();
        var result = await _adminService.GetAllAsync();
        var adminList = _mapper.Map<IEnumerable<AdminAdminListVM>>(result.Data);
        return View(adminList);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        return View(new AdminAdminCreateVM()
        {
            Cities = await GetCitiesAsync()
        });
    }

    [HttpPost]
    public async Task<IActionResult> Create(AdminAdminCreateVM model, IFormCollection collection)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors);
            foreach (var error in errors)
            {
                var errorMessage = error.ErrorMessage;
                //var propertyName = error.PropertyName;
            }
            model.Cities = await GetCitiesAsync();
            return RedirectToAction(nameof(Index));
        }

        var adminDto = _mapper.Map<AdminCreateDto>(model);

        if (model.NewImage != null)
        {
            adminDto.Image = await model.NewImage.FileToStringAsync();
        }

        adminDto.FirstName = StringExtensions.TitleFormat(model.FirstName);
        adminDto.LastName = StringExtensions.TitleFormat(model.LastName);

        var addAdminResult = await _adminService.AddAsync(adminDto);
        if (!addAdminResult.IsSuccess)
        {
            NotifyErrorLocalized(addAdminResult.Message);
            return RedirectToAction(nameof(Index));
        }

        var adminOtherEmailList = new List<EmailCreateDto>();
        var otherEmailsList = collection["otherEmails"].ToList();
        var identityId = addAdminResult.Data.IdentityId;

        foreach (var adminOtherEmail in otherEmailsList)
        {
            adminOtherEmailList.Add(new EmailCreateDto() { EmailAddress = adminOtherEmail, IdentityId = identityId });
        }

        var addEmailResult = await _emailService.AddRangeAsync(adminOtherEmailList);
        if (!addEmailResult.IsSuccess)
        {
            NotifyErrorLocalized(addEmailResult.Message);
            return RedirectToAction(nameof(Index));
        }

        string link = Url.Action("index", "login", new { Area = "" }, Request.Scheme);
        _rabbitMQPublishService.EnqueueModel(new NewUserMailDto() { Email = addAdminResult.Data.Email, Url = link }, RabbitMQQueueNames.EmailNewAdmin);
        NotifySuccess($"{model.FirstName} {model.LastName} kişisi başarıyla eklendi, Mail adresine mail gönderildi.");
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Delete(Guid id)
    {
        var adminDeleteResponse = await _adminService.DeleteAsync(id);

        if (adminDeleteResponse.IsSuccess)
            NotifySuccess(adminDeleteResponse.Message);
        else
            NotifyError(adminDeleteResponse.Message);
        return Json(adminDeleteResponse);
    }
    public async Task<IActionResult> Details(Guid Id)
    {

        ViewBag.Cities = await GetCitiesAsync();
        var getAdmin = await _adminService.GetDetailsByIdAsync(Id);
        if (getAdmin.IsSuccess)
        {
            var adminDetailsVM = _mapper.Map<AdminAdminDetailsVM>(getAdmin.Data);
            adminDetailsVM.OtherEmails = (await _emailService.GetEmailAddressesByIdentityIdAsync(getAdmin.Data.IdentityId)).Data;
            return View(adminDetailsVM);
        }

        NotifyError(getAdmin.Message);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Update(Guid id)
    {
        var getAdminResult = await _adminService.GetByIdAsync(id);
        if (!getAdminResult.IsSuccess)
        {
            NotifyErrorLocalized(getAdminResult.Message);
            return RedirectToAction(nameof(Index));
        }

        var adminDto = getAdminResult.Data;
        var adminUpdateVM = _mapper.Map<AdminAdminUpdateVM>(adminDto);
        adminUpdateVM.OtherEmails = (await _emailService.GetEmailAddressesByIdentityIdAsync(getAdminResult.Data.IdentityId)).Data;
        adminUpdateVM.Cities = await GetCitiesAsync(adminUpdateVM.CityId);

        return View(adminUpdateVM);
    }

    [HttpPost]
    public async Task<IActionResult> Update(AdminAdminUpdateVM model, IFormCollection collection)
    {

        if (!ModelState.IsValid)
        {
            model.Cities = await GetCitiesAsync(model.CityId);
            return View(model);
        }
        var updateAdmin = _mapper.Map<AdminUpdateDto>(model);
        if (model.NewImage != null)
        {
            updateAdmin.Image = await model.NewImage.FileToStringAsync();
        }
        updateAdmin.FirstName = StringExtensions.TitleFormat(model.FirstName);
        updateAdmin.LastName = StringExtensions.TitleFormat(model.LastName);

        var updateAdminResult = await _adminService.UpdateAsync(updateAdmin);
        if (!updateAdminResult.IsSuccess)
        {
            NotifyErrorLocalized(updateAdminResult.Message);
            return View(model);
        }

        var otherEmailsList = collection["otherEmails"].ToList();
        var adminOtherEmailList = new List<EmailCreateDto>();
        var identityId = updateAdminResult.Data.IdentityId;

        foreach (var adminOtherEmail in otherEmailsList)
        {
            adminOtherEmailList.Add(new EmailCreateDto() { EmailAddress = adminOtherEmail, IdentityId = identityId });
        }
        var addEmailResult = await _emailService.UpdateRangeAsync(adminOtherEmailList, identityId);

        NotifySuccessLocalized(updateAdminResult.Message);
        return RedirectToAction(nameof(Index));
    }

    /// <summary>
    /// Şehirleri liste olarak getirir
    /// </summary>
    /// <param name="cityId">City Id</param>
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
    public async Task<AdminAdminUpdateVM> GetAdmin(Guid adminId)
    {
        var getAdminResult = await _adminService.GetByIdAsync(adminId);
        var adminDto = getAdminResult.Data;
        var adminUpdateVM = _mapper.Map<AdminAdminUpdateVM>(adminDto);
        adminUpdateVM.OtherEmails = (await _emailService.GetEmailAddressesByIdentityIdAsync(getAdminResult.Data.IdentityId)).Data;
        adminUpdateVM.Cities = await GetCitiesAsync(adminUpdateVM.CityId);

        return adminUpdateVM;
    }
}