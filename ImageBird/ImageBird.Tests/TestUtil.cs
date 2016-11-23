using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageBird.Properties;
using SUT = ImageBird;

namespace ImageBird.Tests
{
    using global::System.Collections;

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
#pragma warning disable IDE0018 // Inline variable declaration
            Point? outBuffer;
#pragma warning restore IDE0018 // Inline variable declaration
            return TestUtil.ContentsEqual(left, right, out outBuffer);
        }

        /// <summary>
        /// Compares two Bitmap objects and checks if their contents are equal, outputting the indices at which the
        /// mismatch occurred (or null if indices do not apply to the situation).
        /// </summary>
        /// <param name="left">
        /// The first Bitmap.
        /// </param>
        /// <param name="right">
        /// The second Bitmap.
        /// </param>
        /// <param name="mismatch">
        /// The indices at which the mismatch was detected (or null if indices do not apply to the situation).
        /// </param>
        /// <returns>
        /// True if the contents are equal, and false otherwise.
        /// </returns>
        public static bool ContentsEqual(Bitmap left, Bitmap right, out Point? mismatch)
        {
            mismatch = null;

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
                    Color leftPixel = left.GetPixel(xPos, yPos);
                    Color rightPixel = right.GetPixel(xPos, yPos);

                    if (!leftPixel.Equals(rightPixel))
                    {
                        mismatch = new Point(xPos, yPos);
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Checks whether the supplied kernel matches the supplied expected values up to a given resolution.
        /// </summary>
        /// <param name="expected">The expected value of the kernel.</param>
        /// <param name="actual">The actual kernel.</param>
        /// <param name="truncateAt">
        /// The number of decimal places to check to (for example, a truncateAt of 1 would mean to check only the first
        /// digit of the supplied values for equivalency).
        /// </param>
        /// <returns></returns>
        public static bool TruncatedContentsEqual(double[,] expected, SUT.Kernel actual, int truncateAt)
        {
            throw new NotImplementedException("This probably works, but hasn't been checked by a test.");

            if (expected == null)
            {
                throw new ArgumentNullException(nameof(expected));
            }

            if (actual == null)
            {
                throw new ArgumentNullException(nameof(actual));
            }

            if (expected.Length != actual.Contents.Length)
            {
                return false;
            }
            
            decimal calculateTruncation = 1m;
            for (; truncateAt != 0; truncateAt--)
            {
                calculateTruncation /= 10m;
            }

            double truncateBy = (double)calculateTruncation;

            return !expected.Cast<double>()
                .Select(x => x - (x % truncateBy))
                .Except(actual.Contents.Cast<double>().Select(y => y - (y % truncateBy)))
                .Any();
        }
    }
}
