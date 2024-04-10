using BAExamApp.Business.Interfaces.Services;
using BAExamApp.Dtos.Exams;
using BAExamApp.Dtos.StudentExams;
using BAExamApp.Entities.DbSets;

namespace BAExamApp.Business.Services;
public class StudentExamService : IStudentExamService
{
    private readonly IStudentExamRepository _studentExamRepository;
    private readonly ISendMailService _sendMailService;
    private IMapper _mapper;
    private readonly IStudentClassroomService _studentClassroomService;
    private readonly IClassroomService _classroomService;
    private readonly ITrainerClassroomService _trainerClassroomService;

    public StudentExamService(IStudentExamRepository studentExamRepository, ISendMailService sendMailService, IMapper mapper, IStudentClassroomService studentClassroomService, IClassroomService classroomService, ITrainerClassroomService trainerClassroomService)
    {
        _studentExamRepository = studentExamRepository;
        _sendMailService = sendMailService;
        _mapper = mapper;
        _studentClassroomService = studentClassroomService;
        _classroomService = classroomService;
        _trainerClassroomService = trainerClassroomService;
    }

    public async Task<IDataResult<StudentExamDto>> GetByIdAsync(Guid id)
    {
        var studentExam = await _studentExamRepository.GetByIdAsync(id);
        if (studentExam == null)
        {
            return new ErrorDataResult<StudentExamDto>(Messages.StudentExamNotFound);
        }

        return new SuccessDataResult<StudentExamDto>(_mapper.Map<StudentExamDto>(studentExam), Messages.FoundSuccess);
    }

    public async Task<IDataResult<List<StudentExamListDto>>> GetAllByStudentIdAsync(Guid id)
    {
        var studentExams = await _studentExamRepository.GetAllAsync(x => x.StudentId == id);

        return new SuccessDataResult<List<StudentExamListDto>>(_mapper.Map<List<StudentExamListDto>>(studentExams), Messages.FoundSuccess);
    }

    public async Task<IDataResult<List<StudentExamListDto>>> GetAllByExamIdAsync(Guid id)
    {
        var studentExams = await _studentExamRepository.GetAllAsync(x => x.ExamId == id);

        return new SuccessDataResult<List<StudentExamListDto>>(_mapper.Map<List<StudentExamListDto>>(studentExams), Messages.FoundSuccess);
    }

    public async Task<IDataResult<StudentExamDto>> UpdateAsync(StudentExamUpdateDto studentExamUpdateDto)
    {
        var studentExam = await _studentExamRepository.GetByIdAsync(studentExamUpdateDto.Id);

        if (studentExam is null)
        {
            return new ErrorDataResult<StudentExamDto>(Messages.StudentExamNotFound);
        }

        var updatedStudentExam = _mapper.Map(studentExamUpdateDto, studentExam);

        await _studentExamRepository.UpdateAsync(updatedStudentExam);
        await _studentExamRepository.SaveChangesAsync();

        return new SuccessDataResult<StudentExamDto>(_mapper.Map<StudentExamDto>(updatedStudentExam), Messages.UpdateSuccess);
    }
    public async Task<IDataResult<List<StudentExamsDetailsDto>>> GetAllExamsWithDetailsByIdForTrainerAsync(Guid id)
    {
        var studentExams = await _studentExamRepository.GetAllAsync(x => x.StudentId == id);
        var studentsExamScores = studentExams.Where(x => x.Score != null);
        if (studentsExamScores.Count() == 0)
        {
            return new ErrorDataResult<List<StudentExamsDetailsDto>>(Messages.StudentExamsNotFound);
        }

        var studentsLatestClassroom = await _studentClassroomService.GetLatestClassroomByStudentIdForAdminAsync(id);

        if (studentsLatestClassroom.Data == null)
        {
            return new ErrorDataResult<List<StudentExamsDetailsDto>>(Messages.ClassroomNotFound);
        }

        var studentsLatestClassroomId = studentsLatestClassroom.Data.ClassroomId;
        var studentsLatestClassroomname = await _classroomService.GetByIdAsync(studentsLatestClassroomId);
        var studentsLatestClassroomsTrainers = await _trainerClassroomService.GetTrainersWithSpesificClassroomIdAsync(studentsLatestClassroomId);

        var studentExamsDetails = _mapper.Map<List<StudentExamsDetailsDto>>(studentsExamScores);
        foreach (var item in studentExamsDetails)
        {
            item.LatestClassroom = studentsLatestClassroomname.Data.Name;
            item.LatestClassroomsTrainers = studentsLatestClassroomsTrainers.Data != null && studentsLatestClassroomsTrainers.Data.Any() ? string.Join(", ", studentsLatestClassroomsTrainers.Data.Select(x => $"{x.FirstName} {x.LastName}")) : "";
        }

        return new SuccessDataResult<List<StudentExamsDetailsDto>>(studentExamsDetails, Messages.FoundSuccess);
    }


    public async Task<IDataResult<List<StudentExamsDetailsDto>>> GetAllExamsWithDetailsByIdAsync(Guid id)
    {
        var studentExams = await _studentExamRepository.GetAllAsync(x => x.StudentId == id);
        if (studentExams.Count() == 0)
        {
            return new ErrorDataResult<List<StudentExamsDetailsDto>>(Messages.StudentExamsNotFound);
        }

        var studentsLatestClassroom = await _studentClassroomService.GetLatestClassroomByStudentIdForAdminAsync(id);

        if (studentsLatestClassroom.Data == null)
        {
            return new ErrorDataResult<List<StudentExamsDetailsDto>>(Messages.ClassroomNotFound);
        }

        var studentsLatestClassroomId = studentsLatestClassroom.Data.ClassroomId;
        var studentsLatestClassroomname = await _classroomService.GetByIdAsync(studentsLatestClassroomId);
        var studentsLatestClassroomsTrainers = await _trainerClassroomService.GetTrainersWithSpesificClassroomIdAsync(studentsLatestClassroomId);

        var studentExamsDetails = _mapper.Map<List<StudentExamsDetailsDto>>(studentExams);
        foreach (var item in studentExamsDetails)
        {
            item.LatestClassroom = studentsLatestClassroomname.Data.Name;
            item.LatestClassroomsTrainers = studentsLatestClassroomsTrainers.Data != null && studentsLatestClassroomsTrainers.Data.Any() ? string.Join(", ", studentsLatestClassroomsTrainers.Data.Select(x => $"{x.FirstName} {x.LastName}")) : "";
        }

        return new SuccessDataResult<List<StudentExamsDetailsDto>>(studentExamsDetails, Messages.FoundSuccess);
    }

    public async Task<IDataResult<List<StudentExamsAdminDto>>> GetAllExamsByStudentIdAsync(Guid id)
    {
        var studentExams = await _studentExamRepository.GetAllAsync(x => x.StudentId == id);

        if (studentExams.Count()==0)
        {
            return new ErrorDataResult<List<StudentExamsAdminDto>>(Messages.StudentExamsNotFound);
        }

            return new SuccessDataResult<List<StudentExamsAdminDto>>(_mapper.Map<List<StudentExamsAdminDto>>(studentExams), Messages.FoundSuccess);
    }
    public async Task<IDataResult<ExamStrudentQuestionDetailsDto>> GetExamStrudentQuestionDetailsByIdAsync(Guid id)
    {
        var studentExam = await _studentExamRepository.GetByIdAsync(id);
        if (studentExam == null)
        {
            return new ErrorDataResult<ExamStrudentQuestionDetailsDto>(Messages.StudentExamNotFound);
        }

        var examStrudentQuestionDetails = _mapper.Map<ExamStrudentQuestionDetailsDto>(studentExam);

        return new SuccessDataResult<ExamStrudentQuestionDetailsDto>(examStrudentQuestionDetails, Messages.FoundSuccess);
    }
}
