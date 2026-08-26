namespace Wms.Application.Users;

public sealed record UserSummary(
    Guid Id,
    string FullName,
    string Email,
    bool IsActive,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset? LastLoginAtUtc,
    IReadOnlyCollection<string> Roles);

public sealed record UserDetails(
    Guid Id,
    string FullName,
    string Email,
    bool IsActive,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset? UpdatedAtUtc,
    DateTimeOffset? LastLoginAtUtc,
    IReadOnlyCollection<string> Roles);

public sealed record RoleSummary(
    string Name,
    string? Description,
    bool IsSystem,
    int PermissionCount);

public sealed record PagedResult<T>(
    IReadOnlyCollection<T> Items,
    int Page,
    int PageSize,
    int TotalCount)
{
    public int TotalPages => TotalCount == 0
        ? 0
        : (int)Math.Ceiling(TotalCount / (double)PageSize);
}

public sealed record CreateUserCommand(
    string FullName,
    string Email,
    string Password,
    IReadOnlyCollection<string> Roles);

public sealed record UpdateUserCommand(
    string FullName,
    string Email);

public enum UserAdministrationFailure
{
    None,
    NotFound,
    Validation,
    Conflict,
    Forbidden
}

public sealed record UserAdministrationResult<T>(
    UserAdministrationFailure Failure,
    T? Value,
    IReadOnlyCollection<string> Errors)
{
    public bool Succeeded => Failure == UserAdministrationFailure.None;

    public static UserAdministrationResult<T> Success(T value) =>
        new(UserAdministrationFailure.None, value, []);

    public static UserAdministrationResult<T> Fail(
        UserAdministrationFailure failure,
        params string[] errors) => new(failure, default, errors);
}
