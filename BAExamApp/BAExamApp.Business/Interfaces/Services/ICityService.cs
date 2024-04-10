using BAExamApp.Dtos.Branches;
using BAExamApp.Dtos.Cities;

namespace BAExamApp.Business.Interfaces.Services;

public interface ICityService
{
    /// <summary>
    /// Mevcut şehirlere erişimi sağlar 
    /// </summary>
    /// <returns>Mevcut şehirleri liste olarak döner.</returns>
    Task<IDataResult<List<CityListDto>>> GetAllAsync();
    /// <summary>
    /// Yeni şehir eklememizi sağlar. 
    /// </summary>
    /// <param name="cityCreateDto">Eklecenek olan model</param>
    /// <returns>Eğer aynı isimde Şehir var ise Error Result ve mesajını döner ve daha sonra ekleme işleminin Error Result veya Success Result değerlerinin ve mesajlarını döner.</returns>
    Task<IDataResult<CityDto>> AddAsync(CityCreateDto cityCreateDto);
    Task<IDataResult<CityDto>> UpdateAsync(CityUpdateDto cityUpdateDto);
    Task<IDataResult<CityDto>> GetByIdAsync(Guid id);
    Task<IResult> DeleteAsync(Guid id);
}