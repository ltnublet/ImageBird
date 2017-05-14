using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using SUT = ImageBird.Core2;

namespace ImageBird.Tests.Core2
{
    public class FastBitmap
    {
        private const string ResourcePath = @"..\..\Core\Resources\";

        private const string TestData1_KnownGood = ResourcePath + "TestData1_KnownGood.png";
        private const string TestData1_GrayScale_KnownGood = ResourcePath + "TestData1_GrayScale_KnownGood.png";
        private const string TestData1_BlurSigma1Weight5_KnownGood = ResourcePath + "TestData1_BlurSigma1Weight5_KnownGood.png";
        private const string TestData1_BlurSigma1Weight25_KnownGood = ResourcePath + "TestData1_BlurSigma1Weight25_KnownGood.png";
        private const string TestData1_BlurSigma1478Weight5_KnownGood = ResourcePath + "TestData1_BlurSigma1.478Weight5_KnownGood.png";

        private const string TestData2_KnownGood = ResourcePath + "TestData2_KnownGood.png";
        private const string TestData2_GrayScale_KnownGood = ResourcePath + "TestData2_GrayScale_KnownGood.png";

        private const string TestData3_KnownGood = ResourcePath + "TestData3_KnownGood.gif";
        private const string TestData3_GrayScale_KnownGood = ResourcePath + "TestData3_GrayScale_KnownGood.gif";

        [Theory]
        [InlineData(1D, 5, FastBitmap.TestData1_KnownGood, FastBitmap.TestData1_BlurSigma1Weight5_KnownGood)]
        public void Blur_ValidParams_ShouldSucceed(double sigma, int weight, string input, string expectedOutput)
        {
            using (Bitmap expected = (Bitmap)Image.FromFile(expectedOutput))
            {
                SUT.FastBitmap actual = SUT.FastBitmap.FromFile(input).Blur(sigma, weight);

                actual.Content.Save("ayy.png");
                
                TestUtil.AssertContentsEqual(expected, actual.Content);
            }
        }

        [Theory]
        [InlineData(FastBitmap.TestData1_GrayScale_KnownGood, FastBitmap.TestData1_KnownGood)]
        [InlineData(FastBitmap.TestData2_GrayScale_KnownGood, FastBitmap.TestData2_KnownGood)]
        [InlineData(FastBitmap.TestData3_GrayScale_KnownGood, FastBitmap.TestData3_KnownGood)]
        public void Grayscale_VaryingBitColorDepths_ShouldSucceed(string expectedFile, string actualFile)
        {
            using (Bitmap expected = new Bitmap(Image.FromFile(expectedFile)))
            {
                SUT.FastBitmap actual = SUT.FastBitmap.FromFile(actualFile);
                TestUtil.AssertContentsEqual(expected, actual.Grayscale().Content);
            }
        }
    }
}
