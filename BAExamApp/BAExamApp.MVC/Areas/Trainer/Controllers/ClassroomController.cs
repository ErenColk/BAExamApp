using AutoMapper;
using BAExamApp.MVC.Areas.Trainer.Models.ClassroomVMs;

namespace BAExamApp.MVC.Areas.Trainer.Controllers;

public class ClassroomController : TrainerBaseController
{
    private readonly ITrainerService _trainerService;
    private readonly IClassroomService _classroomService;
    private readonly IMapper _mapper;
    public ClassroomController(ITrainerService trainerService, IClassroomService classroomService, IMapper mapper)
    {
        _trainerService = trainerService;
        _classroomService = classroomService;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var classroomResult = await _trainerService.GetClassroomsByIdentityId(UserIdentityId);

        var classrooms = _mapper.Map<IEnumerable<TrainerClassroomListVM>>(classroomResult.Data);

        return View(classrooms);
    }

    [HttpGet]
    public async Task<IActionResult> Details(Guid id)
    {
        var classroomDetailResult = await _classroomService.GetDetailsByIdAsync(id);

        if (!classroomDetailResult.IsSuccess)
            return NotFound();

        var classroomDetails = _mapper.Map<TrainerClassroomDetailsVM>(classroomDetailResult.Data);

        return View(classroomDetails);
    }
}