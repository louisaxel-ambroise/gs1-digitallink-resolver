using System.Text.Json.Serialization;

namespace DigitalLinkToolkit.Conversion.DTOs;

public record OptimizationCode
{
    public required string Code { get; init; }
    public required string[] SequenceAIs { get; init; }
    public required string Meaning { get; init; }
    public required string Usage { get; init; }

    public int Priority => SequenceAIs.Length;

    public bool IsFulfilledBy(IEnumerable<string> identifierCodes) => SequenceAIs.All(identifierCodes.Contains);

    public static readonly OptimizationCode Default = new()
    {
        Code = string.Empty,
        SequenceAIs = [],
        Meaning = string.Empty,
        Usage = string.Empty
    };
}

public record AIIdentifier
{
    public static readonly AIIdentifier None = new();

    public string Code { get; init; } = string.Empty;
    public string ShortCode { get; init; } = string.Empty;
    public AIType Type { get; init; }
    public IReadOnlyList<AIComponent> Components { get; init; } = [];
    public string Pattern { get; set; } = string.Empty;
}

public record AIComponent
{
    public required Charset Type { get; init; }
    public required int Length { get; init; }
    public ComponentFlag Flags { get; set; }
    public int Gcp { get; init; }

    internal string GetValue(string remaining)
    {
        return Flags.HasFlag(ComponentFlag.FixedLength) ? remaining[..Length] : remaining;
    }
}

[Flags]
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ComponentFlag
{
    [JsonStringEnumMemberName("F")]
    FixedLength = 1,
    [JsonStringEnumMemberName("C")]
    CheckDigit = 2,
    [JsonStringEnumMemberName("G")]
    GCP = 4
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum Charset
{
    Unknown,
    [JsonStringEnumMemberName("N")]
    Numeric,
    [JsonStringEnumMemberName("X")]
    Alpha
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum AIType
{
    Unknown,
    [JsonStringEnumMemberName("P")]
    PrimaryKey,
    [JsonStringEnumMemberName("Q")]
    Qualifier,
    [JsonStringEnumMemberName("D")]
    DataAttribute,
}
