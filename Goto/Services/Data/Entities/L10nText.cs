using Goto.Services.Data.Enums;

namespace Goto.Services.Data.Entities;

public sealed class L10nText
{
    private readonly Dictionary<string, string> _translations = [];

    public L10nText(string language, string value)
    {
        _translations.Add(language, value);
    }

    public L10nText(Dictionary<string, string>? translations)
    {
        foreach(var translation in translations ?? [])
        {
            _translations.Add(translation.Key, translation.Value);
        }
    }

    public void SetValue(string language, string value)
    {
        _translations[language] = value;
    }

    public string? Localize(IEnumerable<Language> languages)
    {
        var translation = languages
            .Select(lang => _translations.Where(t => Language.Parse(t.Key).Matches(lang) is not Match.NoMatch))
            .FirstOrDefault(translation => translation.Any(), _translations);

        return translation?.FirstOrDefault().Value;
    }

    internal IDictionary<string, string> GetTranslations()
    {
        return _translations.AsReadOnly();
    }
}