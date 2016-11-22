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
        private const string TestData1_DivideBy32_KnownGood = ResourcePath + "TestData1_DivideBy32_KnownGood.png";
        private const string TestData1_DivideBy128_KnownGood = ResourcePath + "TestData1_DivideBy128_KnownGood.png";
        private const string TestData1_Pow2_KnownGood = ResourcePath + "TestData1_Pow2_KnownGood.png";
        private const string TestData2_KnownGood = ResourcePath + "TestData2_KnownGood.png";
        private const string TestData2_GrayScale_KnownGood = ResourcePath + "TestData2_GrayScale_KnownGood.png";
        private const string TestData3_KnownGood = ResourcePath + "TestData3_KnownGood.gif";
        private const string TestData3_GrayScale_KnownGood = ResourcePath + "TestData3_GrayScale_KnownGood.gif";
        private const string TestData4_KnownGood = ResourcePath + "TestData4_KnownGood.png";

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
            using (SUT.FastBitmap expected = 
                new SUT.FastBitmap(new Bitmap(Image.FromFile(FastBitmap.TestData1_KnownGood))))
            {
            }
        }

        [Fact]
        public void Blur_InvalidWeight_ThrowsArgument()
        {
            Assert.Throws<ArgumentException>(() => 
                SUT.FastBitmap.FromFile(FastBitmap.TestData1_KnownGood).Blur(1f, 0));
        }

        [Theory]
        [InlineData(32, FastBitmap.TestData1_KnownGood, FastBitmap.TestData1_DivideBy32_KnownGood)]
        [InlineData(128, FastBitmap.TestData1_KnownGood, FastBitmap.TestData1_DivideBy128_KnownGood)]
        public void ScaleBy_ValidFactor_ShouldSucceed(ushort factor, string input, string expectedOutput)
        {
            using (Bitmap expected = (Bitmap)Image.FromFile(expectedOutput))
            using (SUT.FastBitmap actual = SUT.FastBitmap.FromFile(input))
            {
                actual.ScaleBy(factor);

                FastBitmap.AssertContentsEqual(expected, actual.Buffer);
            }
        }

        [Theory]
        [InlineData(1D, 5, FastBitmap.TestData1_KnownGood, FastBitmap.TestData1_BlurSigma1Weight5_KnownGood)]
        [InlineData(1D, 25, FastBitmap.TestData1_KnownGood, FastBitmap.TestData1_BlurSigma1Weight25_KnownGood)]
        [InlineData(1.478D, 5, FastBitmap.TestData1_KnownGood, FastBitmap.TestData1_BlurSigma1478Weight5_KnownGood)]
        public void Blur_ValidParams_ShouldSucceed(double sigma, int weight, string input, string expectedOutput)
        {
            using (Bitmap expected = (Bitmap)Image.FromFile(expectedOutput))
            using (SUT.FastBitmap actual = SUT.FastBitmap.FromFile(input))
            {
                actual.Blur(sigma, weight);

                FastBitmap.AssertContentsEqual(expected, actual.Buffer);
            }
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
            using (SUT.FastBitmap expected = SUT.FastBitmap.FromFile(FastBitmap.TestData1_KnownGood))
            {
            }
        }

        [Theory]
        [InlineData(FastBitmap.TestData1_GrayScale_KnownGood, FastBitmap.TestData1_KnownGood)]
        [InlineData(FastBitmap.TestData2_GrayScale_KnownGood, FastBitmap.TestData2_KnownGood)]
        [InlineData(FastBitmap.TestData3_GrayScale_KnownGood, FastBitmap.TestData3_KnownGood)]
        public void Grayscale_VaryingBitColorDepths_ShouldSucceed(string expectedFile, string actualFile)
        {
            using (Bitmap expected = new Bitmap(Image.FromFile(expectedFile)))
            using (SUT.FastBitmap actual = SUT.FastBitmap.FromFile(actualFile))
            {
                actual.ToGrayscale();

                FastBitmap.AssertContentsEqual(expected, actual.Buffer);
            }
        }

        [Fact]
        public void Max_ValidBitmap_ShouldSucceed()
        {
            using (SUT.FastBitmap actual = SUT.FastBitmap.FromFile(FastBitmap.TestData4_KnownGood))
            {
                Assert.Equal(146, actual.Max());
            }
        }

        [Fact]
        public void Pow_ValidBitmap_ShouldSucceed()
        {
            using (Bitmap expected = new Bitmap(Image.FromFile(TestData1_Pow2_KnownGood)))
            using (SUT.FastBitmap actual = SUT.FastBitmap.FromFile(FastBitmap.TestData1_KnownGood))
            {
                actual.Pow(2);

                FastBitmap.AssertContentsEqual(expected, actual.Buffer);
            }
        }

        private static void AssertContentsEqual(Bitmap expected, Bitmap actual)
        {
            Point? mismatch;
            bool result = TestUtil.ContentsEqual(expected, actual, out mismatch) && !mismatch.HasValue;

            Assert.True(result, result ? string.Empty : $"Mismatch detected at {mismatch.Value.ToString()}");
        }
    }
}
