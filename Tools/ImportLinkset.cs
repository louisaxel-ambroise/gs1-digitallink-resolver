#:project ../DigitalLinkToolkit/DigitalLinkToolkit.csproj
#:property TargetFramework=net11.0
#:property PublishAot=false

using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Text.Json;
using DigitalLinkToolkit.Conversion;
using DigitalLinkToolkit.Conversion.Validation;
using DigitalLinkToolkit.Conversion.Model;

static async Task<string> EnsureAnchorExists(HttpClient client, DigitalLink digitalLink, JsonElement anchorElement)
{
    var anchor = anchorElement.GetProperty("anchor").GetString();
    var description = anchorElement.GetProperty("description").GetString() ?? "No description";
    var request = new
    {
        prefix = digitalLink.ToString(),
        description = new 
        {
            en = description
        }
    };

    await client.PostAsync("/api/anchors", new StringContent(JsonSerializer.Serialize(request), System.Text.Encoding.UTF8, "application/json"));

    var anchorsResponse = await client.GetAsync($"/api/anchors");
    var anchorResult = await anchorsResponse.Content.ReadAsStringAsync();

    var jsonResponse = JsonDocument.Parse(anchorResult);

    return jsonResponse!.RootElement.GetProperty("anchors").EnumerateArray()
        .First(a => a.GetProperty("prefix").GetString() == digitalLink.ToString())
        .GetProperty("id").GetString();
}

static async Task ProcessAnchorAsync(string gotoResolverUrl, string apiKey, JsonElement anchorElement, DigitalLinkConverter converter)
{
    var anchor = anchorElement.GetProperty("anchor").GetString();
    var description = anchorElement.GetProperty("description").GetString() ?? "No description";
    var digitalLink = converter.Parse(new Uri(anchor ?? string.Empty));

    using (var client = new HttpClient())
    {
        client.BaseAddress = new Uri(gotoResolverUrl);
        client.DefaultRequestHeaders.Add("x-api-key", apiKey);

        var anchorId = await EnsureAnchorExists(client, digitalLink, anchorElement);
        await client.DeleteAsync($"/api/anchors/{anchorId}");

        var linkTypes = anchorElement.EnumerateObject().Where(kv => kv.Name.StartsWith("https://ref.gs1.org/voc/"));
        var defaultLinks = anchorElement.EnumerateObject().Where(kv => kv.Name.StartsWith("https://ref.gs1.org/voc/defaultLink")).OrderByDescending(kv => kv.Name.Length).First();
        var defaultUrls = defaultLinks.Value.EnumerateArray().Select(link => link.GetProperty("href").GetString()).ToList();

        foreach (var linkType in linkTypes)
        {
            var gs1Type = $"gs1:{linkType.Name[24..]}";

            foreach (var link in linkType.Value.EnumerateArray())
            {
                if (!gs1Type.StartsWith("gs1:defaultLink"))
                {
                    var redirectUrl = link.GetProperty("href").GetString();
                    var title = link.GetProperty("title").GetString();
                    var mediaType = link.GetProperty("type").GetString();
                    var languages = link.GetProperty("hreflang").EnumerateArray().Select(l => l.GetString()).ToArray();
                    var isDefault = defaultUrls.Contains(redirectUrl);

                    var linkRequest = new
                    {
                        type = gs1Type,
                        redirectUrl,
                        title,
                        mediaType,
                        languages,
                        isDefault
                    };

                    await client.PostAsync($"/api/anchors/{anchorId}/links", new StringContent(JsonSerializer.Serialize(linkRequest), System.Text.Encoding.UTF8, "application/json"));
                }
            }
        }
    }
}

CompanyPrefix.Initialize("Files/gcpprefixformatlist.xml");
ApplicationIdentifiers.Initialize("Files/ApplicationIdentifiers.json", new() { PropertyNameCaseInsensitive = true });

if(args.Length < 3)
{
    Console.WriteLine("Usage: ImportLinkset <apiKey> <linksetUrl> <gotoResolverUrl>");
    return;
}

var converter = new DigitalLinkConverter(ApplicationIdentifiers.Shared, new([], []));
var url = args[1];
var responseBody = string.Empty;

using (var client = new HttpClient())
{
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/linkset+json"));

    try
    {
		HttpResponseMessage response = await client.GetAsync(url);
		response.EnsureSuccessStatusCode();

		responseBody = await response.Content.ReadAsStringAsync();
    }
	catch (HttpRequestException e)
	{
		Console.WriteLine($"Request error: {e.Message}");
	}
}

var jsonResponse = JsonDocument.Parse(responseBody);
var linkset = jsonResponse.RootElement.GetProperty("linkset");

foreach (var anchor in linkset.EnumerateArray())
{
    await ProcessAnchorAsync(args[2], args[0], anchor, converter);
}