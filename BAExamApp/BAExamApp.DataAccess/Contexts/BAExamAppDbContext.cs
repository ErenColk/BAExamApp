using BAExamApp.Core.Entities.Base;
using BAExamApp.Core.Enums;
using BAExamApp.Entities.Configurations;
using BAExamApp.Entities.DbSets;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Security.Claims;

namespace BAExamApp.DataAccess.Contexts;

public class BAExamAppDbContext : IdentityDbContext
{
    public const string ConnectionName = "BAExamApp";

    private readonly IHttpContextAccessor? _contextAccessor;
    public BAExamAppDbContext(DbContextOptions<BAExamAppDbContext> options, IHttpContextAccessor contextAccessor) : base(options)
    {
        _contextAccessor = contextAccessor;
    }
    public BAExamAppDbContext(DbContextOptions<BAExamAppDbContext> options) : base(options) { }
    public virtual DbSet<Admin> Admins { get; set; } = null!;
    public virtual DbSet<Branch> Branches { get; set; } = null!;
    public virtual DbSet<City> Cities { get; set; } = null!;
    public virtual DbSet<Classroom> Classrooms { get; set; } = null!;
    public virtual DbSet<ClassroomProduct> Classrooms_Products { get; set; } = null!;
    public virtual DbSet<Email> Emails { get; set; } = null!;
    public virtual DbSet<Exam> Exams { get; set; } = null!;
    public virtual DbSet<ExamClassrooms> ExamClassrooms { get; set; } = null!;
    public virtual DbSet<ExamEvaluator> Exams_Evaluators { get; set; } = null!;
    public virtual DbSet<ExamRule> ExamRules { get; set; } = null!;
    public virtual DbSet<ExamRuleSubtopic> ExamRules_Subtopics { get; set; } = null!;
    public virtual DbSet<GroupType> GroupTypes { get; set; } = null!;
    public virtual DbSet<Product> Products { get; set; } = null!;
    public virtual DbSet<ProductSubject> Products_Subjects { get; set; } = null!;
    public virtual DbSet<Question> Questions { get; set; } = null!;
    public virtual DbSet<QuestionSubtopics> QuestionSubtopics { get; set; } = null!;
    public virtual DbSet<QuestionAnswer> QuestionAnswers { get; set; } = null!;
    public virtual DbSet<QuestionDifficulty> QuestionDifficulties { get; set; } = null!;
    public virtual DbSet<QuestionFeedback> QuestionFeedbacks { get; set; } = null!;
    public virtual DbSet<QuestionRevision> QuestionRevisions { get; set; } = null!;
    public virtual DbSet<QuestionArrange> QuestionArranges { get; set; } = null!;
    public virtual DbSet<Student> Students { get; set; } = null!;
    public virtual DbSet<StudentAnswer> StudentAnswers { get; set; } = null!;
    public virtual DbSet<StudentClassroom> Students_Classrooms { get; set; } = null!;
    public virtual DbSet<StudentExam> Students_Exams { get; set; } = null!;
    public virtual DbSet<StudentQuestion> Students_Questions { get; set; } = null!;
    public virtual DbSet<Subject> Subjects { get; set; } = null!;
    public virtual DbSet<Subtopic> Subtopics { get; set; } = null!;
    public virtual DbSet<Talent> Talents { get; set; } = null!;
    public virtual DbSet<TechnicalUnit> TechnicalUnits { get; set; } = null!;
    public virtual DbSet<TestExam> TestExams { get; set; } = null!;
    public virtual DbSet<TestExamQuestion> TestExams_Questions { get; set; } = null!;
    public virtual DbSet<TestExamTester> TestExams_Testers { get; set; } = null!;
    public virtual DbSet<Trainer> Trainers { get; set; } = null!;
    public virtual DbSet<TrainerClassroom> Trainers_Classrooms { get; set; } = null!;
    public virtual DbSet<TrainerProduct> Trainers_Products { get; set; } = null!;
    public virtual DbSet<TrainerTalent> Trainers_Talents { get; set; } = null!;
    public virtual DbSet<SentMail> SentMails { get; set; } = null!;
    public virtual DbSet<CandidateQuestion> CandidateQuestions { get; set; } = null!;
    public virtual DbSet<CandidateQuestionAnswer> CandidateQuestionAnswers { get; set; } = null!;
    public virtual DbSet<CandidateAnswer> CandidateStudentAnswers { get; set; } = null!;
    public virtual DbSet<CandidateCandidateQuestion> Candidate_Students_Questions { get; set; } = null!;
    public virtual DbSet<CandidateAdmin> CandidateAdmins { get; set; } = null!;
    public virtual DbSet<CandidateBranch> CandidateBranches { get; set; } = null!;
    public virtual DbSet<Candidate> Candidates { get; set; } = null!;
    public virtual DbSet<CandidateGroup> CandidateGroups { get; set; } = null!;
    public virtual DbSet<CandidateExam> CandidateExams { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(typeof(IEntityConfiguration).Assembly);
        builder.Entity<QuestionAnswer>().ToTable(tb => tb.HasTrigger("CalculateStudentQuestionScoresAndExamScore"));
        builder.Entity<StudentQuestion>().ToTable(tb => tb.HasTrigger("ScoringOfTheExam"));
        builder.Entity<Exam>().ToTable(tb => tb.HasTrigger("SoftDeleteExam"));
        base.OnModelCreating(builder);
    }

    public override int SaveChanges()
    {
        SetBaseProperties();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        SetBaseProperties();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void SetBaseProperties()
    {
        var entries = ChangeTracker.Entries<BaseEntity>();

        var userId = _contextAccessor?.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "NotFound-User";
        foreach (var entry in entries)
        {
            SetIfAdded(entry, userId);
            SetIfModified(entry, userId);
            SetIfDeleted(entry, userId);
        }
    }

    private void SetIfAdded(EntityEntry<BaseEntity> entityEntry, string userId)
    {
        if (entityEntry.State != EntityState.Added)
        {
            return;
        }

        entityEntry.Entity.Status = Status.Active;
        entityEntry.Entity.CreatedDate = DateTime.Now;
        entityEntry.Entity.CreatedBy = userId;
    }

    private void SetIfModified(EntityEntry<BaseEntity> entityEntry, string userId)
    {
        //if (entityEntry.State == EntityState.Modified)
        //{
        //    entityEntry.Entity.Status = Status.Modified;
        //}
        entityEntry.Entity.ModifiedDate = DateTime.Now;
        entityEntry.Entity.ModifiedBy = userId;
    }

    private void SetIfDeleted(EntityEntry<BaseEntity> entityEntry, string userId)
    {
        if (entityEntry.State is not EntityState.Deleted)
        {
            return;
        }

        if (entityEntry.Entity is not AuditableEntity entity)
        {
            return;
        }

        entityEntry.State = EntityState.Modified;

        entity.Status = Status.Deleted;
        entity.DeletedBy = userId;
        entity.DeletedDate = DateTime.Now;
    }
}