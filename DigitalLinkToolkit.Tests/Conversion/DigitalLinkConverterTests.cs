using DigitalLinkToolkit.Conversion;
using DigitalLinkToolkit.Conversion.Validation;

namespace DigitalLinkToolkit.Tests.Conversion;

public static class DigitalLinkConverterTests
{
    public static DigitalLinkConverter DigitalLinkConverter { get; } = InitializeConverter();

    private static DigitalLinkConverter InitializeConverter()
    {
        CompanyPrefix.Initialize("Resources/gcpprefixformatlist.xml");
        ApplicationIdentifiers.Initialize("Resources/ApplicationIdentifiers.json", new () { PropertyNameCaseInsensitive = true });
        
        return new DigitalLinkConverter(ApplicationIdentifiers.Shared, new([], []));
    }

    #region Parse

    [TestClass]
    public class Parse
    {
        [TestMethod]
        public void ItShouldSetTheHostUrl()
        {
            var digitalLink = DigitalLinkConverter.Parse("https://id.goto.it.com", "01/05414195535264", string.Empty);

            Assert.AreEqual("https://id.goto.it.com", digitalLink.HostUrl);
        }

        [TestMethod]
        [DataRow("01/05414195535264", "5414195")]
        [DataRow("01/05414196535263/21/XYZ", "5414196")]
        public void ItShouldParseTheCompanyPrefix(string path, string expectedCompanyPrefix)
        {
            var digitalLink = DigitalLinkConverter.Parse("https://id.goto.it.com", path, string.Empty);
            
            Assert.AreEqual(expectedCompanyPrefix, digitalLink.CompanyPrefix);
        }

        [TestMethod]
        [DataRow("01/05414195535264", "", new[] { "01" })]
        [DataRow("01/05414196535263/21/XYZ", "", new[] { "01", "21" })]
        [DataRow("01/05414196535263/10/123456", "7011=1221541151", new[] { "01", "10", "7011" })]
        [DataRow("01/05414196535263/10/123456", "7011=1221541151&nonAI=true", new[] { "01", "10", "7011" })]
        public void ItShouldParseTheApplicationIdentifiers(string path, string query, string[] expectedAIs)
        {
            var digitalLink = DigitalLinkConverter.Parse("https://id.goto.it.com", path, query);

            Assert.AreSequenceEqual(expectedAIs, digitalLink.AIs.Select(x => x.Key.Code).ToArray());
        }
    }

    #endregion
}
