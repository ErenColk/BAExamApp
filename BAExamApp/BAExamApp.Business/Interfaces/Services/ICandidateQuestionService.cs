using BAExamApp.Dtos.CandidateQuestions;
using BAExamApp.Dtos.Questions;
using BAExamApp.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAExamApp.Business.Interfaces.Services;
public interface ICandidateQuestionService
{


    /// <summary>
    /// Silinmemiş tüm soruları getirir.
    /// </summary>
    /// <returns>Geri dönüş tipi CandidateQuestionListDto modelidir.</returns>
    Task<IDataResult<List<CandidateQuestionListDto>>> GetAllAsync();

    /// <summary>
    /// Verilen Id'ye göre CandidateQuestionDto modelini getirir.
    /// </summary>
    /// <param name="id">CandidateQuestion Id'si</param>
    /// <returns>Verilen Id'ye göre eğer modeli bulursa  CandidateQuestionDto tipinde model döndürür.</returns>
    Task<IDataResult<CandidateQuestionDto>> GetByIdAsync(Guid id);


    /// <summary>
    /// Verilen ıd'ye göre CandidateQuestion modelini veritabanından siler
    /// </summary>
    /// <param name="questionId">Silinecek olan CandidateQuestion Id'si</param>
    /// <returns></returns>
    Task<IResult> DeleteAsync(Guid questionId);

    /// <summary>
    /// Verilen Id'ye göre CandidateQuestionDetailsDto modelini getirir.
    /// </summary>
    /// <param name="id">CandidateQuestion Id'si</param>
    /// <returns>Verilen Id'ye göre eğer modeli bulursa  CandidateQuestionDetailsDto tipinde model döndürür.</returns>
    Task<IDataResult<CandidateQuestionDetailsDto>> GetDetailsByIdAsync(Guid id);

    /// <summary>
    /// Soruyu database üzerinde günceller.
    /// </summary>
    /// <param name="candidateQuestionUpdateDto">Güncellenecek bilgileri içeren soru modeli</param>
    /// <returns>Güncellenen soruyu döndürür</returns>
    Task<IDataResult<CandidateQuestionDto>> UpdateAsync(CandidateQuestionUpdateDto  candidateQuestionUpdateDto);

    /// <summary>
    /// Verilen parametlere göre CandidateQuestion verisinde filtreleme yaparak geriye değer döndürür.
    /// </summary>
    /// <param name="content">Aranacak sorunun içeriği</param>
    /// <param name="candidateQuestionType">Aranacak sorunun, soru tipi</param>
    /// <param name="candidateQuestionCreatedDate">Aranacak sorunun başlangıç tarihi</param>
    /// <returns>Geri dönüş tipi CandidateQuestionListDto modelidir.</returns>
    Task<IDataResult<List<CandidateQuestionListDto>>> GetQuestionBySearchValues(string? content, string? candidateQuestionType,  string? candidateQuestionCreatedDate);

    /// <summary>
    /// Soruyu database üzerinde ekler. 
    /// </summary>
    /// <param name="candidateQuestionCreateDto">Eklenecek bilgileri içeren soru modeli</param>
    /// <returns>Eklenen soruyu döndürür</returns>
    Task<IDataResult<CandidateQuestion>> AddAsync(CandidateQuestionCreateDto candidateQuestionCreateDto);
}
