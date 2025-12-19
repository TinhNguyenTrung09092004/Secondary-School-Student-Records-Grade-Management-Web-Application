using Microsoft.AspNetCore.Identity;
using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public static class SeedData
{
    public static async Task Initialize(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var context = serviceProvider.GetRequiredService<ApplicationDbContext>();

        await SeedRoles(roleManager);
        await SeedUsers(userManager);
    }

    private static async Task SeedRoles(RoleManager<IdentityRole> roleManager)
    {
        string[] roles = {
            "Admin", "Principal", "HomeRoomTeacher",
            "AcademicAffairs", "DepartmentHead", "SubjectTeacher", "Student"
        };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }
    }

    private static async Task SeedUsers(UserManager<ApplicationUser> userManager)
    {
        await CreateUser(userManager, "4801104136@student.hcmue.edu.vn", "Admin@123", "Admin");

        var users = new (string Email, string Role)[]
        {
            ("academicAffairs@gmail.com", "AcademicAffairs"),
            ("principal@gmail.com", "Principal"),
            ("departmentHead@gmail.com", "DepartmentHead"),
            ("student@gmail.com", "Student"),
            ("subjectTeacher@gmail.com", "SubjectTeacher")
        };

        foreach (var u in users)
            await CreateUser(userManager, u.Email, "Admin@123", u.Role);
    }

    private static async Task CreateUser(UserManager<ApplicationUser> userManager,
                                        string email, string password, string role)
    {
        if (await userManager.FindByEmailAsync(email) != null) return;

        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            EmailConfirmed = true,
            IsAccountSetupComplete = true,
            IsDeleted = false
        };

        if ((await userManager.CreateAsync(user, password)).Succeeded)
            await userManager.AddToRoleAsync(user, role);
    }
}