using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Wms.Application.Authorization;
using Wms.Infrastructure.Identity;

namespace Wms.Infrastructure.Authentication;

public static class DevelopmentAdminSeeder
{
    public static async Task SeedDevelopmentAdminAsync(
        this IServiceProvider services,
        CancellationToken cancellationToken = default)
    {
        await using var scope = services.CreateAsyncScope();
        var environment = scope.ServiceProvider
            .GetRequiredService<IHostEnvironment>();

        if (!environment.IsDevelopment())
        {
            return;
        }

        var options = scope.ServiceProvider
            .GetRequiredService<IOptions<BootstrapAdminOptions>>()
            .Value;

        if (!options.Enabled)
        {
            return;
        }

        var userManager = scope.ServiceProvider
            .GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = scope.ServiceProvider
            .GetRequiredService<RoleManager<ApplicationRole>>();

        var user = await userManager.FindByEmailAsync(options.Email);

        if (user is null)
        {
            user = new ApplicationUser
            {
                UserName = options.Email,
                Email = options.Email,
                EmailConfirmed = true,
                FullName = options.FullName,
                IsActive = true
            };

            var creationResult = await userManager.CreateAsync(
                user,
                options.Password);

            EnsureSucceeded(creationResult, "criar o administrador inicial");
        }

        if (!await roleManager.RoleExistsAsync(SystemRoles.Administrator))
        {
            throw new InvalidOperationException(
                "A role Administrator não existe. Aplique as migrations antes de iniciar a API.");
        }

        if (!await userManager.IsInRoleAsync(
                user,
                SystemRoles.Administrator))
        {
            var roleResult = await userManager.AddToRoleAsync(
                user,
                SystemRoles.Administrator);

            EnsureSucceeded(
                roleResult,
                "atribuir a role Administrator");
        }
    }

    private static void EnsureSucceeded(
        IdentityResult result,
        string operation)
    {
        if (result.Succeeded)
        {
            return;
        }

        var errors = string.Join(
            "; ",
            result.Errors.Select(error => error.Description));

        throw new InvalidOperationException(
            $"Não foi possível {operation}: {errors}");
    }
}
