namespace Goto.Infrastructure.Authentication;

public record ApiKeyDefinition
{
    public required int? Seed { get; set; }
    public required Key[] Keys { get; set; }
}

public record Key
{
    public required string Name { get; set; }
    public required string CompanyPrefix { get; set; }
}