using DigitalLinkToolkit.Translation;
using DigitalLinkToolkit.Translation.Functions;
using DigitalLinkToolkit.Translation.Model.Tables;
using System.Text.Json;

namespace DigitalLinkToolkit.Tests.Translation.Functions;

public static class TagDataStandardFunctionsTest
{
    private static Row CreateRow(string section) => new() { RowData = new() { { "c", JsonElement.Parse($"\"{section}\"") } } };

    #region ParseFixedBitLengthNumeric

    [TestClass]
    public class ParseFixedBitLengthNumeric
    {
        public Row Row = CreateRow("14.5.2");

        [TestMethod]
        [DataRow(2, "0")]
        [DataRow(5, "4")]
        [DataRow(4, "2")]
        public void ShouldWork(int length, string expected)
        {
            var binaryInput = new Bitstream("001001110");
            Row.RowData.Add("e", JsonElement.Parse($"\"{length}\""));

            var result = TagDataStandardFunctions.Parse(Row, binaryInput);

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void ShouldFailWhenBitStreamHasNotEnoughData()
        {
            var binaryInput = new Bitstream("001001110");
            Row.RowData.Add("e", JsonElement.Parse($"\"10\""));

            string act() => TagDataStandardFunctions.Parse(Row, binaryInput);

            Assert.Throws<Exception>(act);
        }
    }

    #endregion

    #region ParsePrioritizedDate

    [TestClass]
    public class ParsePrioritizedDate
    {
        public Row Row = CreateRow("14.5.3");

        [TestMethod]
        [DataRow("00110110100010001101", "520413")]
        [DataRow("01010010100110011110", "201230")]
        public void ShouldWork(string bitStream, string expected)
        {
            var binaryInput = new Bitstream(bitStream);

            var result = TagDataStandardFunctions.Parse(Row, binaryInput);

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        [DataRow("00000000000000000000")]
        [DataRow("11111111111111111111")]
        public void ShouldFailWhenBinaryDataIsInvalid(string bitStream)
        {
            var binaryInput = new Bitstream(bitStream);

            string act() => TagDataStandardFunctions.Parse(Row, binaryInput);

            Assert.Throws<Exception>(act);
        }

        [TestMethod]
        public void ShouldFailWhenBitStreamHasNotEnoughData()
        {
            var binaryInput = new Bitstream("00101001110");
            string act() => TagDataStandardFunctions.Parse(Row, binaryInput);

            Assert.Throws<Exception>(act);
        }
    }

    #endregion

    #region ParseFixedLengthNumeric

    [TestClass]
    public class ParseFixedLengthNumeric
    {
        public Row Row = CreateRow("14.5.4");

        [TestMethod]
        [DataRow("00110110100010001101", 16, "3688")]
        [DataRow("01010010100110011110", 12, "529")]
        public void ShouldWork(string bitStream, int length, string expected)
        {
            var binaryInput = new Bitstream(bitStream);
            Row.RowData.Add("e", JsonElement.Parse($"\"{length}\""));

            var result = TagDataStandardFunctions.Parse(Row, binaryInput);

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void ShouldFailWhenBitStreamHasNotEnoughData()
        {
            var binaryInput = new Bitstream("00101001110");
            Row.RowData.Add("e", JsonElement.Parse($"\"20\""));

            string act() => TagDataStandardFunctions.Parse(Row, binaryInput);

            Assert.Throws<Exception>(act);
        }
    }

    #endregion

    #region ParseDelimitedOrTerminatedNumeric

    [TestClass]
    public class ParseDelimitedOrTerminatedNumeric
    {
        public Row Row = CreateRow("14.5.5");

        [TestMethod, Ignore("TODO")]
        public void ShouldWork()
        {
        }
    }

    #endregion

    #region ParseVariableLengthAlphanumeric

    [TestClass]
    public class ParseVariableLengthAlphanumeric
    {
        public Row Row = CreateRow("14.5.6");

        [TestMethod]
        [DataRow("01001001001010010100", 1, 3, "9381")]
        [DataRow("1010100011101111010010110", 2, 3, "1CD96")]
        [DataRow("011101000111011110100101100101101000", 3, 3, "Hello")]
        public void ShouldWork(string bitStream, int encoding, int length, string expected)
        {
            var binaryInput = new Bitstream(bitStream);
            Row.RowData.Add("f", JsonElement.Parse($"\"{encoding}\""));
            Row.RowData.Add("g", JsonElement.Parse($"\"{length}\""));

            var result = TagDataStandardFunctions.Parse(Row, binaryInput);

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void ShouldThrowWhenEncodingIsNotInValidRange()
        {
            var binaryInput = new Bitstream("011101000111011110100101100101101000");
            Row.RowData.Add("f", JsonElement.Parse($"\"5\""));
            Row.RowData.Add("g", JsonElement.Parse($"\"3\""));

            string act() => TagDataStandardFunctions.Parse(Row, binaryInput);

            Assert.Throws<Exception>(act);
        }

        [TestMethod]
        public void ShouldThrowWhenBitStreamIsTooShort()
        {
            var binaryInput = new Bitstream("01110100011101111010010");
            Row.RowData.Add("f", JsonElement.Parse($"\"3\""));
            Row.RowData.Add("g", JsonElement.Parse($"\"3\""));

            string act() => TagDataStandardFunctions.Parse(Row, binaryInput);

            Assert.Throws<Exception>(act);
        }
    }

    #endregion

    #region ParseSingleDataBit

    [TestClass]
    public class ParseSingleDataBit
    {
        public Row Row = CreateRow("14.5.7");

        [TestMethod]
        [DataRow("0")]
        [DataRow("1")]
        public void ShouldWork(string bitStream)
        {
            var binaryInput = new Bitstream(bitStream);

            var result = TagDataStandardFunctions.Parse(Row, binaryInput);

            Assert.AreEqual(bitStream, result);
        }

        [TestMethod]
        public void ShouldThrowWhenEncodingIsNotInValidRange()
        {
            var binaryInput = new Bitstream("");

            string act() => TagDataStandardFunctions.Parse(Row, binaryInput);

            Assert.Throws<Exception>(act);
        }
    }

    #endregion

    // TODO: missing tests 14.5.8 -> 14.5.14
}