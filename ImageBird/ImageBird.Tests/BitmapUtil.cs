using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using SUT = ImageBird.Images;

namespace ImageBird.Tests
{
    /// <summary>
    /// This class contains all tests for the ImageBird.BitmapUtil class.
    /// </summary>
    public class BitmapUtil
    {
        private readonly Bitmap testData1_Good = new Bitmap(@"../../Resources/TestData1_KnownGood.png");

        /// <summary>
        /// Test the ToGrayscale method with a valid (non-malformed) Bitmap object.
        /// </summary>
        [Fact]
        public void ToGrayscale_ValidImage_ShouldSucceed()
        {
            Bitmap expected = new Bitmap(@"../../Resources/TestData1_Grayscale_KnownGood.png");

            Bitmap actual = SUT.BitmapUtil.ToGrayscale(testData1_Good);
            
            for (int y = 0; y < expected.Height; y++)
            {
                for (int x = 0; x < expected.Width; x++)
                {
                    Assert.Equal(expected.GetPixel(x, y), actual.GetPixel(x, y));
                }
            }
        }
    }
}
