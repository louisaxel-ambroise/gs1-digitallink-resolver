using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Goto.Services;

namespace Goto.Tests.Services;

public static class ExtensionsTests
{
    [TestClass]
    public class DateTimeOffsetExtensionsTests
    {
        [TestMethod]
        public void MinShouldReturnEarliestNonNull()
        {
            var a = DateTimeOffset.Parse("2020-01-01T00:00:00Z");
            var b = DateTimeOffset.Parse("2021-01-01T00:00:00Z");

            var result = Extensions.Min(a, null, b);

            Assert.AreEqual(a, result);
        }

        [TestMethod]
        public void MaxShouldReturnLatestNonNull()
        {
            var a = DateTimeOffset.Parse("2020-01-01T00:00:00Z");
            var b = DateTimeOffset.Parse("2021-01-01T00:00:00Z");

            var result = Extensions.Max(a, null, b);

            Assert.AreEqual(b, result);
        }
    }

    [TestClass]
    public class HttpResponseExtensionsTests
    {
        [TestMethod]
        [DataRow(100, true)]
        [DataRow(200, true)]
        [DataRow(399, true)]
        [DataRow(400, false)]
        [DataRow(404, false)]
        [DataRow(500, false)]
        [DataRow(501, false)]
        public void IsSuccessStatusCodeShouldWork(int statusCode, bool expected)
        {
            var ctx = new DefaultHttpContext();
            ctx.Response.StatusCode = statusCode;
            Assert.AreEqual(expected, ctx.Response.IsSuccessStatusCode());
        }
    }

    [TestClass]
    public class ClaimsPrincipalExtensionsTests
    {
        [TestMethod]
        public void GetCompanyPrefixShouldReturnClaimValue()
        {
            var claims = new[] { new Claim("gs1:gcp", "12345") };
            var principal = new ClaimsPrincipal(new ClaimsIdentity(claims));

            Assert.AreEqual("12345", principal.GetCompanyPrefix());
        }

        [TestMethod]
        public void GetCompanyPrefixShouldThrowWhenClaimIsMissing()
        {
            var emptyPrincipal = new ClaimsPrincipal(new ClaimsIdentity());
            var act = () => emptyPrincipal.GetCompanyPrefix();

            Assert.Throws<InvalidOperationException>(act);
        }
    }
}
