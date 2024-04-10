using BAExamApp.Dtos.ClassroomProducts;
using BAExamApp.Dtos.QuestionAnswers;
using Castle.Components.DictionaryAdapter.Xml;

namespace BAExamApp.Business.Services;
public class QuestionAnswerService : IQuestionAnswerService
{
    private readonly IQuestionAnswerRepository _questionAnswerRepository;
    private readonly IMapper _mapper;
    private readonly IStudentAnswerRepository _studentAnswerRepository;

    public QuestionAnswerService(IQuestionAnswerRepository questionAnswerRepository, IMapper mapper)
    {
        _questionAnswerRepository = questionAnswerRepository;
        _mapper = mapper;
    }

    public async Task<IDataResult<QuestionAnswerDto>> GetById(Guid id,bool traking=true)
    {
        var questionAnswer = await _questionAnswerRepository.GetByIdAsync(id,traking);
        if (questionAnswer == null)
        {
            return new ErrorDataResult<QuestionAnswerDto>(Messages.QuestionAnswerNotFound);
        }

        return new SuccessDataResult<QuestionAnswerDto>(_mapper.Map<QuestionAnswerDto>(questionAnswer), Messages.FoundSuccess);
    }

    public async Task<IDataResult<QuestionAnswerDto>> AddAsync(QuestionAnswerCreateDto questionAnswerCreateDto)
    {
        var questionAnswer = _mapper.Map<QuestionAnswer>(questionAnswerCreateDto);

        await _questionAnswerRepository.AddAsync(questionAnswer);
        await _questionAnswerRepository.SaveChangesAsync();

        return new SuccessDataResult<QuestionAnswerDto>(_mapper.Map<QuestionAnswerDto>(questionAnswer), Messages.AddSuccess);
    }

    public async Task<IDataResult<List<QuestionAnswerDto>>> AddRangeAsync(List<QuestionAnswerCreateDto> questionAnswersCreateDto)
    {
        var questionAnswers = new List<QuestionAnswer>();

        foreach (var questionAnswerCreateDto in questionAnswersCreateDto)
        {
            var questionAnswer = _mapper.Map<QuestionAnswer>(questionAnswerCreateDto);

            await _questionAnswerRepository.AddAsync(questionAnswer);

            questionAnswers.Add(questionAnswer);
        }
        await _questionAnswerRepository.SaveChangesAsync();

        return new SuccessDataResult<List<QuestionAnswerDto>>(_mapper.Map<List<QuestionAnswerDto>>(questionAnswers), Messages.AddSuccess);
    }

    public async Task<IDataResult<List<QuestionAnswerDto>>> UpdateRangeAsync(List<QuestionAnswerCreateDto> questionAnswersUpdateDto)
    {
        if (questionAnswersUpdateDto.Count > 0)
        {
            var CurrentQuestionAnswers = await _questionAnswerRepository.GetAllAsync(x => x.QuestionId == questionAnswersUpdateDto[0].QuestionId);
            await DeleteRangeAsync(CurrentQuestionAnswers.Select(x => x.Id).ToList());
        }

        return await AddRangeAsync(questionAnswersUpdateDto);
    }
    //public async Task<IDataResult<List<QuestionAnswerDto>>> Update(List<QuestionAnswerDto> questionAnswersUpdateDto)
    //{

    //    if (questionAnswersUpdateDto == null || !questionAnswersUpdateDto.Any())
    //        return new ErrorDataResult<List<QuestionAnswerDto>>("No question answers provided to update.");

    //    using (var transaction = await _questionAnswerRepository.BeginTransactionAsync())
    //    {

    //        try
    //        {
    //            foreach (var updatedDto in questionAnswersUpdateDto)
    //            {
    //                var existingEntity = await _questionAnswerRepository.GetAsync(x => x.QuestionId == updatedDto.QuestionId);

    //                if (existingEntity != null)
    //                {
    //                    // Güncelleme işlemi
    //                    existingEntity.Answer = updatedDto.Answer;
    //                    await _questionAnswerRepository.UpdateAsync(existingEntity);
    //                }
    //                else
    //                {
    //                    // Hata durumu
    //                    await transaction.RollbackAsync();
    //                    return new ErrorDataResult<List<QuestionAnswerDto>>($"Question with ID {updatedDto.QuestionId} not found.");
    //                }
    //            }

    //            await _questionAnswerRepository.SaveChangesAsync(); // Transaction içinde işlemleri kaydet
    //            await transaction.CommitAsync(); // Transaction'ı commit et

    //            return new SuccessDataResult<List<QuestionAnswerDto>>("Question answers successfully updated.");
    //        }
    //        catch (Exception ex)
    //        {
    //            await transaction.RollbackAsync(); // Transaction'ı geri al
    //            return new ErrorDataResult<List<QuestionAnswerDto>>($"Error updating question answers: {ex.Message}");
    //        }
    //    }
    //}


    public async Task<IDataResult<List<QuestionAnswerDto>>> Update(List<QuestionAnswerDto> questionAnswersUpdateDto)
    {
        if (questionAnswersUpdateDto.Count > 0)
        {
            foreach (var updatedDto in questionAnswersUpdateDto)
            {
                var existingEntity = await _questionAnswerRepository.GetAsync(x => x.Id == updatedDto.Id);

                if (existingEntity != null)
                {
                    // Güncelleme işlemi
                    var updatedAnswer=_mapper.Map(updatedDto, existingEntity);

                    await _questionAnswerRepository.UpdateAsync(updatedAnswer);
                    
                }
                else
                {
                    return new ErrorDataResult<List<QuestionAnswerDto>>();
                }
            }
            await _questionAnswerRepository.SaveChangesAsync();
        }

        return new SuccessDataResult<List<QuestionAnswerDto>>();

    }


    public async Task<IResult> DeleteAsync(Guid id)
    {
        var questionAnswer = await _questionAnswerRepository.GetByIdAsync(id);

        if (questionAnswer is null)
        {
            return new ErrorDataResult<ClassroomProductDto>(Messages.ClassroomProductNotFound);
        }

        await _questionAnswerRepository.DeleteAsync(questionAnswer);
        await _questionAnswerRepository.SaveChangesAsync();

        return new SuccessResult(Messages.DeleteSuccess);
    }

    public async Task<IResult> DeleteRangeAsync(List<Guid> ids)
    {
        foreach (var id in ids)
        {
            var questionAnswer = await _questionAnswerRepository.GetByIdAsync(id);

            if (questionAnswer is null)
            {
                return new ErrorDataResult<ClassroomProductDto>(Messages.ClassroomProductNotFound);
            }

            await _questionAnswerRepository.DeleteAsync(questionAnswer);
        }

        await _questionAnswerRepository.SaveChangesAsync();

        return new SuccessResult(Messages.DeleteSuccess);
    }

    public Task<IDataResult<List<QuestionAnswerDto>>> GetByQuestionId(Guid Id)
    {
        throw new NotImplementedException();
    }
}
