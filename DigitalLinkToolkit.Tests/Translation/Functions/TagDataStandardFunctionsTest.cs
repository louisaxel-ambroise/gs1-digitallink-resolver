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

    #region PrioritizedDate

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
    [TestClass]
    public class FormatPrioritizedDate
    {
        public Row Row = CreateRow("14.5.3");

        [TestMethod]
        [DataRow("520413", "00000110100010001101")]
        [DataRow("201230", "00000010100110011110")]
        public void ShouldWork(string input, string expected)
        {
            var result = TagDataStandardFunctions.Format(Row, input);

            Assert.AreEqual(expected, result);
        }
    }

    #endregion

    #region FixedLengthNumeric

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

    [TestClass]
    public class FormatFixedLengthNumeric
    {
        public Row Row = CreateRow("14.5.4");

        [TestMethod]
                          
        [DataRow("3688", "0011011010001000")]
        [DataRow("529", "010100101001")]
        public void ShouldWork(string input, string expected)
        {
            var result = TagDataStandardFunctions.Format(Row, input);

            Assert.AreEqual(expected, result);
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
        [DataRow("1010100011101111010010110", 2, 3, "1de96")]
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

    #region SingleDataBit

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
        public void ShouldThrowWhenBitStreamIsTooShort()
        {
            var binaryInput = new Bitstream("");

            string act() => TagDataStandardFunctions.Parse(Row, binaryInput);

            Assert.Throws<Exception>(act);
        }
    }

    [TestClass]
    public class FormatSingleDataBit
    {
        public Row Row = CreateRow("14.5.7");

        [TestMethod]
        [DataRow("0")]
        [DataRow("1")]
        public void ShouldWork(string value)
        {
            var result = TagDataStandardFunctions.Format(Row, value);

            Assert.AreEqual(value, result);
        }
    }

    #endregion

    #region 6DigitDateYYMMDD

    [TestClass]
    public class Parse6DigitDateYYMMDD
    {
        public Row Row = CreateRow("14.5.8");

        [TestMethod]
        [DataRow("1010011101001100", "831012")]
        [DataRow("0000010110011111", "021231")]
        public void ShouldWork(string bitStream, string expected)
        {
            var binaryInput = new Bitstream(bitStream);

            var result = TagDataStandardFunctions.Parse(Row, binaryInput);

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void ShouldThrowWhenBitStreamIsTooShort()
        {
            var binaryInput = new Bitstream("000001011001111");

            string act() => TagDataStandardFunctions.Parse(Row, binaryInput);

            Assert.Throws<Exception>(act);
        }
    }

    [TestClass]
    public class Format6DigitDateYYMMDD
    {
        public Row Row = CreateRow("14.5.8");

        [TestMethod]
        [DataRow("831012", "1010011101001100")]
        [DataRow("021231", "0000010110011111")]
        public void ShouldWork(string input, string expected)
        {
            var result = TagDataStandardFunctions.Format(Row, input);

            Assert.AreEqual(expected, result);
        }
    }

    #endregion

    #region 10DigitDateYYMMDDhhmm

    [TestClass]
    public class Parse10DigitDateYYMMDDhhmm
    {
        public Row Row = CreateRow("14.5.9");

        [TestMethod]
        [DataRow("001101010110010001110111001", "2611041457")]
        [DataRow("000000100010000110111111011", "0101012359")]
        public void ShouldWork(string bitStream, string expected)
        {
            var binaryInput = new Bitstream(bitStream);

            var result = TagDataStandardFunctions.Parse(Row, binaryInput);

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void ShouldThrowWhenBitStreamIsTooShort()
        {
            var binaryInput = new Bitstream("00110101011001000111011100");

            string act() => TagDataStandardFunctions.Parse(Row, binaryInput);

            Assert.Throws<Exception>(act);
        }
    }

    [TestClass]
    public class Format10DigitDateYYMMDDhhmm
    {
        public Row Row = CreateRow("14.5.9");

        [TestMethod]
        [DataRow("2611041457", "001101010110010001110111001")]
        [DataRow("0101012359", "000000100010000110111111011")]
        public void ShouldWork(string input, string expected)
        {
            var result = TagDataStandardFunctions.Format(Row, input);

            Assert.AreEqual(expected, result);
        }
    }

    #endregion

    #region VariableFormatDateOrDateRange

    [TestClass]
    public class ParseVariableFormatDateOrDateRange
    {
        public Row Row = CreateRow("14.5.10");

        [TestMethod]
        [DataRow("100000010001000010011010101100100", "010101261104")]
        [DataRow("00000001000100001", "010101")]
        [DataRow("00011010101100100", "261104")]
        public void ShouldWork(string bitStream, string expected)
        {
            var binaryInput = new Bitstream(bitStream);

            var result = TagDataStandardFunctions.Parse(Row, binaryInput);

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        [DataRow("10000001000100001001101010110010")]
        [DataRow("10000001000100001")]
        [DataRow("0000000100010000")]
        public void ShouldThrowWhenBitStreamIsTooShort(string bitStream)
        {
            var binaryInput = new Bitstream(bitStream);

            string act() => TagDataStandardFunctions.Parse(Row, binaryInput);

            Assert.Throws<Exception>(act);
        }
    }

    [TestClass]
    public class FormatVariableFormatDateOrDateRange
    {
        public Row Row = CreateRow("14.5.10");

        [TestMethod]
        [DataRow("010101261104", "100000010001000010011010101100100")]
        [DataRow("010101", "00000001000100001")]
        [DataRow("261104", "00011010101100100")]
        public void ShouldWork(string input, string expected)
        {
            var result = TagDataStandardFunctions.Format(Row, input);

            Assert.AreEqual(expected, result);
        }
    }

    #endregion

    #region VariablePrecisionDateTime

    [TestClass]
    public class ParseVariablePrecisionDateTime
    {
        public Row Row = CreateRow("14.5.11");

        [TestMethod]
        [DataRow("00000000100010000100110", "01010106")]
        [DataRow("01001101010110010001110111001", "2611041457")]
        [DataRow("10001101011001111110111111011111001", "261231235957")]
        [DataRow("110000001000100001", "010101")]
        public void ShouldWork(string bitStream, string expected)
        {
            var binaryInput = new Bitstream(bitStream);

            var result = TagDataStandardFunctions.Parse(Row, binaryInput);

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        [DataRow("0000000010001000010011")]
        [DataRow("0100110101011001000111011100")]
        [DataRow("1000110101100111111011111101111100")]
        [DataRow("11000000100010000")]
        public void ShouldThrowWhenBitStreamIsTooShort(string bitStream)
        {
            var binaryInput = new Bitstream(bitStream);

            string act() => TagDataStandardFunctions.Parse(Row, binaryInput);

            Assert.Throws<Exception>(act);
        }
    }

    [TestClass]
    public class FormatVariablePrecisionDateTime
    {
        public Row Row = CreateRow("14.5.11");

        [TestMethod]
        [DataRow("01010106", "00000000100010000100110")]
        [DataRow("2611041457", "01001101010110010001110111001")]
        [DataRow("261231235957", "10001101011001111110111111011111001")]
        [DataRow("010101", "110000001000100001")]
        public void ShouldWork(string input, string expected)
        {
            var result = TagDataStandardFunctions.Format(Row, input);

            Assert.AreEqual(expected, result);
        }
    }

    #endregion

    #region CountryCode

    [TestClass]
    public class ParseCountryCode
    {
        public Row Row = CreateRow("14.5.12");

        [TestMethod]
        [DataRow("000001000100", "BE")]
        [DataRow("000101010001", "FR")]
        public void ShouldWork(string bitStream, string expected)
        {
            var binaryInput = new Bitstream(bitStream);

            var result = TagDataStandardFunctions.Parse(Row, binaryInput);

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void ShouldThrowWhenBitStreamIsTooShort()
        {
            var binaryInput = new Bitstream("00100110001");

            string act() => TagDataStandardFunctions.Parse(Row, binaryInput);

            Assert.Throws<Exception>(act);
        }
    }

    [TestClass]
    public class FormatCountryCode
    {
        public Row Row = CreateRow("14.5.12");

        [TestMethod]
        [DataRow("BE", "000001000100")]
        [DataRow("FR", "000101010001")]
        public void ShouldWork(string input, string expected)
        {
            var result = TagDataStandardFunctions.Format(Row, input);

            Assert.AreEqual(expected, result);
        }
    }

    #endregion

    #region ParseVariableLengthNumericStringWithoutEncodingIndicator

    [TestClass]
    public class ParseVariableLengthNumericStringWithoutEncodingIndicator
    {
        public Row Row = CreateRow("14.5.13");

        [TestMethod]
        [DataRow(4, "010000000000000000", "0000")]
        [DataRow(4, "0100011001001101000", "6452")]
        [DataRow(5, "10010110000010100111100001011100010101111110010001000010101000101", "870585304003151173")]
        public void ShouldWork(int length, string bitStream, string expected)
        {
            Row.RowData.Add("g", JsonElement.Parse($"\"{length}\""));
            var binaryInput = new Bitstream(bitStream);

            var result = TagDataStandardFunctions.Parse(Row, binaryInput);

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        [DataRow(4, "01000000000000000")]
        [DataRow(5, "1001011000001010011110000101110001010111111001000100001010100010")]
        public void ShouldThrowWhenBitStreamIsTooShort(int length, string bitStream)
        {
            Row.RowData.Add("g", JsonElement.Parse($"\"{length}\""));
            var binaryInput = new Bitstream(bitStream);

            string act() => TagDataStandardFunctions.Parse(Row, binaryInput);

            Assert.Throws<Exception>(act);
        }
    }

    #endregion

    #region 10DigiParseOptionalMinusSignIn1BittDateYYMMDDhhmm

    [TestClass]
    public class ParseOptionalMinusSignIn1Bit
    {
        public Row Row = CreateRow("14.5.14");

        [TestMethod]
        [DataRow("0", "")]
        [DataRow("1", "-")]
        public void ShouldWork(string bitStream, string expected)
        {
            var binaryInput = new Bitstream(bitStream);

            var result = TagDataStandardFunctions.Parse(Row, binaryInput);

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void ShouldThrowWhenBitStreamIsTooShort()
        {
            var binaryInput = new Bitstream("");

            string act() => TagDataStandardFunctions.Parse(Row, binaryInput);

            Assert.Throws<Exception>(act);
        }
    }

    [TestClass]
    public class FormatOptionalMinusSignIn1Bit
    {
        public Row Row = CreateRow("14.5.14");

        [TestMethod]
        [DataRow("", "0")]
        [DataRow("-", "1")]
        public void ShouldWork(string input, string expected)
        {

            var result = TagDataStandardFunctions.Format(Row, input);

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void ShouldThrowWhenInputIsNotInExpectedRange()
        {
            var binaryInput = "+";

            string act() => TagDataStandardFunctions.Format(Row, binaryInput);

            Assert.Throws<Exception>(act);
        }
    }

    #endregion
}