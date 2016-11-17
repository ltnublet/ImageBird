using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using SUT = ImageBird;

namespace ImageBird.Tests
{
    /// <summary>
    /// Tests for the ImageBird.FastBitmap class.
    /// </summary>
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Underscores are used to make names human-readable.")]
    [SuppressMessage("ReSharper", "StyleCop.SA1310", Justification = "Underscores are used to make names human-readable.")]
    [SuppressMessage("ReSharper", "StyleCop.SA1600", Justification = "Test names should adequately describe the SUT, supplied data, and outcome.")]
    public class FastBitmap
    {
        private const string ResourcePath = @"..\..\Resources\";

        private const string TestData1_KnownGood = ResourcePath + "TestData1_KnownGood.png";
        private const string TestData1_GrayScale_KnownGood = ResourcePath + "TestData1_GrayScale_KnownGood.png";
        private const string TestData1_BlurSigma1Weight5_KnownGood = ResourcePath + "TestData1_BlurSigma1Weight5_KnownGood.png";
        private const string TestData1_BlurSigma1Weight25_KnownGood = ResourcePath + "TestData1_BlurSigma1Weight25_KnownGood.png";
        private const string TestData1_BlurSigma1478Weight5_KnownGood = ResourcePath + "TestData1_BlurSigma1.478Weight5_KnownGood.png";
        private const string TestData2_KnownGood = ResourcePath + "TestData2_KnownGood.png";
        private const string TestData2_GrayScale_KnownGood = ResourcePath + "TestData2_GrayScale_KnownGood.png";
        private const string TestData3_KnownGood = ResourcePath + "TestData3_KnownGood.gif";
        private const string TestData3_GrayScale_KnownGood = ResourcePath + "TestData3_GrayScale_KnownGood.gif";

        private const string TestData_txt = ResourcePath + "TestData_txt.txt";

        [Fact]
        public void FastBitmap_NullBitmap_ThrowsArgumentNull()
        {
            Assert.Throws<ArgumentNullException>(() => new SUT.FastBitmap((Bitmap)null));
        }

        [Fact]
        public void FastBitmap_InvalidBitmap_ThrowsArgument()
        {
            Assert.Throws<ArgumentException>(() => new SUT.FastBitmap(new Bitmap(0, 0)));
        }

        [Fact]
        public void FastBitmap_ValidBitmap_ShouldSucceed()
        {
            SUT.FastBitmap expected = new SUT.FastBitmap(new Bitmap(Image.FromFile(FastBitmap.TestData1_KnownGood)));
            expected.Dispose();
        }

        [Fact]
        public void Blur_InvalidWeight_ThrowsArgument()
        {
            Assert.Throws<ArgumentException>(() => 
            SUT.FastBitmap.FromFile(FastBitmap.TestData1_KnownGood).Blur(1f, 0));
        }

        [Fact]
        public void DivideBy_ValidFactor_ShouldSucceed()
        {
            SUT.FastBitmap expected = SUT.FastBitmap.FromFile(FastBitmap.TestData1_KnownGood);
            SUT.FastBitmap actual = SUT.FastBitmap.FromFile(FastBitmap.TestData1_KnownGood);
            actual.DivideBy(128);

            actual.Buffer.Save("whatuplol.png");

            Point? mismatch;
            bool result = TestUtil.ContentsEqual(expected.Buffer, actual.Buffer, out mismatch) && !mismatch.HasValue;

            Assert.True(result, result ? string.Empty : mismatch.Value.ToString());
        }

        [Theory]
        [InlineData(1D, 5, FastBitmap.TestData1_KnownGood, FastBitmap.TestData1_BlurSigma1Weight5_KnownGood)]
        [InlineData(1D, 25, FastBitmap.TestData1_KnownGood, FastBitmap.TestData1_BlurSigma1Weight25_KnownGood)]
        [InlineData(1.478D, 5, FastBitmap.TestData1_KnownGood, FastBitmap.TestData1_BlurSigma1478Weight5_KnownGood)]
        public void Blur_ValidParams_ShouldSucceed(double sigma, int weight, string input, string expectedOutput)
        {
            Bitmap expected = (Bitmap)Image.FromFile(expectedOutput);

            SUT.FastBitmap actual = SUT.FastBitmap.FromFile(input);
            actual.Blur(sigma, weight);

            Assert.True(TestUtil.ContentsEqual(expected, actual.Buffer));
        }

        [Fact]
        public void FromFile_NullPath_ThrowsArgumentNull()
        {
            Assert.Throws<ArgumentNullException>(() => SUT.FastBitmap.FromFile(null));
        }

        [Fact]
        public void FromFile_NonexistentFile_ThrowsArgument()
        {
            Assert.Throws<ArgumentException>(() => SUT.FastBitmap.FromFile(string.Empty));
        }

        public void FromFile_NotABitmap_ThrowsArgument()
        {
            Assert.Throws<ArgumentException>(() => SUT.FastBitmap.FromFile(TestData_txt));
        }

        [Fact]
        public void FromFile_ValidBitmap_ShouldSucceed()
        {
            SUT.FastBitmap expected = SUT.FastBitmap.FromFile(FastBitmap.TestData1_KnownGood);
            expected.Dispose();
        }

        [Fact]
        public void Grayscale_32BitColorDepth_ShouldSucceed()
        {
            Bitmap expected = new Bitmap(Image.FromFile(FastBitmap.TestData1_GrayScale_KnownGood));

            SUT.FastBitmap actual = SUT.FastBitmap.FromFile(FastBitmap.TestData1_KnownGood);
            actual.ToGrayscale();

            Assert.True(TestUtil.ContentsEqual(expected, actual.Buffer));
        }

        [Fact]
        public void Grayscale_24BitColorDepth_ShouldSucceed()
        {
            Bitmap expected = new Bitmap(Image.FromFile(FastBitmap.TestData2_GrayScale_KnownGood));

            SUT.FastBitmap actual = SUT.FastBitmap.FromFile(FastBitmap.TestData2_KnownGood);
            actual.ToGrayscale();

            Assert.True(TestUtil.ContentsEqual(expected, actual.Buffer));
        }

        [Fact]
        public void Grayscale_8BitColorDepth_ShouldSucceed()
        {
            Bitmap expected = new Bitmap(Image.FromFile(FastBitmap.TestData3_GrayScale_KnownGood));

            SUT.FastBitmap actual = SUT.FastBitmap.FromFile(FastBitmap.TestData3_KnownGood);
            actual.ToGrayscale();

            Assert.True(TestUtil.ContentsEqual(expected, actual.Buffer));
        }
    }
}
