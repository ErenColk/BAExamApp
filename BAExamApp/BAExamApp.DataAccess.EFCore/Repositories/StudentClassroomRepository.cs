namespace BAExamApp.DataAccess.EFCore.Repositories;

public class StudentClassroomRepository : EFBaseRepository<StudentClassroom>, IStudentClassroomRepository
{
    public StudentClassroomRepository(BAExamAppDbContext context) : base(context)
    {

    }
    
}
