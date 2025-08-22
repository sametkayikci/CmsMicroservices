namespace Cms.Shared.Abstractions;

public interface IDateTime
{
    DateTime UtcNow { get; }
}

public sealed class SystemDateTime : IDateTime
{
    public DateTime UtcNow => DateTime.UtcNow;
}