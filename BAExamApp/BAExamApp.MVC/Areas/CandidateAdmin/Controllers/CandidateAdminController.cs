using AutoMapper;
using BAExamApp.Business.Constants;
using BAExamApp.Dtos.Admins;
using BAExamApp.Dtos.CandidateAdmins;
using BAExamApp.Dtos.Emails;
using BAExamApp.Dtos.SendMails;
using BAExamApp.MVC.Areas.Admin.Models.AdminVMs;
using BAExamApp.MVC.Areas.CandidateAdmin.Models.CandidateAdminVMs;
using BAExamApp.MVC.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BAExamApp.MVC.Areas.CandidateAdmin.Controllers;
public class CandidateAdminController : CandidateAdminBaseController
{
    private readonly ICandidateAdminService _candidateAdminService;
    private readonly IMapper _mapper;
    private readonly IEmailService _emailService;
    private readonly ISendMailService _sendMailService;
    private readonly IRabbitMQPublishService _rabbitMQPublishService;


    public CandidateAdminController(ICandidateAdminService candidateAdminService, IMapper mapper, IEmailService emailService, ISendMailService sendMailService,IRabbitMQPublishService rabbitMQPublishService)
    {
        _candidateAdminService = candidateAdminService;
        _mapper = mapper;
        _emailService = emailService;
        _sendMailService = sendMailService;
        _rabbitMQPublishService = rabbitMQPublishService;
    }

    public async Task<IActionResult> Index()
    {
        
        var result = await _candidateAdminService.GetAllAsync();
        var candidateAdminList = _mapper.Map<IEnumerable<CandidateAdminListVM>>(result.Data);
        return View(candidateAdminList);
    }

   

    [HttpPost]
    public async Task<IActionResult> Create(CandidateAdminCreateVM model)
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

        var candidateAdminDto = _mapper.Map<CandidateAdminCreateDto>(model);

        if (model.NewImage != null)
        {
            candidateAdminDto.Image = await model.NewImage.FileToStringAsync();
        }

        candidateAdminDto.FirstName = StringExtensions.TitleFormat(model.FirstName);
        candidateAdminDto.LastName = StringExtensions.TitleFormat(model.LastName);

        var addCandidateAdminResult = await _candidateAdminService.AddAsync(candidateAdminDto);
        if (!addCandidateAdminResult.IsSuccess)
        {
            NotifyErrorLocalized(addCandidateAdminResult.Message);
            return RedirectToAction(nameof(Index));
        }

        

        string link = Url.Action("index", "login", new { Area = "" }, Request.Scheme);
        _rabbitMQPublishService.EnqueueModel(new NewUserMailDto() { Email = addCandidateAdminResult.Data.Email, Url = link }, RabbitMQQueueNames.EmailNewAdmin);
        NotifySuccess($"{model.FirstName} {model.LastName} kişisi başarıyla eklendi, Mail adresine mail gönderildi.");
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Details(Guid Id)
    {

       
        var getCandidateAdmin = await _candidateAdminService.GetDetailsByIdAsync(Id);
        if (getCandidateAdmin.IsSuccess)
        {
            var candidateAdminDetailsVM = _mapper.Map<CandidateAdminDetailsVM>(getCandidateAdmin.Data);
            
            return View(candidateAdminDetailsVM);
        }

        NotifyError(getCandidateAdmin.Message);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Delete(Guid id)
    {
        var candidateAdminDeleteResponse = await _candidateAdminService.DeleteAsync(id);

        if (candidateAdminDeleteResponse.IsSuccess)
            NotifySuccess(candidateAdminDeleteResponse.Message);
        else
            NotifyError(candidateAdminDeleteResponse.Message);
        return Json(candidateAdminDeleteResponse);
    }

    

    [HttpPost]
    public async Task<IActionResult> Update(CandidateAdminUpdateVM model)
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
        var updateCandidateAdmin = _mapper.Map<CandidateAdminUpdateDto>(model);
        if (model.NewImage != null)
        {
            updateCandidateAdmin.Image = await model.NewImage.FileToStringAsync();
        }
        updateCandidateAdmin.FirstName = StringExtensions.TitleFormat(model.FirstName);
        updateCandidateAdmin.LastName = StringExtensions.TitleFormat(model.LastName);

        var updateCandidateAdminResult = await _candidateAdminService.UpdateAsync(updateCandidateAdmin);
        if (!updateCandidateAdminResult.IsSuccess)
        {
            NotifyErrorLocalized(updateCandidateAdminResult.Message);
            return View(model);
        }


        NotifySuccessLocalized(updateCandidateAdminResult.Message);
        return RedirectToAction(nameof(Index));
    }

    public async Task<CandidateAdminUpdateVM> GetAdmin(Guid adminId)
    {
        var getCandidateAdminResult = await _candidateAdminService.GetByIdAsync(adminId);
        var candidateAdminDto = getCandidateAdminResult.Data;
        var candidateAdminUpdateVM = _mapper.Map<CandidateAdminUpdateVM>(candidateAdminDto);
        

        return candidateAdminUpdateVM;
    }
}
