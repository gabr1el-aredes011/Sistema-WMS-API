namespace Wms.Application.Users;

public interface IUserAdministrationService
{
    Task<PagedResult<UserSummary>> GetUsersAsync(
        string? search,
        bool? isActive,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<UserDetails?> GetUserAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<RoleSummary>> GetRolesAsync(
        CancellationToken cancellationToken = default);

    Task<UserAdministrationResult<UserDetails>> CreateUserAsync(
        CreateUserCommand command,
        CancellationToken cancellationToken = default);

    Task<UserAdministrationResult<UserDetails>> UpdateUserAsync(
        Guid userId,
        UpdateUserCommand command,
        CancellationToken cancellationToken = default);

    Task<UserAdministrationResult<UserDetails>> SetUserStatusAsync(
        Guid actorUserId,
        Guid userId,
        bool isActive,
        CancellationToken cancellationToken = default);

    Task<UserAdministrationResult<UserDetails>> SetUserRolesAsync(
        Guid actorUserId,
        Guid userId,
        IReadOnlyCollection<string> roles,
        CancellationToken cancellationToken = default);
}
