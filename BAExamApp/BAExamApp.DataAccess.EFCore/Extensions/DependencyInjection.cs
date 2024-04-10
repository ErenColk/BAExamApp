using BAExamApp.DataAccess.EFCore.Repositories;
using BAExamApp.DataAccess.EFCore.Seeds;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BAExamApp.DataAccess.EFCore.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddEFCoreServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IAdminRepository, AdminRepository>();
        services.AddScoped<IBranchRepository, BranchRepository>();
        services.AddScoped<ICityRepository, CityRepository>();
        services.AddScoped<IClassroomRepository, ClassroomRepository>();
        services.AddScoped<IClassroomProductRepository, ClassroomProductRepository>();
        services.AddScoped<IEmailRepository, EmailRepository>();
        services.AddScoped<IExamRepository, ExamRepository>();
        services.AddScoped<IExamEvaluatorRepository, ExamEvaluatorRepository>();
        services.AddScoped<IExamRuleRepository, ExamRuleRepository>();
        services.AddScoped<IExamRuleSubtopicRepository, ExamRuleSubtopicRepository>();
        services.AddScoped<IExamClassroomsRepository, ExamClassroomsRepository>();
        services.AddScoped<IGroupTypeRepository, GroupTypeRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IProductSubjectRepository, ProductSubjectRepository>();
        services.AddScoped<IQuestionSubtopicsRepository, QuestionSubtopicsRepository>();
        services.AddScoped<IQuestionRepository, QuestionRepository>();
        services.AddScoped<IQuestionAnswerRepository, QuestionAnswerRepository>();
        services.AddScoped<IQuestionArrangeRepository, QuestionArrangeRepository>();
        services.AddScoped<IQuestionDifficultyRepository, QuestionDifficultyRepository>();
        services.AddScoped<IQuestionFeedbackRepository, QuestionFeedbackRepository>();
        services.AddScoped<IQuestionRevisionRepository, QuestionRevisionRepository>();
        services.AddScoped<IStudentRepository, StudentRepository>();
        services.AddScoped<IStudentAnswerRepository, StudentAnswerRepository>();
        services.AddScoped<IStudentClassroomRepository, StudentClassroomRepository>();
        services.AddScoped<IStudentExamRepository, StudentExamRepository>();
        services.AddScoped<IStudentQuestionRepository, StudentQuestionRepository>();
        services.AddScoped<ISubjectRepository, SubjectRepository>();
        services.AddScoped<ISubtopicRepository, SubtopicRepository>();
        services.AddScoped<ITalentRepository, TalentRepository>();
        services.AddScoped<ITechnicalUnitRepository, TechnicalUnitRepository>();
        services.AddScoped<ITestExamRepository, TestExamRepository>();
        services.AddScoped<ITestExamQuestionRepository, TestExamQuestionRepository>();
        services.AddScoped<ITestExamTesterRepository, TestExamTesterTrainer>();
        services.AddScoped<ITrainerRepository, TrainerRepository>();
        services.AddScoped<ITrainerClassroomRepository, TrainerClassroomRepository>();
        services.AddScoped<ITrainerProductRepository, TrainerProductRepository>();
        services.AddScoped<ITrainerTalentRepository, TrainerTalentRepository>();
        services.AddScoped<ISentMailRepository, SentMailRepository>();
        services.AddScoped<ICandidateCandidateQuestionRepository, CandidateCandidateQuestionRepository>();
        services.AddScoped<ICandidateAnswerRepository, CandidateAnswerRepository>();
        services.AddScoped<ICandidateQuestionRepository, CandidateQuestionRepository>();
        services.AddScoped<ICandidateQuestionAnswerRepository, CandidateQuestionAnswerRepository>();
        services.AddScoped<ICandidateAdminRepository, CandidateAdminRepository>();
        services.AddScoped<ICandidateRepository, CandidateRepository>();
        services.AddScoped<ICandidateBranchRepository, CandidateBranchRepository>();
        services.AddScoped<ICandidateGroupRepository, CandidateGroupRepository>();

        AdminSeed.SeedAsync(configuration).GetAwaiter().GetResult();

        return services;
    }
}