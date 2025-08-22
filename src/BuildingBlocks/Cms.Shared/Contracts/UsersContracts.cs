namespace Cms.Shared.Contracts;

public record UserDto(Guid Id, string Email, string FullName, DateTime CreatedAtUtc);

public record CreateUserRequest(string Email, string FullName);

public record UpdateUserRequest(string Email, string FullName);