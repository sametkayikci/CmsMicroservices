namespace Contents.Api.Features.Contents.Entities;


public class Content
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Title { get; set; } = null!;
    public string Body { get; set; } = null!;
    public DateTime CreatedAtUtc { get; set; }
}