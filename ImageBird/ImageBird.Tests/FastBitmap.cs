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
        private const string TestData1_EdgeDetect_KnownGood = ResourcePath + "TestData1_CannyEdgeDetect_KnownGood.png";
        private const string TestData1_Pow2_KnownGood = ResourcePath + "TestData1_Pow2_KnownGood.png";
        private const string TestData1_Sobel_KnownGood = ResourcePath + "TestData1_Sobel_KnownGood.png";
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
                FastBitmap.AssertContentsEqual(expected, actual.ScaleBy(factor).Content);
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
                FastBitmap.AssertContentsEqual(expected, actual.Blur(sigma, weight).Content);
            }
        }

        [Fact]
        public void EdgeDetect_ValidBitmap_ShouldSucceed()
        {
            using (SUT.FastBitmap expected = SUT.FastBitmap.FromFile(TestData1_EdgeDetect_KnownGood))
            using (SUT.FastBitmap actual = SUT.FastBitmap.FromFile(TestData1_KnownGood).EdgeDetect())
            {
                actual.Content.Save("output.png");
                FastBitmap.AssertContentsEqual(expected.Content, actual.Content);
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
                FastBitmap.AssertContentsEqual(expected, actual.ToGrayscale().Content);
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
                FastBitmap.AssertContentsEqual(expected, actual.Pow(2).Content);
            }
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void RadonTransform_InvalidNumberOfAngles_ThrowsArgument(int numberOfAngles)
        {
            using (SUT.FastBitmap actual = SUT.FastBitmap.FromFile(FastBitmap.TestData1_KnownGood))
            {
                Assert.Throws<ArgumentException>(() => actual.RadonTransform(numberOfAngles));
            }
        }

        [Fact(Skip = "This test isn't working correctly yet.")]
        public void RadonTransform_ValidBitmap_ShouldSucceed()
        {
            using (SUT.FastBitmap input = SUT.FastBitmap.FromFile(FastBitmap.TestData1_KnownGood))
            {
                SUT.FastBitmap actual = input.ToGrayscale().Blur(1f, 5);
                actual = actual.ScaleBy(actual.Max()).Pow(2);
                actual.Content.Save("testing_before.png");
                actual.RadonTransform(actual.Content.Width).Transform.Content.Save("testing_after.png");
            }
        }

        [Fact]
        public void Sobel_ValidBitmap_ShouldSucceed()
        {
            using (SUT.FastBitmap expected = SUT.FastBitmap.FromFile(FastBitmap.TestData1_Sobel_KnownGood))
            using (SUT.FastBitmap actual = SUT.FastBitmap.FromFile(FastBitmap.TestData1_KnownGood).Sobel().magnitude)
            {
                actual.Content.Save("sobel.png");
                AssertContentsEqual(expected.Content, actual.Content);
            }
        }

        private static void AssertContentsEqual(Bitmap expected, Bitmap actual)
        {
#pragma warning disable IDE0018 // Inline variable declaration
            (Point point, Color left, Color right)? mismatch;
#pragma warning restore IDE0018 // Inline variable declaration
            bool result = TestUtil.ContentsEqual(expected, actual, out mismatch) && !mismatch.HasValue;

            string message = mismatch.HasValue
                ? $"Mismatch detected at {mismatch.Value.point.ToString()}: expected {mismatch.Value.left}, got {mismatch.Value.right}"
                : "Mismatch detected, but was not due to color mismatch (did the bitmaps have the same dimensions?)";

            Assert.True(result, result ? string.Empty : message);
        }
    }
}
