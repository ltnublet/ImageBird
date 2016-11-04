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
        private const string TestData1_GrayScale_KnownBad = ResourcePath + "TestData1_GrayScale_KnownBad.png";

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
        public void Grayscale_ValidBitmap_ShouldSucceed()
        {
            Bitmap expected = new Bitmap(Image.FromFile(FastBitmap.TestData1_GrayScale_KnownGood));
            Bitmap notExpected = new Bitmap(Image.FromFile(FastBitmap.TestData1_GrayScale_KnownBad));

            SUT.FastBitmap actual = new SUT.FastBitmap(new Bitmap(Image.FromFile(FastBitmap.TestData1_KnownGood)));

            actual.ToGrayscale();

            Assert.True(TestUtil.ContentsEqual(expected, actual.Buffer));
            Assert.False(TestUtil.ContentsEqual(notExpected, actual.Buffer));
        }
    }
}
