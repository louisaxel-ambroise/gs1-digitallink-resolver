namespace DigitalLinkToolkit.Conversion.Validation;

public static class CheckDigit
{
    public static char Calculate(string input)
    {
        var weightedSum = 0;

        for (var i = 0; i < input.Length; i++)
        {
            var weight = i % 2 == 0 ? 3 : 1;
            weightedSum += (input[i] - '0') * weight;
        }

        var checkDigit = 10 - weightedSum % 10;

        return (char)(checkDigit % 10 + '0');
    }
}
