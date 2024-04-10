using BAExamApp.Dtos.CandidateQuestionAnswers;
using BAExamApp.Dtos.QuestionAnswers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAExamApp.Business.Interfaces.Services;
public interface ICandidateQuestionAnswerService
{


    /// <summary>
    /// Verilen Id'ye göre CandidateQuestionAnswerDto modelini getirir.
    /// </summary>
    /// <param name="id">CandidateQuestionAnswer Id'si</param>
    /// <returns>Verilen Id'ye göre eğer modeli bulursa  CandidateQuestionAnswerDto tipinde model döndürür.</returns>
    Task<IDataResult<CandidateQuestionAnswerDto>> GetById(Guid id, bool traking = true);


    /// <summary>
    /// Verilen modele göre cevapları günceller
    /// </summary>
    /// <param name="questionAnswersUpdateDto">Güncellenecek bilgileri içeren sorunun cevaplarının modeli</param>
    /// <returns>Güncellenen sorunun cevaplarını geri döndürür</returns>
    Task<IDataResult<List<CandidateQuestionAnswerDto>>> Update(List<CandidateQuestionAnswerDto> questionAnswersUpdateDto);

    /// <summary>
    /// Verilen modele göre sorunun cevaplarını ekler
    /// </summary>
    /// <param name="questionAnswersCreateDto">Eklenecek bilgileri içeren sorunun cevaplarının modeli</param>
    /// <returns>Eklenen sorunun cevaplarını geri döndürür</returns>
    Task<IDataResult<List<CandidateQuestionAnswerDto>>> AddRangeAsync(List<CandidateQuestionAnswerCreateDto> questionAnswersCreateDto);


    /// <summary>
    /// Verilen id'lere göre sorunun cevaplarını veritabanından siler
    /// </summary>
    /// <param name="ids">Silinecek soru cevaplarının id'leri</param>
    /// <returns></returns>
    Task<IResult> DeleteRangeAsync(List<Guid> ids);

    /// <summary>
    /// Verilen modele göre soruya bütün cevapları günceller
    /// </summary>
    /// <param name="questionAnswersUpdateDto">Güncellenecek bilgileri içeren soruya ait cevapların modeli</param>
    /// <returns>Güncellenen soruya ait cevapların modelini döndürür </returns>
    Task<IDataResult<List<CandidateQuestionAnswerDto>>> UpdateRangeAsync(List<CandidateQuestionAnswerCreateDto> questionAnswersUpdateDto);


}
