using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
