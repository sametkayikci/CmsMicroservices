namespace Cms.Shared.Contracts;

public record ContentDto(Guid Id, Guid UserId, string Title, string Body, DateTime CreatedAtUtc);

public record CreateContentRequest(Guid UserId, string Title, string Body);

public record UpdateContentRequest(string Title, string Body);