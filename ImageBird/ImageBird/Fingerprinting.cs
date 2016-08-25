using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageBird
{
    /// <summary>
    /// This class contains all functionality related to image fingerprinting.
    /// </summary>
    public class Fingerprinting
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

        /// <summary>
        /// This method performs image fingerprinting of the supplied Bitmap, using the specified mode.
        /// </summary>
        /// <param name="image">The Bitmap to fingerprint.</param>
        /// <param name="mode">The fingerprint mode to use.</param>
        /// <returns>The fingerprint of the supplied image, using the specified mode.</returns>
        public static string Fingerprint(Bitmap image, FingerprintMode mode)
        {
            string returnValue = null;
            switch (mode)
            {
                case FingerprintMode.PCASIFT:
                    returnValue = FingerprintPCASIFT(image);
                    break;
                case FingerprintMode.SIFT:
                    returnValue = FingerprintSIFT(image);
                    break;
                case FingerprintMode.SURF:
                    returnValue = FingerprintSURF(image);
                    break;
                case FingerprintMode.PHASH:
                    returnValue = FingerprintPHASH(image);
                    break;
                default:
                    throw new ArgumentException(
                        string.Format(
                            "Mode {0} not recognized.", 
                            mode), 
                        "mode");
            }

            if (returnValue == null)
            {
                throw new Exception("Could not fingerprint supplied image.");
            }
            return returnValue;
        }

        /// <summary>
        /// Performs fingerprinting on the supplied image using the PCA-SIFT algorithm.
        /// </summary>
        /// <param name="image">The image to fingerprint. Assumed to be non-null.</param>
        /// <returns>The fingerprint of the supplied image.</returns>
        private static string FingerprintPCASIFT(Bitmap image)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Performs fingerprinting on the supplied image using the SIFT algorithm.
        /// </summary>
        /// <param name="image">The image to fingerprint. Assumed to be non-null.</param>
        /// <returns>The fingerprint of the supplied image.</returns>
        private static string FingerprintSIFT(Bitmap image)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Performs fingerprinting on the supplied image using the SURF algorithm.
        /// </summary>
        /// <param name="image">The image to fingerprint. Assumed to be non-null.</param>
        /// <returns>The fingerprint of the supplied image.</returns>
        private static string FingerprintSURF(Bitmap image)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Performs fingerprinting on the supplied image using the PHASH algorithm.
        /// </summary>
        /// <param name="image">The image to fingerprint. Assumed to be non-null.</param>
        /// <returns>The fingerprint of the supplied image.</returns>
        private static string FingerprintPHASH(Bitmap image)
        {
            throw new NotImplementedException();
        }
    }
}
