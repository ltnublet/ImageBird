using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageBird.Properties;

namespace ImageBird.Tests
{
    /// <summary>
    /// Shared utilities for tests.
    /// </summary>
    public static class TestUtil
    {
        /// <summary>
        /// Compares two Bitmap objects and checks if their contents are equal.
        /// </summary>
        /// <param name="left">
        /// The first Bitmap.
        /// </param>
        /// <param name="right">
        /// The second Bitmap.
        /// </param>
        /// <returns>
        /// True if the contents are equal, and false otherwise.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Occurs when either <paramref name="left"/> or <paramref name="right"/> are null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Occurs when either <paramref name="left"/> or <paramref name="right"/> are malformed.
        /// </exception>
        public static bool ContentsEqual(Bitmap left, Bitmap right)
        {
            if (left == null)
            {
                throw new ArgumentNullException(nameof(left));
            }

            if (right == null)
            {
                throw new ArgumentNullException(nameof(right));
            }

            if (left.Height == 0 || left.Width == 0)
            {
                throw new ArgumentException(Resources.SuppliedBitmapHasInvalidDimensions, nameof(left));
            }

            if (right.Height == 0 || right.Width == 0)
            {
                throw new ArgumentException(Resources.SuppliedBitmapHasInvalidDimensions, nameof(right));
            }

            if (left.PixelFormat != right.PixelFormat)
            {
                return false;
            }

            if (left.Height != right.Height || left.Width != right.Width)
            {
                return false;
            }

            for (int yPos = 0; yPos < left.Height; yPos++)
            {
                for (int xPos = 0; xPos < left.Width; xPos++)
                {
                    if (left.GetPixel(xPos, yPos) != right.GetPixel(xPos, yPos))
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}
