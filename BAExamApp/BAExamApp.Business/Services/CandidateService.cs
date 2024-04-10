using BAExamApp.Business.Interfaces.Services;
using BAExamApp.Core.Enums;
using BAExamApp.Dtos.Candidates;

namespace BAExamApp.Business.Services;
public class CandidateService : ICandidateService
{
    private readonly ICandidateRepository _candidateRepository;
    private readonly IMapper _mapper;

    public CandidateService(ICandidateRepository candidateRepository,IMapper mapper)
    {
        _candidateRepository = candidateRepository;
        _mapper = mapper;
    }

    /// <summary>
    /// Parametreden gelen değere göre öğrenciyi getirir.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<IDataResult<CandidateDto>> GetByIdAsync(Guid id)
    {
        var candidate = await _candidateRepository.GetByIdAsync(id);

        if (candidate is null)
        {
            return new ErrorDataResult<CandidateDto>(Messages.UserNotFound);
        }

        return new SuccessDataResult<CandidateDto>(_mapper.Map<CandidateDto>(candidate), Messages.FoundSuccess); ;
    }

    /// <summary>
    /// Candidates tablosundaki silinmemiş tüm aday öğrencilerin listesini döndüren metot.
    /// </summary>
    /// <returns></returns>
    public async Task<IDataResult<List<CandidateListDto>>> GetActiveCandidatesAsync()
    {
        var candidates = await _candidateRepository.GetAllAsync();

        var activeCandidates = candidates.Where(Candidate => Candidate.Status != Status.Deleted);

        var mappedActiveCandidates = _mapper.Map<List<CandidateListDto>>(activeCandidates);

        return mappedActiveCandidates.Any() ?
            new SuccessDataResult<List<CandidateListDto>>(mappedActiveCandidates, Messages.ListedSuccess) :
            new ErrorDataResult<List<CandidateListDto>>(Messages.CandidateNotFound);
    }

    /// <summary>
    /// Parametre olarak girilen ad, soyad veya mail adresine göre öğrenci listesindeki öğrencileri filtreleyen metot.
    /// </summary>
    /// <param name="name">Öğrenci adına karşılık gelen değişken</param>
    /// <param name="surname">Öğrenci soyadına karşılık gelen değişken</param>
    /// <param name="mailAdress">Öğrenci mail adresine karşılık gelen değişken</param>
    /// <returns></returns>
    public async Task<IDataResult<List<CandidateListDto>>> GetCandidatesByNameOrSurnameOrMailAdressAsync(string? name, string? surname, string? mailAdress)
    {
        var candidatesByName = await _candidateRepository.GetAllAsync(x => x.FirstName.Contains(name));
        var candidatesBySurname = await _candidateRepository.GetAllAsync(x => x.LastName.Contains(surname));
        var candidatesByMailAdress = await _candidateRepository.GetAllAsync(x => x.Email.Contains(mailAdress));

        var filteredCandidates = IntersectNonEmpty(candidatesByName, candidatesBySurname, candidatesByMailAdress);

        return filteredCandidates.Any()
            ? new SuccessDataResult<List<CandidateListDto>>(_mapper.Map<List<CandidateListDto>>(filteredCandidates), Messages.ListedSuccess)
            : new ErrorDataResult<List<CandidateListDto>>(Messages.CandidateNotFound);
    }

    /// <summary>
    /// Parametre olarak girilen listelerin içindeki ortak elemanlarını, listeler boşsa boş liste döndürür.
    /// </summary>
    /// <typeparam name="T">Listenin tipine karşılık gelen parametre</typeparam>
    /// <param name="lists">Listelere karşılık gelen parametre</param>
    /// <returns></returns>
    private static IEnumerable<T> IntersectNonEmpty<T>(params IEnumerable<T>[] lists)
    {
        var nonEmptyLists = lists.Where(list => list != null && list.Any()).ToList();

        if (nonEmptyLists.Count == 0)
        {
            return new List<T>();
        }

        IEnumerable<T> result = nonEmptyLists[0];

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

    /// <summary>
    /// Parametreden gelen değerlere göre yeni öğrenci eklemeyi sağlar
    /// </summary>
    /// <param name="candidateCandidateCreateDto"></param>
    /// <returns></returns>
    public async Task<IDataResult<CandidateDto>> AddAsync(CandidateCreateDto candidateCandidateCreateDto)
    {
        if (await _candidateRepository.AnyAsync(x => x.Email == candidateCandidateCreateDto.Email))
        {
            return new ErrorDataResult<CandidateDto>(Messages.EmailDuplicate);
        }

        Candidate candidate = _mapper.Map<Candidate>(candidateCandidateCreateDto);

        await _candidateRepository.AddAsync(candidate);
        await _candidateRepository.SaveChangesAsync();

        return new SuccessDataResult<CandidateDto>(_mapper.Map<CandidateDto>(candidate), Messages.AddSuccess);
    }

    /// <summary>
    /// Parametreden gelen değerlere göre ilgili öğrenciyi güncellemeyi sağlar
    /// </summary>
    /// <param name="candidateCandidateUpdateDto"></param>
    /// <returns></returns>
    public async Task<IDataResult<CandidateDto>> UpdateAsync(CandidateUpdateDto candidateCandidateUpdateDto)
    {
        var candidate = await _candidateRepository.GetByIdAsync(candidateCandidateUpdateDto.Id);

        if (candidate is null)
        {
            return new ErrorDataResult<CandidateDto>(Messages.CandidateNotFound);
        }

        var updatedCandidate = _mapper.Map(candidateCandidateUpdateDto, candidate);

        await _candidateRepository.UpdateAsync(updatedCandidate);
        await _candidateRepository.SaveChangesAsync();

        return new SuccessDataResult<CandidateDto>(_mapper.Map<CandidateDto>(updatedCandidate), Messages.UpdateSuccess);
    }

    public async Task<IDataResult<CandidateDetailsDto>> GetCandidateDetailsByIdAsync(Guid id)
    {
        var candidate = await _candidateRepository.GetByIdAsync(id);

        if (candidate is null)
        {
            return new ErrorDataResult<CandidateDetailsDto>(Messages.CandidateNotFound);
        }
        var candidateDetailsDto = new SuccessDataResult<CandidateDetailsDto>(_mapper.Map<CandidateDetailsDto>(candidate), Messages.CandidateFoundSuccess);

        return candidateDetailsDto;
    }

    public async Task<IResult> DeleteAsync(Guid id)
    {
        var candidate = await _candidateRepository.GetByIdAsync(id);
        if (candidate is null)
        {
            return new ErrorResult(Messages.UserNotFound);
        }

        await _candidateRepository.DeleteAsync(candidate);
        await _candidateRepository.SaveChangesAsync();

        return new SuccessResult(Messages.DeleteSuccessRedirect);
    }
}
