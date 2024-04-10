using BAExamApp.Dtos.Branches;
using BAExamApp.Dtos.Cities;

namespace BAExamApp.Business.Services;

public class CityService : ICityService
{
    private readonly ICityRepository _cityRepository;
    private readonly IMapper _mapper;
    
    public CityService(ICityRepository cityRepository, IMapper mapper)
    {
        _cityRepository = cityRepository;
        _mapper = mapper;
    }

    public async Task<IDataResult<List<CityListDto>>> GetAllAsync()
    {
        var cities = await _cityRepository.GetAllAsync(false);

        return new SuccessDataResult<List<CityListDto>>(_mapper.Map<List<CityListDto>>(cities), Messages.ListedSuccess);
    }

    public async Task<IDataResult<CityDto>> AddAsync(CityCreateDto cityCreateDto)
    {
        var hasCity = await _cityRepository.AnyAsync(x => x.Name.ToLower().Equals(cityCreateDto.Name.ToLower()));

        if (hasCity)
        {
            return new ErrorDataResult<CityDto>(Messages.AddFailAlreadyExists);
        }

        var city = _mapper.Map<City>(cityCreateDto);

        await _cityRepository.AddAsync(city);
        await _cityRepository.SaveChangesAsync();

        return new SuccessDataResult<CityDto>(_mapper.Map<CityDto>(city), Messages.AddSuccess);
    }

    public async Task<IResult> DeleteAsync(Guid id)
    {
        var city = await _cityRepository.GetByIdAsync(id);

        if (city is null)
        {
            return new ErrorDataResult<CityDto>(Messages.CityNotFound);
        }

        await _cityRepository.DeleteAsync(city);
        await _cityRepository.SaveChangesAsync();

        return new SuccessResult(Messages.DeleteSuccess);
    }


    public async Task<IDataResult<CityDto>> UpdateAsync(CityUpdateDto cityUpdateDto)
    {
        var hasCity = await _cityRepository.AnyAsync(city => city.Id != cityUpdateDto.Id && city.Name.ToLower() == cityUpdateDto.Name.ToLower());

        if (hasCity)
        {
            return new ErrorDataResult<CityDto>(Messages.AddFailAlreadyExists);
        }

        var city = await _cityRepository.GetByIdAsync(cityUpdateDto.Id);

        if (city is null)
        {
            return new ErrorDataResult<CityDto>(Messages.CityNotFound);
        }

        var updatedCity = _mapper.Map(cityUpdateDto, city);

        await _cityRepository.UpdateAsync(updatedCity);
        await _cityRepository.SaveChangesAsync();

        return new SuccessDataResult<CityDto>(_mapper.Map<CityDto>(updatedCity), Messages.UpdateSuccess);
    }

    public async Task<IDataResult<CityDto>> GetByIdAsync(Guid id)
    {
        var city = await _cityRepository.GetByIdAsync(id);

        if (city is null)
        {
            return new ErrorDataResult<CityDto>(Messages.CityNotFound);
        }

        return new SuccessDataResult<CityDto>(_mapper.Map<CityDto>(city), Messages.FoundSuccess);
    }
}
