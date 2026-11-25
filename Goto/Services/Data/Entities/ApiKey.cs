namespace Goto.Services.Data.Entities;

public sealed class ApiKey
{
    public int Id { get; init; }
    public required string Secret { get; init; }
    public required string Name { get; init; }
    public required string CompanyPrefix { get; init; }
    public required DateTimeOffset BeginValidityDate { get; init; }
    public DateTimeOffset EndValidityDate { get; set; } = DateTime.MaxValue;

    public void Disable(DateTimeOffset endValidityDate)
    {
        EndValidityDate = endValidityDate;
    }
}