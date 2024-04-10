namespace BAExamApp.DataAccess.EFCore.Repositories;

public class CityRepository : EFBaseRepository<City>, ICityRepository
{
    public CityRepository(BAExamAppDbContext context) : base(context)
    {

    }
}
