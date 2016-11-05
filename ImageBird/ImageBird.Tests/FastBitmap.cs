﻿using System;
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
        private const string TestData2_KnownGood = ResourcePath + "TestData2_KnownGood.png";
        private const string TestData2_GrayScale_KnownGood = ResourcePath + "TestData2_GrayScale_KnownGood.png";
        private const string TestData3_KnownGood = ResourcePath + "TestData3_KnownGood.gif";
        private const string TestData3_GrayScale_KnownGood = ResourcePath + "TestData3_GrayScale_KnownGood.gif";

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
        public void Grayscale_32BitColorDepth_ShouldSucceed()
        {
            Bitmap expected = new Bitmap(Image.FromFile(FastBitmap.TestData1_GrayScale_KnownGood));
            Bitmap notExpected = new Bitmap(Image.FromFile(FastBitmap.TestData1_KnownGood));

            SUT.FastBitmap actual = new SUT.FastBitmap(new Bitmap(Image.FromFile(FastBitmap.TestData1_KnownGood)));

            actual.ToGrayscale();

            Assert.True(TestUtil.ContentsEqual(expected, actual.Buffer));
            Assert.False(TestUtil.ContentsEqual(notExpected, actual.Buffer));
        }

        [Fact]
        public void Grayscale_24BitColorDepth_ShouldSucceed()
        {
            Bitmap expected = new Bitmap(Image.FromFile(FastBitmap.TestData2_GrayScale_KnownGood));
            Bitmap notExpected = new Bitmap(Image.FromFile(FastBitmap.TestData2_KnownGood));

            SUT.FastBitmap actual = new SUT.FastBitmap(new Bitmap(Image.FromFile(FastBitmap.TestData2_KnownGood)));

            actual.ToGrayscale();

            Assert.True(TestUtil.ContentsEqual(expected, actual.Buffer));
            Assert.False(TestUtil.ContentsEqual(notExpected, actual.Buffer));
        }

        [Fact]
        public void Grayscale_8BitColorDepth_ShouldSucceed()
        {
            Bitmap expected = new Bitmap(Image.FromFile(FastBitmap.TestData3_GrayScale_KnownGood));
            Bitmap notExpected = new Bitmap(Image.FromFile(FastBitmap.TestData3_KnownGood));

            SUT.FastBitmap actual = new SUT.FastBitmap(new Bitmap(Image.FromFile(FastBitmap.TestData3_KnownGood)));

            actual.ToGrayscale();

            Assert.True(TestUtil.ContentsEqual(expected, actual.Buffer));
            Assert.False(TestUtil.ContentsEqual(notExpected, actual.Buffer));
        }
    }
}
