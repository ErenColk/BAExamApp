using AutoMapper;
using BAExamApp.Dtos.Trainers;
using BAExamApp.MVC.Areas.Trainer.Models.TrainerVMs;
using BAExamApp.MVC.Extensions;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BAExamApp.MVC.Areas.Trainer.Controllers;

public class TrainerController : TrainerBaseController
{
    private readonly ITrainerService _trainerService;
    private readonly IMapper _mapper;
    private readonly ICityService _cityService;
    private readonly IExamEvaluatorService _examsEvaluatorsService;
    public TrainerController(ITrainerService trainerService, IMapper mapper, ICityService cityService, IExamEvaluatorService examsEvaluatorsService)
    {
        _trainerService = trainerService;
        _mapper = mapper;
        _cityService = cityService;
        _examsEvaluatorsService = examsEvaluatorsService;
    }


    [HttpGet]
    public async Task<IActionResult> Details()
    {
        var trainerResult = await _trainerService.GetDetailsByIdentityIdForTrainerAsync(UserIdentityId);

        if (trainerResult.IsSuccess)
        {
            return View(_mapper.Map<TrainerTrainerDetailVM>(trainerResult.Data));
        }

        NotifyErrorLocalized(trainerResult.Message);
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public async Task<IActionResult> Update()
    {
        var trainerFoundResult = await _trainerService.GetByIdentityIdAsync(UserIdentityId);

        if (!trainerFoundResult.IsSuccess)
        {
            NotifyErrorLocalized(trainerFoundResult.Message);
            return RedirectToAction(nameof(Index));
        }

        var trainerUpdateVM = _mapper.Map<TrainerTrainerUpdateVM>(trainerFoundResult.Data);

        trainerUpdateVM.CityList = await GetCitiesAsync(trainerUpdateVM.CityId);

        return View(trainerUpdateVM);
    }

    [HttpPost]
    public async Task<IActionResult> Update(TrainerTrainerUpdateVM trainerUpdateVM)
    {
        if (!ModelState.IsValid)
        {
            trainerUpdateVM.CityList = await GetCitiesAsync(trainerUpdateVM.CityId);
            return View(trainerUpdateVM);
        }

        var trainerUpdateDto = _mapper.Map<TrainerUpdateDto>(trainerUpdateVM);
        if (trainerUpdateVM.ImageFile is not null)
        {
            trainerUpdateDto.Image = await trainerUpdateVM.ImageFile.FileToStringAsync();
        }

        var result = await _trainerService.UpdateAsync(trainerUpdateDto);
        if (result.IsSuccess)
        {
            NotifySuccessLocalized(result.Message);
            return RedirectToAction("Index", "Home");
        }

        NotifyErrorLocalized(result.Message);
        trainerUpdateVM.CityList = await GetCitiesAsync(trainerUpdateVM.CityId);
        return View(trainerUpdateVM);
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

    //Bu Action ExamRule'a sınav tipi eklendikten sonra düzeltilecek. Bu Action'ın amacı otomatik puanlanmayacak sınavlara atanan Evaluator'ların okumayı tamamladığı sınavları listelemektir.
    //[HttpGet]
    //public async Task<IActionResult> GetAssignedExamsForTrainer()
    //{
    //    var trainerId = (await _trainerService.GetByIdentityIdAsync(UserIdentityId)).Data.Id;

    //    var result = await _examsEvaluatorsService.GetAllByTrainerIdAsync(trainerId);

    //    return View(_mapper.Map<List<TrainerExamEvaluatorListVM>>(result.Data));
    //}

    //Bu Action ExamRule'a sınav tipi eklendikten sonra düzeltilecek. Bu Action'ın amacı otomatik puanlanmayacak sınavlara atanan Evaluator'ların henüz okumadığı sınavları listelemektir.
    //[HttpGet]
    //public async Task<IActionResult> GetUnfinishedAssignedExamsForTrainer(int itemsPerPage = 10, int page = 1)
    //{
    //    var trainerId = (await _trainerService.GetByIdentityIdAsync(UserIdentityId)).Data.Id;

    //    var result = (await _examsEvaluatorsService.GetAllByTrainerIdAsync(trainerId)).Data.Where(x => x.ExamDateTime < DateTime.Now);

    //    return View(_mapper.Map<List<TrainerExamEvaluatorListVM>>(result));
    //}
}