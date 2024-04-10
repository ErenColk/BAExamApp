using BAExamApp.Dtos.Emails;
using BAExamApp.Dtos.SentMails;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BAExamApp.Business.Interfaces.Services;
public interface ISentMailService
{
    /// <summary>
    /// Verilen sorguya göre gönderilen mailin varlığını kontrol etmek için kullanılır.
    /// </summary>
    /// <param name="expression">Atılacak olan sorgu </param>
    /// <returns>Sorguya göre veriyi bulursa bool değer döndürür.</returns>
    Task<bool> AnyAsync(Expression<Func<SentMail, bool>> expression);
    /// <summary>
    /// Verilen Id'ye göre gönderilen mailin varlığını kontrol etmek için kullanılır.
    /// </summary>
    /// <param name="id">Gönderilen mailin Id'si</param>
    /// <returns>Id'ye göre veriyi bulursa SentMailDto tipinde değer döndürür.</returns>
    Task<IDataResult<SentMail>> GetByIdAsync(Guid id);
    /// <summary>
    /// Verilen alıcı mailine göre gönderilen maillerin varlığını kontrol etmek için kullanılır.
    /// </summary>
    /// <param name="email">Maili alan kişinin email adresi</param>
    /// <returns>Emaile göre veriyi bulursa SentMailListDto tipinde liste değer döndürür</returns>
    Task<IDataResult<List<SentMailListDto>>> GetAllByEmailAsync(string email);

    /// <summary>
    /// Verilen alıcı mailine göre gönderilen maillerin detaylarını kontrol etmek için kullanılır.
    /// </summary>
    /// <param name="email">Maili alan kişinin email adresi</param>
    /// <returns>Emaile göre detayları bulursa SentMailListDto tipinde liste değer döndürür</returns>
    Task<IDataResult<List<SentMailListDto>>> GetAllSentMailWithDetailsByEmailAsync(string email);



    /// <summary>
    /// Gönderilen maili database üzerinde kaydeder.
    /// </summary>
    /// <param name="sentMailCreateDto">Gönderilen mail modeli </param>
    /// <returns>Kaydetme başarılı olduğunda SentMailDto tipinde değer döndürür.</returns>
    Task<IDataResult<SentMailDto>> AddAsync(SentMailCreateDto sentMailCreateDto);

    /// <summary>
    /// Gönderilen maili database üzerinde günceller.
    /// </summary>
    /// <param name="sentMailUpdate">Gönderilen mail modeli</param>
    /// <returns>Güncelleme başarılı olduğunda SentMailUpdateDto tipinde değer döndürür.</returns>
    Task<IDataResult<SentMailUpdateDto>> UpdateAsync(SentMailUpdateDto sentMailUpdate);
    /// <summary>
    /// Verilen Id'ye göre maili database üzerinde siler.
    /// </summary>
    /// <param name="id">Silinecek gönderilen mailin Id'si</param>
    /// <returns>Result döndürür</returns>
    Task<IResult> DeleteAsync(Guid id);
    /// <summary>
    /// Verilen Id'lere göre mailleri toplu olarak databaseden siler
    /// </summary>
    /// <param name="ids">Silinecek gönderilen maillerin Id'leri</param>
    /// <returns>Result döndürür</returns>
    Task<IResult> DeleteRangeAsync(List<Guid> ids);


}
