using BAExamApp.Dtos.Products;
using BAExamApp.Dtos.Subjects;

namespace BAExamApp.Business.Services;

public class SubjectService : ISubjectService
{
    private readonly ISubjectRepository _subjectRepository;
    private readonly IProductSubjectRepository _productsSubjectsRepository;
    private readonly IMapper _mapper;
    public SubjectService(ISubjectRepository subjectRepository, IProductSubjectRepository productsSubjectsRepository, IMapper mapper)
    {
        _subjectRepository = subjectRepository;
        _productsSubjectsRepository = productsSubjectsRepository;
        _mapper = mapper;
    }

    public async Task<IDataResult<List<SubjectListDto>>> GetAllAsync()
    {
        var subjects = await _subjectRepository.GetAllAsync(false);
        return new SuccessDataResult<List<SubjectListDto>>(_mapper.Map<List<SubjectListDto>>(subjects), Messages.ListedSuccess);
    }
    
    public async Task<IDataResult<SubjectDto>> AddAsync(SubjectCreateDto subjectCreateDto)
    {
        if (await _subjectRepository.AnyAsync(x => x.Name.ToLower().Equals(subjectCreateDto.Name.Trim().ToLower())))
        {
            return new ErrorDataResult<SubjectDto>(Messages.SubjectAlreadyExist);
        }

        var subject = _mapper.Map<Subject>(subjectCreateDto);
        await _subjectRepository.AddAsync(subject);
        await _subjectRepository.SaveChangesAsync();
        return new SuccessDataResult<SubjectDto>(_mapper.Map<SubjectDto>(subject), Messages.AddSuccess);
    }

    public async Task<IResult> DeleteAsync(Guid id)
    {
        var subject = await _subjectRepository.GetByIdAsync(id);
        if (subject is null)
            return new ErrorResult(Messages.SubjectNotFound);

        await _subjectRepository.DeleteAsync(subject);
        await _subjectRepository.SaveChangesAsync();

        return new SuccessResult(Messages.DeleteSuccess);
    }

    public async Task<IDataResult<SubjectDto>> GetByIdAsync(Guid id)
    {
        var subject = await _subjectRepository.GetByIdAsync(id);
        if (subject is null)
        {
            return new ErrorDataResult<SubjectDto>(Messages.SubjectNotFound);
        }

        return new SuccessDataResult<SubjectDto>(_mapper.Map<SubjectDto>(subject), Messages.FoundSuccess);
    }

    public async Task<IDataResult<List<SubjectListDto>>> GetAllByProductIdAsync(Guid productId)
    {
        var productSubjects = await _productsSubjectsRepository.GetAllAsync(x => x.ProductId == productId);
        var subjects = productSubjects.Select(x => x.Subject).ToList();

        var subjectListDto = _mapper.Map<List<SubjectListDto>>(subjects);
        return new SuccessDataResult<List<SubjectListDto>>(subjectListDto, Messages.ListReceived);
    }

    public async Task<IDataResult<List<SubjectListDto>>> GetAllByListProductIdsAsync(List<Guid> productIds)
    {
        List<ProductSubject> productSubjectList = new List<ProductSubject>();
        foreach (var productId in productIds)
        {
            var productSubjects = await _productsSubjectsRepository.GetAllAsync(x => x.ProductId == productId);

            foreach (var productSubject in productSubjects)
            {
                productSubjectList.Add(productSubject);
            }

        }
            var subjects = productSubjectList.Select(x => x.Subject).ToList();

        var subjectListDto = _mapper.Map<List<SubjectListDto>>(subjects);
        return new SuccessDataResult<List<SubjectListDto>>(subjectListDto, Messages.ListReceived);
    }

    
    public async Task<IDataResult<SubjectDto>> UpdateAsync(SubjectUpdateDto entity)
    {
        if (await _subjectRepository.AnyAsync(x => x.Name.ToLower().Equals(entity.Name.Trim().ToLower())))
        {
            return new ErrorDataResult<SubjectDto>(Messages.SubjectAlreadyExist);
        }

        var subject = await _subjectRepository.GetByIdAsync(entity.Id);
        var updatedSubject = _mapper.Map(entity, subject);
        await _subjectRepository.UpdateAsync(updatedSubject);
        await _subjectRepository.SaveChangesAsync();
        return new SuccessDataResult<SubjectDto>(_mapper.Map<SubjectDto>(updatedSubject), Messages.UpdateSuccess);
    }
   
    public async Task<IDataResult<SubjectDetailDto>> GetDetailsByIdAsync(Guid id)
    {
        var subject = await _subjectRepository.GetByIdAsync(id);
        if (subject is null)
            return new ErrorDataResult<SubjectDetailDto>(Messages.SubjectNotFound);

        var subjectDetailDto = _mapper.Map<SubjectDetailDto>(subject);
        subjectDetailDto.Products = new();
        foreach (var subjectProduct in subject.ProductSubjects)
        {
            subjectDetailDto.Products.Add(_mapper.Map<ProductListDto>(subjectProduct.Product));
        }
        return new SuccessDataResult<SubjectDetailDto>(subjectDetailDto, Messages.FoundSuccess);
    }

}