using BAExamApp.Dtos.Talents;

namespace BAExamApp.Business.Interfaces.Services;
public interface ITrainerTalentService
{
    /// <summary>
    /// Eğitmene ait yetenekleri döner.
    /// </summary>
    /// <param name="trainerId">İlgili yetenekleri bulmak için trainerId gereklidir.</param>
    /// <returns>TalentListDto</returns>
    Task<IDataResult<List<TalentListDto>>> GetTrainersTalents(Guid trainerId);
}
