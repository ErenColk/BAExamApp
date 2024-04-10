using BAExamApp.Core.Enums;
using BAExamApp.Dtos.CandidateQuestions;
using BAExamApp.Dtos.Questions;
using BAExamApp.Entities.DbSets;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAExamApp.Business.Services;
public class CandidateQuestionService : ICandidateQuestionService
{
    private readonly ICandidateQuestionRepository _candidateQuestionRepository;
    private readonly IMapper _mapper;
    private readonly ICandidateQuestionAnswerRepository _candidateQuestionAnswerRepository;

    public CandidateQuestionService(ICandidateQuestionRepository candidateQuestionRepository, IMapper mapper,ICandidateQuestionAnswerRepository candidateQuestionAnswerRepository)
    {
        _candidateQuestionRepository = candidateQuestionRepository;
        _mapper = mapper;
        _candidateQuestionAnswerRepository = candidateQuestionAnswerRepository;
    }

    /// <summary>
    /// Silinmemiş tüm soruları getirir.
    /// </summary>
    /// <returns>Geri dönüş tipi CandidateQuestionListDto modelidir.</returns>
    public async Task<IDataResult<List<CandidateQuestionListDto>>> GetAllAsync()
    {
        var questions = await _candidateQuestionRepository.GetAllAsync();
        if (questions == null)
        {
            return new ErrorDataResult<List<CandidateQuestionListDto>>(Messages.ListNotFound);

        }
        return new SuccessDataResult<List<CandidateQuestionListDto>>(_mapper.Map<List<CandidateQuestionListDto>>(questions), Messages.ListedSuccess);
    }



    /// <summary>
    /// Verilen parametlere göre CandidateQuestion verisinde filtreleme yaparak geriye değer döndürür.
    /// </summary>
    /// <param name="content">Aranacak sorunun içeriği</param>
    /// <param name="candidateQuestionType">Aranacak sorunun, soru tipi</param>
    /// <param name="candidateQuestionCreatedDate">Aranacak sorunun başlangıç tarihi</param>
    /// <returns>Geri dönüş tipi CandidateQuestionListDto modelidir.</returns>
    public async Task<IDataResult<List<CandidateQuestionListDto>>> GetQuestionBySearchValues(string? content, string? candidateQuestionType, string? candidateQuestionCreatedDate)
    {
        int nullParamCount = new[] { content, candidateQuestionType }.Count(param => param != null && param != "");
        var candidateQuestionCreatedYear = DateTime.Parse(candidateQuestionCreatedDate).Year.ToString();

        if (DateTime.Parse(candidateQuestionCreatedDate).Year.ToString() != "1")
        {
            nullParamCount++;
        }

        var candidateQuestionList = await _candidateQuestionRepository.GetAllAsync();

        var candidateQuestionsByContent = content != null ? candidateQuestionList.Where(x => x.Content.ToLower().Contains(content)).ToList() : new List<CandidateQuestion>();
        var candidateQuestionsBySubject = candidateQuestionType != "" ? candidateQuestionList.Where(x => x.CandidateQuestionType.ToString() == candidateQuestionType).ToList() : new List<CandidateQuestion>();

        var candidateQuestionsByCreatedDate = candidateQuestionCreatedYear != "1" ? candidateQuestionList.Where(x => x.CreatedDate.Day == DateTime.Parse(candidateQuestionCreatedDate).Day && x.CreatedDate.Month == DateTime.Parse(candidateQuestionCreatedDate).Month && x.CreatedDate.Year == DateTime.Parse(candidateQuestionCreatedDate).Year).ToList() : new List<CandidateQuestion>();


        //var candidateQuestionsByCreatedDate = candidateQuestionCreatedYear != "1" ? candidateQuestionList.Where(x => EF.Functions.DateDiffDay(x.CreatedDate, DateTime.Parse(candidateQuestionCreatedDate)) == 0).ToList()  : new List<CandidateQuestion>();

        var filteredCandidateQuestions = IntersectNonEmpty(nullParamCount, candidateQuestionsByContent, candidateQuestionsBySubject, candidateQuestionsByCreatedDate);

        return filteredCandidateQuestions.Any()
            ? new SuccessDataResult<List<CandidateQuestionListDto>>(_mapper.Map<List<CandidateQuestionListDto>>(filteredCandidateQuestions), Messages.ListedSuccess)
            : new ErrorDataResult<List<CandidateQuestionListDto>>(Messages.QuestionNotFound);
    }




    /// <summary>
    /// Farklı filtrelenmiş modelleri karşılaştırarak, belirtilen sayıda null olmayan filtrelerin kesişimini bulur.
    /// </summary>
    /// <typeparam name="T">Filtrelenmiş modellerin türü.</typeparam>
    /// <param name="_nullParamCount">Null olmayan filtrelerin sayısı.</param>
    /// <param name="lists">Filtrelenmiş modellerin listesi.</param>
    /// <returns>Belirtilen sayıda null olmayan filtrelerin kesişimi.</returns>
    private static IEnumerable<T> IntersectNonEmpty<T>(int _nullParamCount, params IEnumerable<T>[] lists)
    {
        var nonEmptyLists = lists.Where(list => list != null && list.Any()).ToList();

        if (nonEmptyLists.Count == 0)
        {
            return new List<T>();
        }

        IEnumerable<T> result = nonEmptyLists[0];
        if (_nullParamCount == nonEmptyLists.Count)
        {
            for (int i = 1; i < nonEmptyLists.Count; i++)
            {
                result = result.Intersect(nonEmptyLists[i]).ToList();

                if (!result.Any())
                {
                    break;
                }
            }
            return result;
        }
        else
        {

            return result = new List<T>();
        }


    }


    /// <summary>
    /// Parametreye gönderilen model ile CandidateQuestion oluşturur. 
    /// </summary>
    /// <param name="candidateQuestionCreateDto">CandidateQuestionCreateDto tipinde model</param>
    /// <returns>Ekleme başarılı olduğunda CandidateQuestionCreateDto tipinde model döndürür.</returns>
    public async Task<IDataResult<CandidateQuestion>> AddAsync(CandidateQuestionCreateDto candidateQuestionCreateDto)
    {
        var question = _mapper.Map<CandidateQuestion>(candidateQuestionCreateDto);

        await _candidateQuestionRepository.AddAsync(question);
        var addResult = await _candidateQuestionRepository.SaveChangesAsync();

        if (addResult > 0)
        {

            return new SuccessDataResult<CandidateQuestion>(_mapper.Map<CandidateQuestion>(question), Messages.AddSuccess);
        }
        else
        {
            return new ErrorDataResult<CandidateQuestion>(Messages.AddError);

        }

    }



    /// <summary>
    /// Verilen Id'ye göre CandidateQuestionDto modelini getirir.
    /// </summary>
    /// <param name="id">CandidateQuestion Id'si</param>
    /// <returns>Verilen Id'ye göre eğer modeli bulursa  CandidateQuestionDto tipinde model döndürür.</returns>
    public async Task<IDataResult<CandidateQuestionDto>> GetByIdAsync(Guid id)
    {
        var question = await _candidateQuestionRepository.GetByIdAsync(id);
        question.QuestionAnswers = question.QuestionAnswers.Where(answers => answers.Status != Status.Deleted).ToList();
        if (question == null)
        {
            return new ErrorDataResult<CandidateQuestionDto>(Messages.QuestionNotFound);
        }

        var questionDto = _mapper.Map<CandidateQuestionDto>(question);

        return new SuccessDataResult<CandidateQuestionDto>(questionDto, Messages.QuestionFoundSuccess);
    }

    /// <summary>
    /// Soruyu database üzerinde günceller.
    /// </summary>
    /// <param name="candidateQuestionUpdateDto">Güncellenecek bilgileri içeren soru modeli</param>
    /// <returns>Güncellenen soruyu döndürür</returns>
    public async Task<IDataResult<CandidateQuestionDto>> UpdateAsync(CandidateQuestionUpdateDto candidateQuestionUpdateDto)
    {
        var question = await _candidateQuestionRepository.GetAsync(src => src.Id == candidateQuestionUpdateDto.Id);
        

        var questionAnswerList = await _candidateQuestionAnswerRepository.GetAllAsync(src => src.QuestionId == question.Id);
        if (question is null)
            return new ErrorDataResult<CandidateQuestionDto>(Messages.QuestionNotFound);

        var updatedQuestion = _mapper.Map(candidateQuestionUpdateDto, question);
        updatedQuestion.QuestionAnswers = questionAnswerList.ToList();
        await _candidateQuestionRepository.UpdateAsync(updatedQuestion);
        await _candidateQuestionRepository.SaveChangesAsync();

        return new SuccessDataResult<CandidateQuestionDto>(_mapper.Map<CandidateQuestionDto>(updatedQuestion), Messages.UpdateSuccess);

    }

    /// <summary>
    /// Verilen Id'ye göre CandidateQuestionDetailsDto modelini getirir.
    /// </summary>
    /// <param name="id">CandidateQuestion Id'si</param>
    /// <returns>Verilen Id'ye göre eğer modeli bulursa  CandidateQuestionDetailsDto tipinde model döndürür.</returns>
    public async Task<IDataResult<CandidateQuestionDetailsDto>> GetDetailsByIdAsync(Guid id)
    {
        var question = await _candidateQuestionRepository.GetByIdAsync(id);

        if (question is null)
        {
            return new ErrorDataResult<CandidateQuestionDetailsDto>(Messages.QuestionNotFound);
        }

        return new SuccessDataResult<CandidateQuestionDetailsDto>(_mapper.Map<CandidateQuestionDetailsDto>(question), Messages.FoundSuccess);


    }


    /// <summary>
    /// Verilen ıd'ye göre CandidateQuestion modelini veritabanından siler
    /// </summary>
    /// <param name="questionId">Silinecek olan CandidateQuestion Id'si</param>
    /// <returns></returns>
    public async Task<IResult> DeleteAsync(Guid questionId)
    {
        var question = await _candidateQuestionRepository.GetByIdAsync(questionId);
        var questionAnswer = await _candidateQuestionAnswerRepository.GetAllAsync(answers => answers.QuestionId == questionId);
        if (question is null)
        {
            return new ErrorResult(Messages.QuestionNotFound);
        }
        await _candidateQuestionAnswerRepository.DeleteRangeAsync(questionAnswer);
        await _candidateQuestionRepository.DeleteAsync(question);
        await _candidateQuestionRepository.SaveChangesAsync();

        return new SuccessResult(Messages.DeleteSuccess);
    }
}
