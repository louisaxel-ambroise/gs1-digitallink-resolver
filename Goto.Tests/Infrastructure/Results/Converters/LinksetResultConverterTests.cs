using Goto.Infrastructure.Results.Converters;
using System.Text.Json;
using Goto.Controllers.Results;

namespace Goto.Tests.Infrastructure.Results.Converters;

[TestClass]
public class LinksetResultConverterTests
{
    [TestMethod]
    public void ShouldSerializeLinksetWithDefaultLinkAndLinkTypes()
    {
        var options = new JsonSerializerOptions();
        options.Converters.Add(new LinksetResultConverter());

        var result = new LinksetResult
        {
            LinksetUrl = "https://linkset.test",
            Anchors = new[]
            {
                new LinksetResultAnchor
                {
                    Anchor = "01/123",
                    Description = "desc",
                    Links =
                    [
                        new LinksetLink { LinkType = "gs1:pip", Title = "t", Href = "h", IsDefault = true },
                        new LinksetLink { LinkType = "gs1:homepage", Title = "t2", Href = "h2" }
                    ]
                }
            }
        };

        var json = JsonSerializer.Serialize(result, options);

        Assert.Contains("https://ref.gs1.org/voc/pip", json);
        Assert.Contains("https://ref.gs1.org/voc/homepage", json);
        Assert.Contains("https://ref.gs1.org/voc/defaultLink", json);
    }

    [TestMethod]
    public void ShouldIncludeDefaultLinkMultiWhenMultipleDefaults()
    {
        var options = new JsonSerializerOptions();
        options.Converters.Add(new LinksetResultConverter());

        var result = new LinksetResult
        {
            LinksetUrl = "https://linkset.test",
            Anchors = new[]
            {
                new LinksetResultAnchor
                {
                    Anchor = "01/123",
                    Description = "desc",
                    Links =
                    [
                        new LinksetLink { LinkType = "gs1:pip", Title = "t", Href = "h", IsDefault = true },
                        new LinksetLink { LinkType = "gs1:pip", Title = "t2", Href = "h2", IsDefault = true }
                    ]
                }
            }
        };

        var json = JsonSerializer.Serialize(result, options);

        Assert.Contains("defaultLinkMulti", json);
    }
}
