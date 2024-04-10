using BAExamApp.Dtos.ProductSubjects;

namespace BAExamApp.Business.Services;
public class ProductSubjectService : IProductSubjectService
{
    public async Task<IDataResult<ProductSubjectDto>> AddAsync(ProductSubjectCreateDto productSubjectCreateDto)
    {
        throw new NotImplementedException();
    }

    public async Task<IDataResult<List<ProductSubjectDto>>> AddRangeAsync(List<ProductSubjectCreateDto> productSubjectsCreateDto)
    {
        throw new NotImplementedException();
    }

    public async Task<IResult> DeleteAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public async Task<IResult> DeleteRangeAsync(List<Guid> ids)
    {
        throw new NotImplementedException();
    }
}
