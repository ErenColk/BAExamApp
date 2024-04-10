using BAExamApp.Core.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace BAExamApp.DataAccess.EFCore.Seeds;

internal static class AdminSeed
{
    private const string AdminEmail = "admin@baexamapp.com";
    private const string AdminPassword = "newPassword+0";
    private const string DefaultCity = "İstanbul";

    private const string CandidateAdminEmail = "candidateAdmin@baexamapp.com";
    private const string CandidateAdminPassword = "newPassword+0";
    private const string BranchName = "İstanbul";
    public static async Task SeedAsync(IConfiguration configuration)
    {
        var dbContextBuilder = new DbContextOptionsBuilder<BAExamAppDbContext>();

        dbContextBuilder.UseSqlServer(configuration.GetConnectionString(BAExamAppDbContext.ConnectionName));

        using BAExamAppDbContext context = new(dbContextBuilder.Options);
        if (!context.Roles.Any())
        {
            await AddRoles(context);
        }

        if (!context.Users.Any(user => user.Email == AdminEmail))
        {
            await AddAdmin(context);
        }

        if(!context.Users.Any(user => user.Email == CandidateAdminEmail))
        {
            await AddCandidateAdmin(context);
        }

        await Task.CompletedTask;
    }

    private static async Task AddAdmin(BAExamAppDbContext context)
    {
        IdentityUser user = new()
        {
            UserName = AdminEmail,
            NormalizedUserName = AdminEmail.ToUpper(),
            Email = AdminEmail,
            NormalizedEmail = AdminEmail.ToUpper(),
            EmailConfirmed = true
        };
        user.PasswordHash = new PasswordHasher<IdentityUser>().HashPassword(user, AdminPassword);
        await context.Users.AddAsync(user);

        var adminRoleId = context.Roles.FirstOrDefault(role => role.Name == Roles.Admin.ToString())!.Id;

        await context.UserRoles.AddAsync(new IdentityUserRole<string> { UserId = user.Id, RoleId = adminRoleId });

        City city = new()
        {
            CreatedBy = "Super-Admin",
            CreatedDate = DateTime.Now,
            ModifiedBy = "Super-Admin",
            ModifiedDate = DateTime.Now,
            Name = DefaultCity,
            Status = Status.Added
        };
        await context.Cities.AddAsync(city);

        context.Admins.Add(new Admin
        {
            Status = Status.Added,
            CreatedBy = "Super-Admin",
            CreatedDate = DateTime.Now,
            ModifiedBy = "Super-Admin",
            ModifiedDate = DateTime.Now,
            FirstName = "Admin",
            LastName = "Admin",
            Email = AdminEmail,
            Gender = true,
            DateOfBirth = new DateTime(2000, 1, 1),
            IdentityId = user.Id,
            CityId = city.Id
        });

        await context.SaveChangesAsync();
    }

    private static async Task AddCandidateAdmin(BAExamAppDbContext context)
    {
        IdentityUser user = new()
        {
            UserName = CandidateAdminEmail,
            NormalizedUserName = CandidateAdminEmail.ToUpper(),
            Email = CandidateAdminEmail,
            NormalizedEmail = CandidateAdminEmail.ToUpper(),
            EmailConfirmed = true
        };
        user.PasswordHash = new PasswordHasher<IdentityUser>().HashPassword(user, CandidateAdminPassword);
        await context.Users.AddAsync(user);

        var candidateAdminRoleId = context.Roles.FirstOrDefault(role => role.Name == Roles.CandidateAdmin.ToString())!.Id;

        await context.UserRoles.AddAsync(new IdentityUserRole<string> { UserId = user.Id, RoleId = candidateAdminRoleId });

        

        context.CandidateAdmins.Add(new CandidateAdmin
        {
            Status = Status.Added,
            CreatedBy = "Candidate-Admin",
            CreatedDate = DateTime.Now,
            ModifiedBy = "Candidate-Admin",
            ModifiedDate = DateTime.Now,
            FirstName = "CandidateAdmin",
            LastName = "CandidateAdmin",
            Email = AdminEmail,
            Gender = true,
            DateOfBirth = new DateTime(2000, 1, 1),
            IdentityId = user.Id,
        });

        await context.SaveChangesAsync();
    }

    private static async Task AddRoles(BAExamAppDbContext context)
    {
        string[] roles = Enum.GetNames(typeof(Roles));
        for (int i = 0; i < roles.Length; i++)
        {
            if (await context.Roles.AnyAsync(role => role.Name == roles[i]))
            {
                continue;
            }

            await context.Roles.AddAsync(new IdentityRole(roles[i]));
        }
    }
}

