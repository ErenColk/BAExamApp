using BAExamApp.Dtos.Classrooms;
using BAExamApp.Dtos.Products;
using BAExamApp.Dtos.Trainers;
using BAExamApp.Entities.DbSets;
using System.Linq.Expressions;

namespace BAExamApp.Business.Services;

public class ClassroomService : IClassroomService
{
    private readonly IClassroomRepository _classroomRepository;
    private readonly IMapper _mapper;

    public ClassroomService(IClassroomRepository classroomRepository, IMapper mapper)
    {
        _classroomRepository = classroomRepository;
        _mapper = mapper;
    }

    public async Task<IDataResult<ClassroomDto>> GetByIdAsync(Guid id)
    {
        var classroom = await _classroomRepository.GetByIdAsync(id);

        if (classroom != null)
        {
            return new SuccessDataResult<ClassroomDto>(_mapper.Map<ClassroomDto>(classroom), Messages.ClassroomFoundSuccess);
        }

        return new ErrorDataResult<ClassroomDto>(Messages.ClassroomNotFound);
    }

    public async Task<IDataResult<List<ClassroomListDto>>> GetAllAsync()
    {
        var classrooms = await _classroomRepository.GetAllAsync();

        return new SuccessDataResult<List<ClassroomListDto>>(_mapper.Map<List<ClassroomListDto>>(classrooms), Messages.ListedSuccess);
    }

    public async Task<IDataResult<List<ClassroomListDto>>> GetAllByIdentityIdAsync(string id)
    {
        var classrooms = await _classroomRepository.GetAllAsync(x => x.TrainerClassrooms.Select(x => x.Trainer.IdentityId).Any(x => x == id));

        return new SuccessDataResult<List<ClassroomListDto>>(_mapper.Map<List<ClassroomListDto>>(classrooms), Messages.ListedSuccess);
    }

    /// <summary>
    /// Adminin sınıfları; sınıf ismine, şube ismine, eğitim türüne, açılış vve kapanış tarihine göre filtreleme yaparak getirmesini sağlayan metottur.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="branchName"></param>
    /// <param name="groupType"></param>
    /// <param name="openingDate"></param>
    /// <param name="closedDate"></param>
    /// <returns></returns>
    public async Task<IDataResult<List<ClassroomListDto>>> GetFilteredByNameOrBranchNameOrGroupTypeOrOpeningDateOrClosedDateAsync(
      string name, string branchName, string groupType, DateTime openingDate, DateTime closedDate)
    {
        if (string.IsNullOrEmpty(name) &&
            string.IsNullOrEmpty(branchName) &&
            string.IsNullOrEmpty(groupType) &&
            openingDate == default &&
            closedDate == default)
        {
            return new SuccessDataResult<List<ClassroomListDto>>(new List<ClassroomListDto>(), Messages.ListedSuccess);
        }

        var getList = await _classroomRepository.GetAllAsync();
        var filterList = getList.Where(x =>
                                          (string.IsNullOrEmpty(name) || x.Id.ToString() == name) &&
                                          (string.IsNullOrEmpty(branchName) || x.Branch.Id.ToString() == branchName) &&
                                          (string.IsNullOrEmpty(groupType) || x.GroupType.Id.ToString() == groupType) &&
                                          (openingDate == default || x.OpeningDate.Date >= openingDate.Date) &&
                                          (closedDate == default || x.ClosedDate.Date <= closedDate.Date));

        return new SuccessDataResult<List<ClassroomListDto>>(_mapper.Map<List<ClassroomListDto>>(filterList), Messages.ListedSuccess);
    }

    public async Task<IDataResult<List<ClassroomListDto>>> GetActiveAsync()
    {
        List<Classroom> activeClassrooms = new List<Classroom>();

        var classrooms = await _classroomRepository.GetAllAsync();

        foreach (var item in classrooms)
        {
            if (item.IsActive)
            {
                activeClassrooms.Add(item);
            }
        }

        return new SuccessDataResult<List<ClassroomListDto>>(_mapper.Map<List<ClassroomListDto>>(activeClassrooms), Messages.ListedSuccess);
    }

    public async Task<IDataResult<ClassroomDetailsDto>> GetDetailsByIdAsync(Guid id)
    {
        var classroom = await _classroomRepository.GetByIdAsync(id);

        if (classroom is null)
        {
            return new ErrorDataResult<ClassroomDetailsDto>(Messages.ClassroomNotFound);
        }

        return new SuccessDataResult<ClassroomDetailsDto>(_mapper.Map<ClassroomDetailsDto>(classroom), Messages.ClassroomFoundSuccess);
    }

    public async Task<IDataResult<ClassroomDetailsForAdminDto>> GetDetailsByIdForAdminAsync(Guid id)
    {
        var classroom = await _classroomRepository.GetByIdAsync(id);

        if (classroom is null)
        {
            return new ErrorDataResult<ClassroomDetailsForAdminDto>(Messages.ClassroomNotFound);
        }

        return new SuccessDataResult<ClassroomDetailsForAdminDto>(_mapper.Map<ClassroomDetailsForAdminDto>(classroom), Messages.ClassroomFoundSuccess);
    }

    public async Task<IDataResult<ClassroomDto>> AddAsync(ClassroomCreateDto classroomCreateDto)
    {
        var hasClassroom = await _classroomRepository.AnyAsync(classroom => classroom.Name.ToLower() == classroomCreateDto.Name.ToLower());

        if (hasClassroom)
        {
            return new ErrorDataResult<ClassroomDto>(Messages.AddFailAlreadyExists);
        }

        var classroom = _mapper.Map<Classroom>(classroomCreateDto);

        await _classroomRepository.AddAsync(classroom);
        await _classroomRepository.SaveChangesAsync();

        return new SuccessDataResult<ClassroomDto>(_mapper.Map<ClassroomDto>(classroom), Messages.AddSuccess);
    }

    public async Task<IDataResult<ClassroomDto>> UpdateAsync(ClassroomUpdateDto classroomUpdateDto)
    {
        var classroom = await _classroomRepository.GetByIdAsync(classroomUpdateDto.Id);

        if (classroom is null)
        {
            return new ErrorDataResult<ClassroomDto>(Messages.ClassroomNotFound);
        }

        var updatedClassroom = _mapper.Map(classroomUpdateDto, classroom);


        if (classroomUpdateDto.ProductIds != null)
        {
            UpdateClassroomProducts(classroom, classroomUpdateDto);
        }


        await _classroomRepository.UpdateAsync(updatedClassroom);
        await _classroomRepository.SaveChangesAsync();

        return new SuccessDataResult<ClassroomDto>(_mapper.Map<ClassroomDto>(updatedClassroom), Messages.UpdateSuccess);
    }

    public async Task<IResult> DeleteAsync(Guid id)
    {
        var classroom = await _classroomRepository.GetByIdAsync(id);

        if (classroom is null)
        {
            return new ErrorDataResult<ClassroomDto>(Messages.ClassroomNotFound);
        }

        await _classroomRepository.DeleteAsync(classroom);
        await _classroomRepository.SaveChangesAsync();

        return new SuccessResult(Messages.DeleteSuccess);
    }

    private void UpdateClassroomProducts(Classroom classroom, ClassroomUpdateDto classroomUpdateDto)
    {
        foreach (var productId in classroomUpdateDto.ProductIds)
        {
            if (!classroom.ClassroomProducts.Any(x => x.ProductId == productId))
            {
                var classroomProduct = new ClassroomProduct
                {
                    ProductId = productId,
                    ClassroomId = classroom.Id
                };
                classroom.ClassroomProducts.Add(classroomProduct);
            }
        }
        foreach (var product in classroom.ClassroomProducts)
        {
            if (!classroomUpdateDto.ProductIds.Any(x => x.Equals(product.ProductId)))
            {
                classroom.ClassroomProducts.Remove(product);
            }
        }
    }

    public async Task<IDataResult<ClassroomDto>> GetAsync(Expression<Func<Classroom, bool>> expression)
    {
        var classroom = await _classroomRepository.GetAsync(expression);

        if (classroom is null)
        {
            return new ErrorDataResult<ClassroomDto>(Messages.ClassroomNotFound);
        }

        return new SuccessDataResult<ClassroomDto>(_mapper.Map<ClassroomDto>(classroom), Messages.ClassroomFoundSuccess);
    }


    /// <summary>
    /// Filtreyi doldurmak için tüm sınıfları isim ve id'sine göre getirir.
    /// </summary>
    /// <returns>Liste tipinde ClassroomFilterDto döndürür</returns>
    public async Task<IDataResult<List<ClassroomFilterDto>>> GetAllClassroomByFilter()
    {
        var classrooms = await _classroomRepository.GetAllAsync();

        return new SuccessDataResult<List<ClassroomFilterDto>>(_mapper.Map<List<ClassroomFilterDto>>(classrooms), Messages.ListedSuccess);
    }
}
