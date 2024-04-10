using AutoMapper;
using BAExamApp.Business.Constants;
using BAExamApp.Core.Enums;
using BAExamApp.Core.Utilities.Results;
using BAExamApp.Dtos.Emails;
using BAExamApp.Dtos.SendMails;
using BAExamApp.Dtos.Students;
using BAExamApp.Dtos.Talents;
using BAExamApp.Dtos.Trainers;
using BAExamApp.MVC.Areas.Admin.Models.TrainerVMs;
using BAExamApp.MVC.Extensions;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BAExamApp.MVC.Areas.Admin.Controllers;

public class TrainerController : AdminBaseController
{
    private readonly IBranchService _branchService;
    private readonly IClassroomService _classroomService;
    private readonly ICityService _cityService;
    private readonly IEmailService _emailService;
    private readonly IRabbitMQPublishService _rabbitMQPublishService;
    private readonly ITalentService _talentService;
    private readonly ITrainerService _trainerService;
    private readonly ITrainerClassroomService _trainerClassroomService;
    private readonly ITrainerProductService _trainerProductService;
    private readonly ITechnicalUnitService _technicalUnitService;
    private readonly ISendMailService _sendMailService;
    private readonly IProductService _productService;
    private readonly IMapper _mapper;

    public TrainerController(IBranchService branchService, IClassroomService classroomService, ICityService cityService, ITalentService talentService, ITrainerService trainerService, ITechnicalUnitService technicalUnitService, ISendMailService sendMailService, IProductService productService, ITrainerProductService trainerProductService, IMapper mapper, IEmailService emailService,IRabbitMQPublishService rabbitMQPublishService)
    {
        _branchService = branchService;
        _classroomService = classroomService;
        _cityService = cityService;
        _emailService = emailService;
        _rabbitMQPublishService = rabbitMQPublishService;
        _talentService = talentService;
        _trainerService = trainerService;
        _technicalUnitService = technicalUnitService;
        _sendMailService = sendMailService;
        _productService = productService;
        _trainerProductService = trainerProductService;
        _mapper = mapper;
    }


    [HttpGet]
    public async Task<IActionResult> Index(bool showAllData = false, bool showCreateModal = false)
    {
        ViewBag.TecnicalUnit = await GetTechnicalUnitsAsync();
        ViewBag.Cities = await GetCitiesAsync();
        ViewBag.Talents = await GetTalentAsync();

        var result = await _trainerService.GetAllAsync();
        var trainerList = _mapper.Map<IEnumerable<AdminTrainerListVM>>(result.Data);

        if (!showAllData)
        {
            trainerList = trainerList.Where(trainer => trainer.Status != Status.Deleted);
        }

        trainerList = trainerList.OrderBy(trainer => trainer.ModifiedDate);

        ViewBag.ShowAllData = showAllData;
        ViewBag.ShowCreateModal = showCreateModal;

        return View(trainerList);
    }

    [HttpPost]
    public async Task<IActionResult> Create(AdminTrainerCreateVM model, IFormCollection collection)
    {
        if (!ModelState.IsValid)
        {
            model.TechnicalUnitList = await GetTechnicalUnitsAsync();
            model.CityList = await GetCitiesAsync();
            model.TalentList = await GetTalentAsync();
            return RedirectToAction(nameof(Index), new { showCreateModal = true });
        }

        var trainerCreateDto = _mapper.Map<TrainerCreateDto>(model);

        if (model.NewImage is not null)
        {
            trainerCreateDto.Image = await model.NewImage.FileToStringAsync();
        }

        trainerCreateDto.FirstName = StringExtensions.TitleFormat(model.FirstName);
        trainerCreateDto.LastName = StringExtensions.TitleFormat(model.LastName);

        var addTrainerresult = await _trainerService.AddAsync(trainerCreateDto);
        if (!addTrainerresult.IsSuccess)
        {
            NotifyErrorLocalized(addTrainerresult.Message);
            model.TechnicalUnitList = await GetTechnicalUnitsAsync();
            model.CityList = await GetCitiesAsync();
            model.TalentList = await GetTalentAsync();
            return RedirectToAction(nameof(Index), new { showCreateModal = true });
        }
        string link = Url.Action("index", "login", new { Area = "" }, Request.Scheme);
        _rabbitMQPublishService.EnqueueModel(new NewUserMailDto() { Email = addTrainerresult.Data.Email, Url = link }, RabbitMQQueueNames.EmailNewTrainer);
        NotifySuccess($"{model.FirstName} {model.LastName} kişisi başarıyla eklendi, Mail adresine mail gönderildi.");

        var trainerOtherEmailList = new List<EmailCreateDto>();
        var otherEmailsList = collection["otherEmails"].ToList();
        var identityId = addTrainerresult.Data.IdentityId;
        foreach (var trainerOtherEmail in otherEmailsList)
        {
            trainerOtherEmailList.Add(new EmailCreateDto() { EmailAddress = trainerOtherEmail, IdentityId = identityId });
        }
        var addEmailResult = await _emailService.AddRangeAsync(trainerOtherEmailList);

        if (!addEmailResult.IsSuccess)
        {
            NotifyErrorLocalized(addEmailResult.Message);
            return RedirectToAction(nameof(Index));
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Delete(Guid id)
    {
        var trainerDeleteResult = await _trainerService.DeleteAsync(id);

        if (!trainerDeleteResult.IsSuccess)
        {
            NotifyErrorLocalized(trainerDeleteResult.Message);
        }
        else NotifySuccessLocalized(trainerDeleteResult.Message);

        return Json(trainerDeleteResult);
    }

    [HttpGet]
    public async Task<IActionResult> Update(Guid id)
    {
        var trainerFoundResult = await _trainerService.GetByIdAsync(id);
        if (!trainerFoundResult.IsSuccess)
        {
            NotifyErrorLocalized(trainerFoundResult.Message);
            return RedirectToAction(nameof(Index));
        }

        var trainerUpdateVM = _mapper.Map<AdminTrainerUpdateVM>(trainerFoundResult.Data);
        trainerUpdateVM.TechnicalUnitList = await GetTechnicalUnitsAsync(trainerUpdateVM.TechnicalUnitId);
        trainerUpdateVM.CityList = await GetCitiesAsync(trainerUpdateVM.CityId);
        trainerUpdateVM.TalentList = await GetTalentAsync();
        trainerUpdateVM.OtherEmails = (await _emailService.GetEmailAddressesByIdentityIdAsync(trainerFoundResult.Data.IdentityId)).Data;

        return PartialView("Update", trainerUpdateVM);
    }


    [HttpPost]
    public async Task<IActionResult> Update(AdminTrainerUpdateVM trainerUpdateVM, IFormCollection collection)
    {
        if (!ModelState.IsValid)
        {
            trainerUpdateVM.TechnicalUnitList = await GetTechnicalUnitsAsync(trainerUpdateVM.TechnicalUnitId);
            trainerUpdateVM.CityList = await GetCitiesAsync(trainerUpdateVM.CityId);
            trainerUpdateVM.TalentList = await GetTalentAsync();
            return View(trainerUpdateVM);
        }

        var trainerUpdateDto = _mapper.Map<TrainerUpdateDto>(trainerUpdateVM);
        if (trainerUpdateVM.NewImage != null)
        {
            trainerUpdateDto.Image = await trainerUpdateVM.NewImage.FileToStringAsync();
        }

        trainerUpdateDto.FirstName = StringExtensions.TitleFormat(trainerUpdateVM.FirstName);
        trainerUpdateDto.LastName = StringExtensions.TitleFormat(trainerUpdateVM.LastName);

        var updateTrainerresult = await _trainerService.UpdateAsync(trainerUpdateDto);
        if (!updateTrainerresult.IsSuccess)
        {
            NotifyErrorLocalized(updateTrainerresult.Message);
        }
        else
        {
            NotifySuccessLocalized(updateTrainerresult.Message);
        }

        var otherEmailsList = collection["otherEmails"].ToList();
        var trainerOtherEmailList = new List<EmailCreateDto>();
        var identityId = updateTrainerresult.Data.IdentityId;

        foreach (var trainerOtherEmail in otherEmailsList)
        {
            trainerOtherEmailList.Add(new EmailCreateDto() { EmailAddress = trainerOtherEmail, IdentityId = identityId });
        }
        var addEmailResult = await _emailService.UpdateRangeAsync(trainerOtherEmailList, identityId);

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Details(Guid id)
    {
        ViewBag.TecnicalUnit = await GetTechnicalUnitsAsync();
        ViewBag.Cities = await GetCitiesAsync();
        ViewBag.Talents = await GetTalentAsync();

        var getTrainerResponse = await _trainerService.GetTrainerDetailsByIdAsync(id);

        if (getTrainerResponse.IsSuccess)
        {
            var trainerDetails = _mapper.Map<AdminTrainerDetailsVM>(getTrainerResponse.Data);
            trainerDetails.OtherEmails = (await _emailService.GetEmailAddressesByIdentityIdAsync(getTrainerResponse.Data.IdentityId)).Data;

            return View(trainerDetails);
        }

        NotifyErrorLocalized(getTrainerResponse.Message);
        return RedirectToAction(nameof(Index));
    }

    private async Task<SelectList> GetTechnicalUnitsAsync(Guid? technicalUnitId = null)
    {
        var technicalUnitList = (await _technicalUnitService.GetAllAsync()).Data;
        return new SelectList(technicalUnitList.Select(x => new SelectListItem
        {
            Value = x.Id.ToString(),
            Text = x.Name,
            Selected = x.Id == (technicalUnitId != null ? technicalUnitId.Value : technicalUnitId)
        }), "Value", "Text");

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
    public IActionResult CallProductListTrainerUpdate(Guid technicalUnitId)
    {
        return ViewComponent("TrainerUpdateProductList", new { technicalUnitId });
    }

    /// <summary>
    /// Verilen ClassRoomId ye Göre var olan Trainerların First Name ve Last Name Alanlarını Getirir.
    /// </summary>
    /// <param name="classroomId">Guid  classroomId</param>
    /// <returns>Listeyi string olarak döner</returns>
    private async Task<string> GetClassroomTrainersAsync(Guid classroomId)
    {
        var result = await _trainerClassroomService.GetTrainersWithSpesificClassroomIdAsync(classroomId);
        if (!result.IsSuccess)
        {
            return result.Message;
        }

        var trainerList = result.Data.Select(x => x.FirstName + " " + x.LastName).ToList();
        return string.Join($" | ", trainerList);
    }
    /// <summary>
    /// Eğitimleri liste olarak getirir
    /// </summary>
    /// <param name="technicalUnitId"> teknik birim id ye göre eğitimleri getirir</param>
    /// <returns>Parametre ile kullanılırsa parametre verisine göre json dönüş yapar</returns>
    [HttpGet]
    public async Task<IActionResult> CallProductList(Guid technicalUnitId)
    {
        var productList = await _productService.GetAllByTechnicalUnitIdAsync(technicalUnitId);
        var selectList = new List<SelectListItem>();
        foreach (var product in productList.Data)
        {
            selectList.Add(new SelectListItem
            {
                Value = product.Id.ToString(),
                Text = product.Name
            });
        }
        return Json(selectList);
    }
    /// <summary>
    /// Verilen ClassRoomId ye Göre var olan Trainerların First Name ve Last Name Alanlarını Getirir.
    /// </summary>
    /// <param name="classroomId">Guid  classroomId</param>
    /// <returns>Listeyi string olarak döner</returns>
    private async Task<SelectList> GetTalentAsync(Guid? talentId = null)
    {
        var talentList = (await _talentService.GetAllAsync()).Data;
        return new SelectList(talentList.Select(x => new SelectListItem
        {
            Value = x.Id.ToString(),
            Text = x.Name,
            Selected = x.Id == (talentId != null ? talentId.Value : talentId)
        }).OrderBy(x => x.Text), "Value", "Text");
    }
    /// <summary>
    /// Yetenekleri liste olarak getirir
    /// </summary>
    /// <returns>Parametre ile kullanılırsa parametre verisine göre json dönüş yapar</returns>
    [HttpGet]
    public async Task<IActionResult> CallTalentList()
    {
        var talentList = await _talentService.GetAllAsync();
        var selectList = new List<SelectListItem>();
        foreach (var talent in talentList.Data)
        {
            selectList.Add(new SelectListItem
            {
                Value = talent.Id.ToString(),
                Text = talent.Name
            });
        }
        return Json(selectList);
    }
    [HttpGet]
    public async Task<IActionResult> AddTalent()
    {
        return PartialView("_AddTalentPartialView");
    }
    [HttpPost]
    public async Task<IActionResult> AddTalent(string name)
    {
        if (name != null)
        {
            var talentNames = name.Split(',');

            foreach (var talentName in talentNames)
            {
                var talentCreateDto = new TalentCreateDto
                {
                    Name = talentName.Trim()
                };
                await _talentService.AddAsync(talentCreateDto);
            }
            var talents = (await _talentService.GetAllAsync()).Data;
            return Json(talents);
        }
        return Json(new { success = false });
    }

    public async Task<AdminTrainerUpdateVM> GetTrainer(Guid trainerId)
    {
        var trainerFoundResult = await _trainerService.GetByIdAsync(trainerId);

        var trainerUpdateVM = _mapper.Map<AdminTrainerUpdateVM>(trainerFoundResult.Data);
        trainerUpdateVM.TechnicalUnitList = await GetTechnicalUnitsAsync(trainerUpdateVM.TechnicalUnitId);
        trainerUpdateVM.CityList = await GetCitiesAsync(trainerUpdateVM.CityId);
        trainerUpdateVM.TalentList = await GetTalentAsync();
        trainerUpdateVM.OtherEmails = (await _emailService.GetEmailAddressesByIdentityIdAsync(trainerFoundResult.Data.IdentityId)).Data;

        return trainerUpdateVM;
    }
}
