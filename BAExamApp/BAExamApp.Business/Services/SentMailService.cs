using BAExamApp.Dtos.Classrooms;
using BAExamApp.Dtos.Emails;
using BAExamApp.Dtos.SentMails;
using BAExamApp.Dtos.Students;
using BAExamApp.Dtos.Trainers;
using BAExamApp.Entities.DbSets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BAExamApp.Business.Services;
public class SentMailService : ISentMailService
{
    private readonly ISentMailRepository _sentMailRepository;
    private readonly IMapper _mapper;
    private readonly IStudentService _studentService;
    private readonly IStudentClassroomService _studentClassroomService;
    private readonly ITrainerClassroomService _trainerClassroomService;
    private readonly ITrainerService _trainerService;

    public SentMailService(ISentMailRepository sentMailRepository, IMapper mapper, IStudentService studentService, IStudentClassroomService studentClassroomService, ITrainerClassroomService trainerClassroomService, ITrainerService trainerService)
    {
        _sentMailRepository = sentMailRepository;
        _mapper = mapper;
        _studentService = studentService;
        _studentClassroomService = studentClassroomService;
        _trainerClassroomService = trainerClassroomService;
        _trainerService = trainerService;
    }
    /// <summary>
    /// Gönderilen maili database üzerinde kaydeder.
    /// </summary>
    /// <param name="sentMailCreateDto">Gönderilen mail modeli </param>
    /// <returns>Kaydetme başarılı olduğunda SentMailDto tipinde değer döndürür.</returns>
    public async Task<IDataResult<SentMailDto>> AddAsync(SentMailCreateDto sentMailCreateDto)
    {

        var sentMail = _mapper.Map<SentMail>(sentMailCreateDto);
        if (sentMail != null)
        {
            await _sentMailRepository.AddAsync(sentMail);
            await _sentMailRepository.SaveChangesAsync();
            return new SuccessDataResult<SentMailDto>(_mapper.Map<SentMailDto>(sentMail), Messages.AddSuccess);
        }

        return new SuccessDataResult<SentMailDto>(Messages.AddFail);
    }

    /// <summary>
    /// Gönderilen maili database üzerinde günceller.
    /// </summary>
    /// <param name="sentMailUpdate">Gönderilen mail modeli</param>
    /// <returns>Güncelleme başarılı olduğunda SentMailUpdateDto tipinde değer döndürür.</returns>
    public async Task<IDataResult<SentMailUpdateDto>> UpdateAsync(SentMailUpdateDto sentMailUpdate)
    {

        var sentMail = await _sentMailRepository.GetByIdAsync(sentMailUpdate.Id);
        var updateSentMail = _mapper.Map(sentMailUpdate, sentMail);
        if (updateSentMail != null)
        {
            await _sentMailRepository.UpdateAsync(updateSentMail);
            await _sentMailRepository.SaveChangesAsync();
            return new SuccessDataResult<SentMailUpdateDto>(_mapper.Map<SentMailUpdateDto>(updateSentMail), Messages.UpdateSuccess);

        }
        return new ErrorDataResult<SentMailUpdateDto>(Messages.UpdateFail);


    }


    /// <summary>
    /// Verilen sorguya göre gönderilen mailin varlığını kontrol etmek için kullanılır.
    /// </summary>
    /// <param name="expression">Atılacak olan sorgu </param>
    /// <returns>Sorguya göre veriyi bulursa bool değer döndürür.</returns>
    public async Task<bool> AnyAsync(Expression<Func<SentMail, bool>> expression)
    {
        return await _sentMailRepository.AnyAsync(expression);
    }
    /// <summary>
    /// Verilen Id'ye göre maili database üzerinde siler.
    /// </summary>
    /// <param name="id">Silinecek gönderilen mailin Id'si</param>
    /// <returns>Result döndürür</returns>
    public async Task<IResult> DeleteAsync(Guid id)
    {
        var sentMail = await _sentMailRepository.GetByIdAsync(id);

        if (sentMail is null)
        {
            return new ErrorDataResult<SentMailDto>(Messages.EmailNotFound);
        }

        await _sentMailRepository.DeleteAsync(sentMail);
        await _sentMailRepository.SaveChangesAsync();

        return new SuccessResult(Messages.DeleteSuccess);
    }
    /// <summary>
    /// Verilen Id'lere göre mailleri toplu olarak databaseden siler
    /// </summary>
    /// <param name="ids">Silinecek gönderilen maillerin Id'leri</param>
    /// <returns>Result döndürür</returns>
    public async Task<IResult> DeleteRangeAsync(List<Guid> ids)
    {
        var sentMails = await _sentMailRepository.GetAllAsync();
        var deletedSentMail = sentMails.Where(sentMail => ids.Contains(sentMail.Id));

        if (deletedSentMail is null)
        {
            return new ErrorDataResult<SentMailDto>(Messages.EmailNotFound);
        }

        foreach (var sentMail in deletedSentMail)
        {
            await _sentMailRepository.DeleteAsync(sentMail);
        }

        await _sentMailRepository.SaveChangesAsync();

        return new SuccessResult(Messages.DeleteSuccess);
    }
    /// <summary>
    /// Verilen alıcı mailine göre gönderilen maillerin varlığını kontrol etmek için kullanılır.
    /// </summary>
    /// <param name="email">Maili alan kişinin email adresi</param>
    /// <returns>Emaile göre veriyi bulursa SentMailDto tipinde liste değer döndürür</returns>
    public async Task<IDataResult<List<SentMailListDto>>> GetAllByEmailAsync(string email)
    {
        var sentMails = await _sentMailRepository.GetAllAsync(x => x.Email == email);

        if (sentMails != null)
        {
            return new SuccessDataResult<List<SentMailListDto>>(_mapper.Map<List<SentMailListDto>>(sentMails), Messages.EmailFoundSuccess);
        }

        return new ErrorDataResult<List<SentMailListDto>>(Messages.EmailNotFound);
    }


    /// <summary>
    /// Verilen alıcı mailine göre gönderilen maillerin detaylarını kontrol etmek için kullanılır.
    /// </summary>
    /// <param name="email">Maili alan kişinin email adresi</param>
    /// <returns>Emaile göre detayları bulursa SentMailListDto tipinde liste değer döndürür</returns>
    public async Task<IDataResult<List<SentMailListDto>>> GetAllSentMailWithDetailsByEmailAsync(string email)
    {

        var sentMailList = await _sentMailRepository.GetAllAsync(x => x.Email == email);
        var student = _studentService.GetAllAsync().Result.Data.FirstOrDefault(student => student.Email == email);

        if (sentMailList.Count() > 0 && student != null)
        {
            var classRoomList = await _studentClassroomService.GetAllByStudentIdForStudentAsync(student.Id);
            var classroomIds = classRoomList.Data.Select(classroom => classroom.ClassroomId).ToList();

            var trainerClassroomList = await _trainerClassroomService.GetAllByExpression(trainerclass =>
                classroomIds.Contains(trainerclass.ClassroomId));
            var trainerClassRoomTrainerId = trainerClassroomList.Data != null ? trainerClassroomList.Data.Select(trainerClassroom => trainerClassroom.TrainerTalentIds.Select(id => id)).ToList() : null;
            var trainerList = await _trainerService.GetAllAsync();
            var trainerIds = trainerList.Data.Select(trainer => trainer.Id).ToList();
            List<TrainerListDto> trainerStudentList = new();
            if (trainerClassRoomTrainerId != null)
            {
                foreach (var item in trainerList.Data)
                {
                    if (trainerClassRoomTrainerId.Any(x => x.Contains(item.Id)))
                    {
                        trainerStudentList.Add(item);
                    }
                }
            }
            var sentMailsDto = _mapper.Map<List<SentMailListDto>>(sentMailList);
            var sentMaildetail = sentMailsDto.FirstOrDefault();
            if (sentMaildetail != null)
            {
                sentMaildetail.StudentFullName = student != null ? student.FirstName + " " + student.LastName : "İsmi Bulunamadı";
                sentMaildetail.LatestClassroom += classRoomList.Data != null && classRoomList.Data.Any() ? string.Join(", ", classRoomList.Data.Select(classroom => $"{classroom.ClassroomName}")) : "";
                sentMaildetail.LatestClassroomsTrainers += trainerStudentList != null && trainerStudentList.Any() ? string.Join(", ", trainerStudentList.Select(trainer => $"{trainer.FirstName + " " + trainer.LastName}")) : "";
            }
            return new SuccessDataResult<List<SentMailListDto>>(sentMailsDto, Messages.SentEmailFoundSuccess);
        }

        return new ErrorDataResult<List<SentMailListDto>>(Messages.EmailSendNotFound);
    }

    /// <summary>
    /// Verilen Id'ye göre gönderilen mailin varlığını kontrol etmek için kullanılır.
    /// </summary>
    /// <param name="id">Gönderilen mailin Id'si</param>
    /// <returns>Id'ye göre veriyi bulursa SentMailDto tipinde değer döndürür.</returns>
    public async Task<IDataResult<SentMail>> GetByIdAsync(Guid id)
    {
        var sentMail = await _sentMailRepository.GetByIdAsync(id, false);

        if (sentMail != null)
        {
            return new SuccessDataResult<SentMail>(_mapper.Map<SentMail>(sentMail), Messages.EmailFoundSuccess);
        }

        return new ErrorDataResult<SentMail>(Messages.EmailNotFound);
    }


}
