
using BAExamApp.Business.Interfaces.Services;
using BAExamApp.Core.Enums;
using BAExamApp.Dtos.Admins;
using BAExamApp.Dtos.CandidateAdmins;
using BAExamApp.Dtos.Trainers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BAExamApp.Business.Services;
public class CandidateAdminService : ICandidateAdminService
{
    private readonly ICandidateAdminRepository _candidateAdminRepository;
    private readonly IMapper _mapper;
    private readonly IAccountService _accountService;

    public CandidateAdminService(ICandidateAdminRepository candidateAdminRepository, IMapper mapper , IAccountService accountService)
    {
        _candidateAdminRepository = candidateAdminRepository;
        _mapper = mapper;
        _accountService = accountService;
    }

    /// <summary>
    /// Aday admini olarak aday admini ekleme işlemi.
    /// </summary>
    /// <param name="candidateAdminCreateDto"></param>
    /// <returns></returns>
    public async Task<IDataResult<CandidateAdminDto>> AddAsync(CandidateAdminCreateDto candidateAdminCreateDto)
    {
        if (await _accountService.AnyAsync(x => x.Email == candidateAdminCreateDto.Email))
        {
            return new ErrorDataResult<CandidateAdminDto>(Messages.EmailDuplicate);
        }

        IdentityUser identityUser = new()
        {
            Email = candidateAdminCreateDto.Email,
            UserName = candidateAdminCreateDto.Email,
            EmailConfirmed = true, // TODO: Email confirmation yapılırsa burası değiştirilmeli.
        };

        DataResult<CandidateAdminDto> result = new ErrorDataResult<CandidateAdminDto>();

        var strategy = await _candidateAdminRepository.CreateExecutionStrategy();

        await strategy.ExecuteAsync(async () =>
        {
            using var transactionScope = await _candidateAdminRepository.BeginTransactionAsync().ConfigureAwait(false);
            try
            {
                var identityResult = await _accountService.CreateUserAsync(identityUser, Roles.Admin);
                if (!identityResult.Succeeded)
                {
                    result = new ErrorDataResult<CandidateAdminDto>(identityResult.ToString());
                    transactionScope.Rollback();
                    return;
                }

                CandidateAdmin candidateAdmin = _mapper.Map<CandidateAdmin>(candidateAdminCreateDto);
                candidateAdmin.IdentityId = identityUser.Id;
                

                await _candidateAdminRepository.AddAsync(candidateAdmin);
                await _candidateAdminRepository.SaveChangesAsync();

                result = new SuccessDataResult<CandidateAdminDto>(_mapper.Map<CandidateAdminDto>(candidateAdmin), Messages.AddSuccess);
                transactionScope.Commit();
            }
            catch (Exception ex)
            {
                result = new ErrorDataResult<CandidateAdminDto>($"{Messages.AddFail} - {ex.Message}");
                transactionScope.Rollback();
            }
            finally
            {
                transactionScope.Dispose();
            }
        });

        return result;
    }

    /// <summary>
    /// Aday admini olarak aday admini silme işlemi.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<IResult> DeleteAsync(Guid id)
    {
        var admin = await _candidateAdminRepository.GetByIdAsync(id);

        if (admin == null)
        {
            return new ErrorResult(Messages.UserNotFound);
        }

        var deleteIdentityResult = await _accountService.DeleteUserAsync(admin.IdentityId!);

        if (!deleteIdentityResult.Succeeded)
        {
            return new ErrorResult(deleteIdentityResult.ToString());
        }

        await _candidateAdminRepository.DeleteAsync(admin);
        await _candidateAdminRepository.SaveChangesAsync();

        return new SuccessResult(Messages.DeleteSuccess);
    }


    /// <summary>
    /// Aday admini olarak tüm aday adminlerini çağırma işlemi
    /// </summary>
    /// <returns></returns>
    public async Task<IDataResult<List<CandidateAdminListDto>>> GetAllAsync()
    {
        var admins = await _candidateAdminRepository.GetAllAsync();

        return new SuccessDataResult<List<CandidateAdminListDto>>(_mapper.Map<List<CandidateAdminListDto>>(admins), Messages.ListedSuccess);
    }


    /// <summary>
    /// Id'ye göre aday admini çağırma işlemi
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<IDataResult<CandidateAdminDto>> GetByIdAsync(Guid id)
    {
        var admin = await _candidateAdminRepository.GetByIdAsync(id);

        if (admin is null)
        {
            return new ErrorDataResult<CandidateAdminDto>(Messages.UserNotFound);
        }

        return new SuccessDataResult<CandidateAdminDto>(_mapper.Map<CandidateAdminDto>(admin), Messages.ListedSuccess);
    }

    /// <summary>
    /// Aday admin'inin identityId'sine göre çağırılma işlemi. 
    /// </summary>
    /// <param name="identityId"></param>
    /// <returns></returns>
    public async Task<IDataResult<CandidateAdminDto>> GetByIdentityIdAsync(string identityId)
    {
        var candidateAdmin = await _candidateAdminRepository.GetAsync(x => x.IdentityId == identityId);

        if (candidateAdmin is null) return new ErrorDataResult<CandidateAdminDto>(Messages.UserNotFound);

        return new SuccessDataResult<CandidateAdminDto>(_mapper.Map<CandidateAdminDto>(candidateAdmin), Messages.FoundSuccess);
    }


    /// <summary>
    /// Aday admin'inin id sine göre çağırılma işlemi.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<IDataResult<CandidateAdminDetailsDto>> GetDetailsByIdAsync(Guid id)
    {
        var admin = await _candidateAdminRepository.GetByIdAsync(id);

        if (admin is null)
        {
            return new ErrorDataResult<CandidateAdminDetailsDto>(Messages.UserNotFound);
        }

        return new SuccessDataResult<CandidateAdminDetailsDto>(_mapper.Map<CandidateAdminDetailsDto>(admin), Messages.FoundSuccess);
    }


    /// <summary>
    /// Aday yöneticisi olarak, aday yöneticisi güncelleme işlemi.
    /// </summary>
    /// <param name="candidateAdminUpdateDto"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task<IDataResult<CandidateAdminDto>> UpdateAsync(CandidateAdminUpdateDto candidateAdminUpdateDto)
    {
        var candidateAdmin = await _candidateAdminRepository.GetByIdAsync(candidateAdminUpdateDto.Id);

        if (candidateAdmin is null)
        {
            return new ErrorDataResult<CandidateAdminDto>(Messages.UserNotFound);
        }

        var updatedCandidateAdmin = _mapper.Map(candidateAdminUpdateDto, candidateAdmin);

        await _candidateAdminRepository.UpdateAsync(updatedCandidateAdmin);
        await _candidateAdminRepository.SaveChangesAsync();

        return new SuccessDataResult<CandidateAdminDto>(_mapper.Map<CandidateAdminDto>(updatedCandidateAdmin), Messages.UpdateSuccess);
    }
}
