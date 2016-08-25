using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageBird.Fingerprinters;

namespace ImageBird
{
    /// <summary>
    /// This class contains all functionality related to image fingerprinting.
    /// </summary>
    public static class Fingerprinting
    {
        /// <summary>
        /// Fingerprinting algorithms available to callers.
        /// </summary>
        public enum FingerprintMode : int
        {
            PHASH = 0,
            SURF = 1,
            SIFT = 2,
            PCASIFT = 3
        }

        private static FingerprinterFactory Factory { get; } = new FingerprinterFactory();

        /// <summary>
        /// This method performs image fingerprinting of the supplied Bitmap, using the specified mode.
        /// </summary>
        /// <param name="image">The Bitmap to fingerprint.</param>
        /// <param name="mode">The fingerprint mode to use.</param>
        /// <returns>The fingerprint of the supplied image, using the specified mode.</returns>
        public static string Fingerprint(Bitmap image, FingerprintMode mode)
        {
            if (image == null)
            {
                throw new ArgumentNullException("image");
            }

            string returnValue = Factory.Fingerprint(image, mode);

            if (returnValue == null)
            {
                throw new Exception("Could not fingerprint supplied image.");
            }
            return returnValue;
        }
    }
}
