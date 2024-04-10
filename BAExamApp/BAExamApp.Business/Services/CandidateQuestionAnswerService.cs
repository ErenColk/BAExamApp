using BAExamApp.Dtos.CandidateQuestionAnswers;
using BAExamApp.Dtos.ClassroomProducts;
using BAExamApp.Dtos.QuestionAnswers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAExamApp.Business.Services;
public class CandidateQuestionAnswerService : ICandidateQuestionAnswerService
{
    private readonly ICandidateQuestionAnswerRepository _candidateQuestionAnswerRepository;
    private readonly IMapper _mapper;

    public CandidateQuestionAnswerService(ICandidateQuestionAnswerRepository candidateQuestionAnswerRepository, IMapper mapper)
    {
        _candidateQuestionAnswerRepository = candidateQuestionAnswerRepository;
        _mapper = mapper;
    }

    /// <summary>
    /// Verilen Id'ye göre CandidateQuestionAnswerDto modelini getirir.
    /// </summary>
    /// <param name="id">CandidateQuestionAnswer Id'si</param>
    /// <returns>Verilen Id'ye göre eğer modeli bulursa  CandidateQuestionAnswerDto tipinde model döndürür.</returns>
    public async Task<IDataResult<CandidateQuestionAnswerDto>> GetById(Guid id, bool traking = true)
    {
        var questionAnswer = await _candidateQuestionAnswerRepository.GetByIdAsync(id, traking);
        if (questionAnswer == null)
        {
            return new ErrorDataResult<CandidateQuestionAnswerDto>(Messages.QuestionAnswerNotFound);
        }

        return new SuccessDataResult<CandidateQuestionAnswerDto>(_mapper.Map<CandidateQuestionAnswerDto>(questionAnswer), Messages.FoundSuccess);

    }


    /// <summary>
    /// Verilen modele göre cevapları günceller
    /// </summary>
    /// <param name="questionAnswersUpdateDto">Güncellenecek bilgileri içeren soru cevabı modeli</param>
    /// <returns>Güncellenen sorunun cevaplarını geri döndürür</returns>
    public async Task<IDataResult<List<CandidateQuestionAnswerDto>>> Update(List<CandidateQuestionAnswerDto> questionAnswersUpdateDto)
    {
        if (questionAnswersUpdateDto.Count > 0)
        {

            var questionAnswers = await _candidateQuestionAnswerRepository.GetAllAsync(answer => answer.QuestionId == questionAnswersUpdateDto.Select(question => question.QuestionId).FirstOrDefault());

            var questionAnswersId = questionAnswersUpdateDto.Select(answers => answers.Id);

            var deletedAnswer = questionAnswers.Where(answer => !questionAnswersId.Contains(answer.Id));


            if (deletedAnswer.Any())
            {
                foreach (var answer in deletedAnswer)
                {
                    await _candidateQuestionAnswerRepository.DeleteAsync(answer);
                }

            }

            foreach (var updatedDto in questionAnswersUpdateDto)
            {
                var existingEntity = await _candidateQuestionAnswerRepository.GetAsync(x => x.Id == updatedDto.Id);


                if (existingEntity != null)
                {
                    // Güncelleme işlemi
                    var updatedAnswer = _mapper.Map(updatedDto, existingEntity);

                    await _candidateQuestionAnswerRepository.UpdateAsync(existingEntity);


                }
                else
                {
                    CandidateQuestionAnswer candidateQuestionAnswer = _mapper.Map<CandidateQuestionAnswer>(updatedDto);
                    await _candidateQuestionAnswerRepository.AddAsync(candidateQuestionAnswer);
                    if (candidateQuestionAnswer == null)
                    {
                        return new ErrorDataResult<List<CandidateQuestionAnswerDto>>();

                    }
                }
            }

            await _candidateQuestionAnswerRepository.SaveChangesAsync();



        }

        return new SuccessDataResult<List<CandidateQuestionAnswerDto>>();
    }

    /// <summary>
    /// Verilen modele göre soruya bütün cevapları günceller
    /// </summary>
    /// <param name="questionAnswersUpdateDto">Güncellenecek bilgileri içeren soruya ait cevapların modeli</param>
    /// <returns>Güncellenen soruya ait cevapların modelini döndürür </returns>
    public async Task<IDataResult<List<CandidateQuestionAnswerDto>>> UpdateRangeAsync(List<CandidateQuestionAnswerCreateDto> questionAnswersUpdateDto)
    {
        if (questionAnswersUpdateDto.Count > 0)
        {
            var CurrentQuestionAnswers = await _candidateQuestionAnswerRepository.GetAllAsync(x => x.QuestionId == questionAnswersUpdateDto[0].QuestionId);
            await DeleteRangeAsync(CurrentQuestionAnswers.Select(x => x.Id).ToList());
        }

        return await AddRangeAsync(questionAnswersUpdateDto);
    }





    /// <summary>
    /// Verilen id'lere göre sorunun cevaplarını veritabanından siler
    /// </summary>
    /// <param name="ids">Silinecek soru cevaplarının id'leri</param>
    /// <returns></returns>
    public async Task<IResult> DeleteRangeAsync(List<Guid> ids)
    {
        foreach (var id in ids)
        {
            var questionAnswer = await _candidateQuestionAnswerRepository.GetByIdAsync(id);

            if (questionAnswer is null)
            {
                return new ErrorDataResult<ClassroomProductDto>(Messages.ClassroomProductNotFound);
            }

            await _candidateQuestionAnswerRepository.DeleteAsync(questionAnswer);
        }

        await _candidateQuestionAnswerRepository.SaveChangesAsync();

        return new SuccessResult(Messages.DeleteSuccess);
    }


    /// <summary>
    /// Verilen modele göre sorunun cevaplarını ekler
    /// </summary>
    /// <param name="questionAnswersCreateDto">Eklenecek bilgileri içeren sorunun cevaplarının modeli</param>
    /// <returns>Eklenen sorunun cevaplarını geri döndürür</returns>
    public async Task<IDataResult<List<CandidateQuestionAnswerDto>>> AddRangeAsync(List<CandidateQuestionAnswerCreateDto> questionAnswersCreateDto)
    {
        var questionAnswers = new List<CandidateQuestionAnswer>();

        foreach (var questionAnswerCreateDto in questionAnswersCreateDto)
        {
            var questionAnswer = _mapper.Map<CandidateQuestionAnswer>(questionAnswerCreateDto);

            await _candidateQuestionAnswerRepository.AddAsync(questionAnswer);

            questionAnswers.Add(questionAnswer);
        }
        await _candidateQuestionAnswerRepository.SaveChangesAsync();

        return new SuccessDataResult<List<CandidateQuestionAnswerDto>>(_mapper.Map<List<CandidateQuestionAnswerDto>>(questionAnswers), Messages.AddSuccess);
    }
}

