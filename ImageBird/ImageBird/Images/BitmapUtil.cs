using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cudafy;
using Cudafy.Host;
using Cudafy.Translator;

namespace ImageBird.Images
{
    /// <summary>
    /// Used for static helper methods relating to images.
    /// </summary>
    public static class BitmapUtil
    {
        /// <summary>
        /// Creates a grayscale version of the supplied bitmap.
        /// </summary>
        /// <param name="image">The bitmap to use as the input.</param>
        /// <returns>A grayscale version of the supplied bitmap.</returns>
        public static Bitmap ToGrayscale(this Bitmap image)
        {
            // TODO: Make sure that this is fast enough for fingerprinting needs.
            Bitmap returnValue = new Bitmap(image.Width, image.Height);
            using (Graphics graphics = Graphics.FromImage(returnValue))
            {
                ColorMatrix grayscaleTransform = new ColorMatrix(new float[][]
                {
                    new float[] { 0.299F, 0.299F, 0.299F, 0, 0 },
                    new float[] { 0.587F, 0.587F, 0.587F, 0, 0 },
                    new float[] { 0.114F, 0.114F, 0.114F, 0, 0 },
                    new float[] { 0, 0, 0, 1, 0 },
                    new float[] { 0, 0, 0, 0, 1 }
                });

                ImageAttributes attributes = new ImageAttributes();
                attributes.SetColorMatrix(grayscaleTransform);
                Rectangle result = new Rectangle(0, 0, image.Width, image.Height);
                graphics.DrawImage(
                    image,
                    result,
                    0,
                    0,
                    image.Width,
                    image.Height,
                    GraphicsUnit.Pixel,
                    attributes);
            }

            return returnValue;
        }

        /// <summary>
        /// Determines whether the specified Bitmap's RGB representations are equivalent.
        /// </summary>
        /// <param name="left">The first bitmap.</param>
        /// <param name="right">The second bitmap.</param>
        /// <returns>True if equivalent, and false otherwise.</returns>
        public static bool ContentsEqual(this Bitmap left, Bitmap right)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Performs a Deriche 0th-order blur operation. (Gaussian blur approximation).
        /// </summary>
        /// <param name="image">The image to blur.</param>
        /// <param name="sigma">The coefficient by which the image should be blurred.</param>
        /// <returns>The resulting blurred image.</returns>
        public static Bitmap Blur(this Bitmap image, double sigma)
        {
            throw new NotImplementedException();
        }
    }
}
