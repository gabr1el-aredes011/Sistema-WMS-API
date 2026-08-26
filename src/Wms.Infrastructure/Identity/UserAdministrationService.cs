using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Wms.Application.Authorization;
using Wms.Application.Users;
using Wms.Infrastructure.Persistence;

namespace Wms.Infrastructure.Identity;

internal sealed class UserAdministrationService(
    UserManager<ApplicationUser> userManager,
    RoleManager<ApplicationRole> roleManager,
    WmsDbContext dbContext,
    TimeProvider timeProvider) : IUserAdministrationService
{
    public async Task<PagedResult<UserSummary>> GetUsersAsync(
        string? search,
        bool? isActive,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        page = Math.Max(page, 1);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var query = userManager.Users.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var normalizedSearch = search.Trim().ToUpperInvariant();
            query = query.Where(user =>
                user.NormalizedEmail!.Contains(normalizedSearch) ||
                user.FullName.ToUpper().Contains(normalizedSearch));
        }

        if (isActive.HasValue)
        {
            query = query.Where(user => user.IsActive == isActive.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var users = await query
            .OrderBy(user => user.FullName)
            .ThenBy(user => user.Email)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(user => new
            {
                user.Id,
                user.FullName,
                Email = user.Email ?? string.Empty,
                user.IsActive,
                user.CreatedAtUtc,
                user.LastLoginAtUtc
            })
            .ToArrayAsync(cancellationToken);

        var userIds = users.Select(user => user.Id).ToArray();
        var roleRows = await (
            from userRole in dbContext.UserRoles.AsNoTracking()
            join role in dbContext.Roles.AsNoTracking()
                on userRole.RoleId equals role.Id
            where userIds.Contains(userRole.UserId)
            select new { userRole.UserId, RoleName = role.Name! })
            .ToArrayAsync(cancellationToken);

        var rolesByUser = roleRows
            .GroupBy(row => row.UserId)
            .ToDictionary(
                group => group.Key,
                group => (IReadOnlyCollection<string>)group
                    .Select(row => row.RoleName)
                    .Order(StringComparer.Ordinal)
                    .ToArray());

        var items = users.Select(user => new UserSummary(
            user.Id,
            user.FullName,
            user.Email,
            user.IsActive,
            user.CreatedAtUtc,
            user.LastLoginAtUtc,
            rolesByUser.GetValueOrDefault(user.Id) ?? []))
            .ToArray();

        return new PagedResult<UserSummary>(
            items,
            page,
            pageSize,
            totalCount);
    }

    public async Task<UserDetails?> GetUserAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var user = await userManager.Users
            .SingleOrDefaultAsync(item => item.Id == userId, cancellationToken);

        return user is null
            ? null
            : await BuildDetailsAsync(user);
    }

    public async Task<IReadOnlyCollection<RoleSummary>> GetRolesAsync(
        CancellationToken cancellationToken = default)
    {
        return await roleManager.Roles
            .AsNoTracking()
            .OrderBy(role => role.Name)
            .Select(role => new RoleSummary(
                role.Name!,
                role.Description,
                role.IsSystem,
                role.RolePermissions.Count))
            .ToArrayAsync(cancellationToken);
    }

    public async Task<UserAdministrationResult<UserDetails>> CreateUserAsync(
        CreateUserCommand command,
        CancellationToken cancellationToken = default)
    {
        var fullName = command.FullName.Trim();
        var email = command.Email.Trim();
        var resolvedRoles = await ResolveRolesAsync(
            command.Roles,
            cancellationToken);

        if (resolvedRoles is null)
        {
            return UserAdministrationResult<UserDetails>.Fail(
                UserAdministrationFailure.Validation,
                "Um ou mais perfis informados não existem.");
        }

        if (resolvedRoles.Length == 0)
        {
            return UserAdministrationResult<UserDetails>.Fail(
                UserAdministrationFailure.Validation,
                "Selecione ao menos um perfil para o usuário.");
        }

        if (await userManager.FindByEmailAsync(email) is not null)
        {
            return UserAdministrationResult<UserDetails>.Fail(
                UserAdministrationFailure.Conflict,
                "Já existe um usuário cadastrado com este e-mail.");
        }

        await using var transaction = await dbContext.Database
            .BeginTransactionAsync(cancellationToken);

        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            EmailConfirmed = true,
            FullName = fullName,
            IsActive = true,
            CreatedAtUtc = timeProvider.GetUtcNow()
        };

        var creationResult = await userManager.CreateAsync(
            user,
            command.Password);

        if (!creationResult.Succeeded)
        {
            return IdentityFailure(creationResult);
        }

        var rolesResult = await userManager.AddToRolesAsync(
            user,
            resolvedRoles);

        if (!rolesResult.Succeeded)
        {
            await transaction.RollbackAsync(cancellationToken);
            return IdentityFailure(rolesResult);
        }

        await transaction.CommitAsync(cancellationToken);
        return UserAdministrationResult<UserDetails>.Success(
            await BuildDetailsAsync(user));
    }

    public async Task<UserAdministrationResult<UserDetails>> UpdateUserAsync(
        Guid userId,
        UpdateUserCommand command,
        CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());

        if (user is null)
        {
            return NotFound();
        }

        var email = command.Email.Trim();
        var userWithEmail = await userManager.FindByEmailAsync(email);

        if (userWithEmail is not null && userWithEmail.Id != userId)
        {
            return UserAdministrationResult<UserDetails>.Fail(
                UserAdministrationFailure.Conflict,
                "Já existe um usuário cadastrado com este e-mail.");
        }

        user.FullName = command.FullName.Trim();
        user.Email = email;
        user.NormalizedEmail = userManager.NormalizeEmail(email);
        user.UserName = email;
        user.NormalizedUserName = userManager.NormalizeName(email);
        user.UpdatedAtUtc = timeProvider.GetUtcNow();

        var updateResult = await userManager.UpdateAsync(user);

        return updateResult.Succeeded
            ? UserAdministrationResult<UserDetails>.Success(
                await BuildDetailsAsync(user))
            : IdentityFailure(updateResult);
    }

    public async Task<UserAdministrationResult<UserDetails>> SetUserStatusAsync(
        Guid actorUserId,
        Guid userId,
        bool isActive,
        CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());

        if (user is null)
        {
            return NotFound();
        }

        if (!isActive && actorUserId == userId)
        {
            return UserAdministrationResult<UserDetails>.Fail(
                UserAdministrationFailure.Forbidden,
                "Você não pode desativar a própria conta.");
        }

        if (!isActive &&
            await userManager.IsInRoleAsync(user, SystemRoles.Administrator) &&
            await CountActiveAdministratorsAsync(cancellationToken) <= 1)
        {
            return UserAdministrationResult<UserDetails>.Fail(
                UserAdministrationFailure.Forbidden,
                "O último administrador ativo do sistema não pode ser desativado.");
        }

        user.IsActive = isActive;
        user.UpdatedAtUtc = timeProvider.GetUtcNow();

        var updateResult = await userManager.UpdateAsync(user);

        if (!updateResult.Succeeded)
        {
            return IdentityFailure(updateResult);
        }

        if (!isActive)
        {
            await RevokeActiveSessionsAsync(
                userId,
                "UserDisabled",
                cancellationToken);
        }

        return UserAdministrationResult<UserDetails>.Success(
            await BuildDetailsAsync(user));
    }

    public async Task<UserAdministrationResult<UserDetails>> SetUserRolesAsync(
        Guid actorUserId,
        Guid userId,
        IReadOnlyCollection<string> roles,
        CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());

        if (user is null)
        {
            return NotFound();
        }

        var resolvedRoles = await ResolveRolesAsync(roles, cancellationToken);

        if (resolvedRoles is null)
        {
            return UserAdministrationResult<UserDetails>.Fail(
                UserAdministrationFailure.Validation,
                "Um ou mais perfis informados não existem.");
        }

        if (resolvedRoles.Length == 0)
        {
            return UserAdministrationResult<UserDetails>.Fail(
                UserAdministrationFailure.Validation,
                "Selecione ao menos um perfil para o usuário.");
        }

        var currentRoles = (await userManager.GetRolesAsync(user)).ToArray();
        var removesAdministrator = currentRoles.Contains(
                SystemRoles.Administrator,
                StringComparer.OrdinalIgnoreCase) &&
            !resolvedRoles.Contains(
                SystemRoles.Administrator,
                StringComparer.OrdinalIgnoreCase);

        if (removesAdministrator && actorUserId == userId)
        {
            return UserAdministrationResult<UserDetails>.Fail(
                UserAdministrationFailure.Forbidden,
                "Você não pode remover seu próprio perfil de administrador.");
        }

        if (removesAdministrator &&
            user.IsActive &&
            await CountActiveAdministratorsAsync(cancellationToken) <= 1)
        {
            return UserAdministrationResult<UserDetails>.Fail(
                UserAdministrationFailure.Forbidden,
                "O último administrador ativo deve manter esse perfil.");
        }

        var rolesToRemove = currentRoles
            .Except(resolvedRoles, StringComparer.OrdinalIgnoreCase)
            .ToArray();
        var rolesToAdd = resolvedRoles
            .Except(currentRoles, StringComparer.OrdinalIgnoreCase)
            .ToArray();

        await using var transaction = await dbContext.Database
            .BeginTransactionAsync(cancellationToken);

        if (rolesToRemove.Length > 0)
        {
            var removeResult = await userManager.RemoveFromRolesAsync(
                user,
                rolesToRemove);

            if (!removeResult.Succeeded)
            {
                await transaction.RollbackAsync(cancellationToken);
                return IdentityFailure(removeResult);
            }
        }

        if (rolesToAdd.Length > 0)
        {
            var addResult = await userManager.AddToRolesAsync(user, rolesToAdd);

            if (!addResult.Succeeded)
            {
                await transaction.RollbackAsync(cancellationToken);
                return IdentityFailure(addResult);
            }
        }

        user.UpdatedAtUtc = timeProvider.GetUtcNow();
        var updateResult = await userManager.UpdateAsync(user);

        if (!updateResult.Succeeded)
        {
            await transaction.RollbackAsync(cancellationToken);
            return IdentityFailure(updateResult);
        }

        await transaction.CommitAsync(cancellationToken);
        await RevokeActiveSessionsAsync(
            userId,
            "RolesChanged",
            cancellationToken);

        return UserAdministrationResult<UserDetails>.Success(
            await BuildDetailsAsync(user));
    }

    private async Task<UserDetails> BuildDetailsAsync(ApplicationUser user)
    {
        var roles = (await userManager.GetRolesAsync(user))
            .Order(StringComparer.Ordinal)
            .ToArray();

        return new UserDetails(
            user.Id,
            user.FullName,
            user.Email ?? string.Empty,
            user.IsActive,
            user.CreatedAtUtc,
            user.UpdatedAtUtc,
            user.LastLoginAtUtc,
            roles);
    }

    private async Task<string[]?> ResolveRolesAsync(
        IReadOnlyCollection<string> roles,
        CancellationToken cancellationToken)
    {
        var requestedNames = roles
            .Where(role => !string.IsNullOrWhiteSpace(role))
            .Select(role => role.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();
        var normalizedNames = requestedNames
            .Select(roleManager.NormalizeKey)
            .ToArray();
        var existingNames = await roleManager.Roles
            .Where(role => normalizedNames.Contains(role.NormalizedName!))
            .Select(role => role.Name!)
            .ToArrayAsync(cancellationToken);

        return existingNames.Length == requestedNames.Length
            ? existingNames.Order(StringComparer.Ordinal).ToArray()
            : null;
    }

    private async Task<int> CountActiveAdministratorsAsync(
        CancellationToken cancellationToken)
    {
        var administratorRole = await roleManager.FindByNameAsync(
            SystemRoles.Administrator);

        if (administratorRole is null)
        {
            return 0;
        }

        return await (
            from user in dbContext.Users
            join userRole in dbContext.UserRoles
                on user.Id equals userRole.UserId
            where user.IsActive && userRole.RoleId == administratorRole.Id
            select user.Id)
            .Distinct()
            .CountAsync(cancellationToken);
    }

    private async Task RevokeActiveSessionsAsync(
        Guid userId,
        string reason,
        CancellationToken cancellationToken)
    {
        var now = timeProvider.GetUtcNow();
        var sessions = await dbContext.RefreshSessions
            .Where(session =>
                session.UserId == userId &&
                session.RevokedAtUtc == null &&
                session.ExpiresAtUtc > now)
            .ToArrayAsync(cancellationToken);

        foreach (var session in sessions)
        {
            session.RevokedAtUtc = now;
            session.RevocationReason = reason;
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private static UserAdministrationResult<UserDetails> IdentityFailure(
        IdentityResult identityResult)
    {
        return UserAdministrationResult<UserDetails>.Fail(
            UserAdministrationFailure.Validation,
            identityResult.Errors
                .Select(error => error.Description)
                .ToArray());
    }

    private static UserAdministrationResult<UserDetails> NotFound()
    {
        return UserAdministrationResult<UserDetails>.Fail(
            UserAdministrationFailure.NotFound,
            "Usuário não encontrado.");
    }
}
