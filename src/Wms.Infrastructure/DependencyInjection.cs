using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Wms.Application.Authentication;
using Wms.Application.Authorization;
using Wms.Application.Users;
using Wms.Infrastructure.Authentication;
using Wms.Infrastructure.Identity;
using Wms.Infrastructure.Persistence;

namespace Wms.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        string connectionString,
        IConfiguration configuration)
    {
        services.AddDbContext<WmsDbContext>(options =>
        {
            options.UseNpgsql(connectionString);
        });

        services.AddDataProtection();

        services
            .AddIdentityCore<ApplicationUser>(options =>
            {
                options.User.RequireUniqueEmail = true;

                options.Password.RequiredLength = 12;
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;

                options.Lockout.AllowedForNewUsers = true;
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.DefaultLockoutTimeSpan =
                    TimeSpan.FromMinutes(15);
            })
            .AddRoles<ApplicationRole>()
            .AddEntityFrameworkStores<WmsDbContext>()
            .AddSignInManager()
            .AddDefaultTokenProviders();

        var jwtSection = configuration.GetSection(JwtOptions.SectionName);
        var jwtOptions = jwtSection.Get<JwtOptions>()
            ?? throw new InvalidOperationException(
                "A configuração JWT não foi encontrada.");

        services
            .AddOptions<JwtOptions>()
            .Bind(jwtSection)
            .ValidateDataAnnotations()
            .Validate(
                options => Encoding.UTF8.GetByteCount(options.SigningKey) >= 32,
                "A chave JWT deve possuir pelo menos 32 bytes.")
            .ValidateOnStart();

        services
            .AddOptions<BootstrapAdminOptions>()
            .Bind(configuration.GetSection(
                BootstrapAdminOptions.SectionName))
            .ValidateDataAnnotations()
            .Validate(
                options => !options.Enabled ||
                    (!string.IsNullOrWhiteSpace(options.Email) &&
                     !string.IsNullOrWhiteSpace(options.FullName) &&
                     !string.IsNullOrWhiteSpace(options.Password)),
                "E-mail, nome completo e senha são obrigatórios quando o bootstrap administrativo está habilitado.")
            .ValidateOnStart();

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.MapInboundClaims = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwtOptions.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwtOptions.Audience,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtOptions.SigningKey)),
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromSeconds(30),
                    NameClaimType = "name",
                    RoleClaimType = ClaimTypes.Role
                };
            });

        services.AddAuthorization(options =>
        {
            foreach (var permission in SystemPermissions.All)
            {
                options.AddPolicy(
                    permission.Code,
                    policy => policy.RequireClaim(
                        "permission",
                        permission.Code));
            }
        });
        services.AddSingleton(TimeProvider.System);
        services.AddScoped<ITokenGenerator, TokenGenerator>();
        services.AddScoped<IAuthenticationService, AuthenticationService>();
        services.AddScoped<IUserAdministrationService, UserAdministrationService>();

        services
            .AddHealthChecks()
            .AddDbContextCheck<WmsDbContext>(
                name: "postgresql",
                tags: new[] { "ready" });

        return services;
    }
}
