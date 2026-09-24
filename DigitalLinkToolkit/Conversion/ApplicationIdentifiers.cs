using DigitalLinkToolkit.Conversion.DTOs;
using System.Text.Json;

namespace DigitalLinkToolkit.Conversion;

public record ApplicationIdentifiers
{
    public static ApplicationIdentifiers Shared { get; private set; } = new ();

    public IReadOnlyList<AIIdentifier> Identifiers { get; init; } = [];
    public IReadOnlyList<OptimizationCode> OptimizationCodes { get; init; } = [];

    public bool TryGetIdentifier(string key, out AIIdentifier ai)
    {
        ai = Identifiers.SingleOrDefault(x => x.Code == key || x.ShortCode == key, AIIdentifier.None);

        return ai != AIIdentifier.None;
    }

    public static void Initialize(string fileName, JsonSerializerOptions? options)
    {
        using var fileStream = File.OpenRead(fileName);

        Shared = JsonSerializer.Deserialize<ApplicationIdentifiers>(fileStream, options) ?? new();
    }

    public bool TryGetOptimizationCode(string code, out OptimizationCode optimizationCode)
    {
        optimizationCode = OptimizationCodes.SingleOrDefault(x => x.Code == code, OptimizationCode.Default);

        return optimizationCode != OptimizationCode.Default;
    }

    internal bool TryGetMatchingOptimizationCode(IEnumerable<string> ais, out OptimizationCode optimizationCode)
    {
        optimizationCode = OptimizationCodes
            .OrderByDescending(x => x.Priority)
            .FirstOrDefault(x => x.IsFulfilledBy(ais), OptimizationCode.Default);

        return optimizationCode != OptimizationCode.Default;
    }
}