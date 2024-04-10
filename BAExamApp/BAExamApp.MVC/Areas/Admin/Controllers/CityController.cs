using AutoMapper;
using BAExamApp.Dtos.Cities;
using BAExamApp.MVC.Areas.Admin.Models.CityVMs;

namespace BAExamApp.MVC.Areas.Admin.Controllers;

public class CityController : AdminBaseController
{

    private readonly ICityService _cityService;
    private readonly IMapper _mapper;
    public CityController(ICityService cityService, IMapper mapper)
    {
        _cityService = cityService;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var result = await _cityService.GetAllAsync();
        var cityList = _mapper.Map<IEnumerable<AdminCityListVM>>(result.Data.OrderBy(x => x.Name));

        return View(cityList);
    }


    [HttpPost]
    public async Task<IActionResult> Create(AdminCityCreateVM cityCreateVM)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(x => x.Errors);
            string errorMessages = null!;
            foreach (var error in errors)
            {
                errorMessages += " ," + error.ErrorMessage;
            }
            NotifyError(errorMessages);
            return RedirectToAction(nameof(Index));
        }

        var addResult = await _cityService.AddAsync(_mapper.Map<CityCreateDto>(cityCreateVM));
        if (!addResult.IsSuccess)
        {
            NotifyErrorLocalized(addResult.Message);
            return RedirectToAction(nameof(Index));
        }

        NotifySuccessLocalized(addResult.Message);
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete([FromQuery(Name = "id")] Guid id)
    {
        var deleteResult = await _cityService.DeleteAsync(id);
        if (!deleteResult.IsSuccess)
        {
            NotifyErrorLocalized(deleteResult.Message);
        }
        else
        {
            NotifySuccessLocalized(deleteResult.Message);
        }

        return Json(deleteResult);
    }

    [HttpPost]
    public async Task<IActionResult> Update(AdminCityUpdateVM model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var cityUpdateDto = _mapper.Map<CityUpdateDto>(model);
        var updateResult = await _cityService.UpdateAsync(cityUpdateDto);
        if (!updateResult.IsSuccess)
        {
            NotifyErrorLocalized(updateResult.Message);
            return View(model);
        }

        NotifySuccessLocalized(updateResult.Message);
        return RedirectToAction(nameof(Index));
    }
}
