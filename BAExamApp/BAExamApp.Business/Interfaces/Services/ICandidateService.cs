using BAExamApp.Dtos.Candidates;

namespace BAExamApp.Business.Interfaces.Services;
public interface ICandidateService
{
    /// <summary>
    /// Candidates tablosundaki silinmemiş tüm öğrencilerin listesini döndüren metot.
    /// </summary>
    /// <returns></returns>
    Task<IDataResult<List<CandidateListDto>>> GetActiveCandidatesAsync();

    /// <summary>
    /// Parametre olarak girilen ad, soyad veya mail adresine göre öğrenci listesindeki öğrencileri filtreleyen metot.
    /// </summary>
    /// <param name="name">Öğrenci adına karşılık gelen değişken</param>
    /// <param name="surname">Öğrenci soyadına karşılık gelen değişken</param>
    /// <param name="mailAdress">Öğrenci mail adresine karşılık gelen değişken</param>
    /// <returns></returns>
    Task<IDataResult<List<CandidateListDto>>> GetCandidatesByNameOrSurnameOrMailAdressAsync(string? name, string? surname, string? mailAdress);

    /// <summary>
    /// Parametreden gelen değerlere göre yeni öğrenci eklemeyi sağlar
    /// </summary>
    /// <param name="candidateCandidateCreateDto"></param>
    /// <returns></returns>
    Task<IDataResult<CandidateDto>> AddAsync(CandidateCreateDto candidateCandidateCreateDto);

    /// <summary>
    /// Parametreden gelen değerlere göre ilgili öğrenciyi güncellemeyi sağlar
    /// </summary>
    /// <param name="candidateCandidateUpdateDto"></param>
    /// <returns></returns>
    Task<IDataResult<CandidateDto>> UpdateAsync(CandidateUpdateDto candidateCandidateUpdateDto);

    /// <summary>
    /// Parametreden gelen değere göre öğrenciyi getirir.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<IDataResult<CandidateDto>> GetByIdAsync(Guid id);

    /// <summary>
    /// Parametreden gelen değen göre öğrenici bilgilerini detaylı olarak getirir.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<IDataResult<CandidateDetailsDto>> GetCandidateDetailsByIdAsync(Guid id);

    /// <summary>
    /// Parametreden gelen değere göre öğrenciyi siler.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<IResult> DeleteAsync(Guid id);
}
