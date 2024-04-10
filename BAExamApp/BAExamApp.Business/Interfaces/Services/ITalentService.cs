using BAExamApp.Dtos.Talents;

namespace BAExamApp.Business.Interfaces.Services;
public interface ITalentService
{
    /// <summary>
    /// Eklenecek olan yetenek verilerini ekler
    /// </summary>
    /// <param name="talentCreateDto"> Eklecenek olan model</param>
    /// <returns>Eklenecek olana yetenek verilerini döner </returns>
    Task<IDataResult<TalentDto>> AddAsync(TalentCreateDto talentCreateDto);
    /// <summary>
    /// yetenek verisini getirir
    /// <returns>yetenek listesi döndürür.</returns>
    Task<IDataResult<List<TalentListDto>>> GetAllAsync();
    /// <summary>
    /// Verilen id ile eşleşen yetenel verisini getirir
    /// </summary>
    /// <param name="id">Talent Id</param>
    /// <returns>Gönderilen id ile eşleşen yetenek nesnesini döndürür.</returns>
    Task<IDataResult<TalentDto>> GetByIdAsync(Guid id);
    /// <summary>
    /// Verilen id ile eşleşen yeteneği bir eğitmenin kullanıp kullanmadığını kontrol eder. Eğer kullanmıyorsa siler.
    /// </summary>
    /// <param name="id">Talent Id</param>
    /// <returns>IResult</returns>
    Task<IResult> DeleteAsync(Guid id);
    

}
