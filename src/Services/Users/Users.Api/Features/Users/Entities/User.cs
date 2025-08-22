namespace Users.Api.Features.Users.Entities;

public class User
{
    public Guid Id { get; set; }
    public string Email { get; set; } = null!;
    public string FullName { get; set; } = null!;
    public DateTime CreatedAtUtc { get; set; }
}