using DigitalLinkToolkit.Compression.Functions;

namespace DigitalLinkToolkit.Tests.Translation.Functions;

public static class FunctionUtilsTests
{
    #region SUBSTR

    [TestClass]
    public class SUBSTR
    {
        [TestMethod]
        [DataRow("ThisIsMyValue", "SUBSTR(value,0,4)", "This")]
        [DataRow("ThisIsMyValue", "SUBSTR(value,4,2)", "Is")]
        [DataRow("ThisIsMyValue", "SUBSTR(value,6)", "MyValue")]
        public void ShouldWork(string value, string functionDefinition, string expected)
        {
            var parameters = new Dictionary<string, string> { { "value", value } };
            var result = FunctionUtils.Execute(functionDefinition, parameters);

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        [DataRow("ThisIsMyValue", "SUBSTR(value)")]
        [DataRow("ThisIsMyValue", "SUBSTR(value,0,4,5)")]
        public void ShouldThrowWhenInvalidArgumentLength(string value, string functionDefinition)
        {
            var parameters = new Dictionary<string, string> { { "value", value } };
            var act = () => FunctionUtils.Execute(functionDefinition, parameters);

            Assert.Throws<Exception>(act);
        }

        [TestMethod]
        public void ShouldThrowWhenParameterDoesNotExist()
        {
            var parameters = new Dictionary<string, string> { { "value", "test" } };
            var act = () => FunctionUtils.Execute("SUBSTR(unknownValue,0,4)", parameters);

            Assert.Throws<Exception>(act);
        }
    }

    #endregion

    #region CONCAT

    [TestClass]
    public class CONCAT
    {
        [TestMethod]
        public void ShouldWork()
        {
            var parameters = new Dictionary<string, string> { { "value1", "value1" }, { "value2", "value2" } };
            var result = FunctionUtils.Execute("CONCAT(value1,value2)", parameters);

            Assert.AreEqual("value1value2", result);
        }

        [TestMethod]
        public void ShouldThrowWhenParameterDoesNotExist()
        {
            var parameters = new Dictionary<string, string> { { "value1", "value1" }, { "value2", "value2" } };
            var act = () => FunctionUtils.Execute("CONCAT(value1,value3)", parameters);

            Assert.Throws<Exception>(act);
        }
    }

    #endregion

    #region LENGTH

    [TestClass]
    public class LENGTH
    {
        [TestMethod]
        [DataRow("Test")]
        [DataRow("ThisIsMyValue")]
        [DataRow("")]
        public void ShouldWork(string value)
        {
            var parameters = new Dictionary<string, string> { { "value", value } };
            var result = FunctionUtils.Execute("LENGTH(value)", parameters);

            Assert.AreEqual(value.Length.ToString(), result);
        }

        [TestMethod]
        public void ShouldThrowWhenParameterDoesNotExist()
        {
            var parameters = new Dictionary<string, string> { { "value1", "value1" } };
            var act = () => FunctionUtils.Execute("LENGTH(value)", parameters);

            Assert.Throws<Exception>(act);
        }
    }

    #endregion

    #region URLENCODE

    [TestClass]
    public class URLENCODE
    {
        [TestMethod]
        [DataRow("https://test.com/", "https%3A%2F%2Ftest.com%2F")]
        [DataRow("NothingToEncode", "NothingToEncode")]
        [DataRow("", "")]
        public void ShouldWork(string value, string expected)
        {
            var parameters = new Dictionary<string, string> { { "value", value } };
            var result = FunctionUtils.Execute("URLENCODE(value)", parameters);

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void ShouldThrowWhenParameterDoesNotExist()
        {
            var parameters = new Dictionary<string, string> { { "value1", "value1" } };
            var act = () => FunctionUtils.Execute("URLENCODE(value)", parameters);

            Assert.Throws<Exception>(act);
        }
    }

    #endregion

    #region URNENCODE

    [TestClass]
    public class URNENCODE
    {
        [TestMethod]
        [DataRow("https://test.com/", "https%3A%2F%2Ftest.com%2F")]
        [DataRow("NothingToEncode", "NothingToEncode")]
        [DataRow("", "")]
        public void ShouldWork(string value, string expected)
        {
            var parameters = new Dictionary<string, string> { { "value", value } };
            var result = FunctionUtils.Execute("URNENCODE(value)", parameters);

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void ShouldThrowWhenParameterDoesNotExist()
        {
            var parameters = new Dictionary<string, string> { { "value1", "value1" } };
            var act = () => FunctionUtils.Execute("URNENCODE(value)", parameters);

            Assert.Throws<Exception>(act);
        }
    }

    #endregion

    #region URLDECODE

    [TestClass]
    public class URLDECODE
    {
        [TestMethod]
        [DataRow("https%3A%2F%2Ftest.com%2F", "https://test.com/")]
        [DataRow("https://test.com/", "https://test.com/")]
        [DataRow("NothingToEncode", "NothingToEncode")]
        [DataRow("", "")]
        public void ShouldWork(string value, string expected)
        {
            var parameters = new Dictionary<string, string> { { "value", value } };
            var result = FunctionUtils.Execute("URLDECODE(value)", parameters);

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void ShouldThrowWhenParameterDoesNotExist()
        {
            var parameters = new Dictionary<string, string> { { "value1", "value1" } };
            var act = () => FunctionUtils.Execute("URLDECODE(value)", parameters);

            Assert.Throws<Exception>(act);
        }
    }

    #endregion

    #region URNDECODE

    [TestClass]
    public class URNDECODE
    {
        [TestMethod]
        [DataRow("https%3A%2F%2Ftest.com%2F", "https://test.com/")]
        [DataRow("https://test.com/", "https://test.com/")]
        [DataRow("NothingToEncode", "NothingToEncode")]
        [DataRow("", "")]
        public void ShouldWork(string value, string expected)
        {
            var parameters = new Dictionary<string, string> { { "value", value } };
            var result = FunctionUtils.Execute("URNDECODE(value)", parameters);

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void ShouldThrowWhenParameterDoesNotExist()
        {
            var parameters = new Dictionary<string, string> { { "value1", "value1" } };
            var act = () => FunctionUtils.Execute("URNDECODE(value)", parameters);

            Assert.Throws<Exception>(act);
        }
    }

    #endregion

    #region GS1CHECKSUM

    [TestClass]
    public class GS1CHECKSUM
    {
        [TestMethod]
        [DataRow("0123465789", "7")]
        [DataRow("5555555555", "0")]
        [DataRow("000", "0")]
        public void ShouldWork(string value, string expected)
        {
            var parameters = new Dictionary<string, string> { { "value", value } };
            var result = FunctionUtils.Execute("GS1CHECKSUM(value)", parameters);

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void ShouldThrowWhenParameterDoesNotExist()
        {
            var parameters = new Dictionary<string, string> { { "value1", "value1" } };
            var act = () => FunctionUtils.Execute("GS1CHECKSUM(value)", parameters);

            Assert.Throws<Exception>(act);
        }
    }

    #endregion

    #region UnknownFunction

    [TestClass]
    public class UnknownFunction
    {
        [TestMethod]
        [DataRow("DECODE(value,5)")]
        [DataRow("SUP(value,5)")]
        [DataRow("UNKNOWN(value)")]
        public void ShouldThrow(string functionDefinition)
        {
            var parameters = new Dictionary<string, string> { { "value", "test" } };
            var act = () => FunctionUtils.Execute(functionDefinition, parameters);

            Assert.Throws<Exception>(act);
        }
    }

    #endregion
}
