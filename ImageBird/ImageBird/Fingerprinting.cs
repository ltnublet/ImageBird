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
        public string Fingerprint(Bitmap image, FingerprintMode mode)
        {
            string returnValue = null;
            switch (mode)
            {
                case FingerprintMode.PCASIFT:
                    returnValue = this.FingerprintPCASIFT(image);
                    break;
                case FingerprintMode.SIFT:
                    returnValue = this.FingerprintSIFT(image);
                    break;
                case FingerprintMode.SURF:
                    returnValue = this.FingerprintSURF(image);
                    break;
                case FingerprintMode.PHASH:
                    returnValue = this.FingerprintPHASH(image);
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
        /// <param name="image">The image to fingerprint.</param>
        /// <returns>The fingerprint of the supplied image.</returns>
        public string FingerprintPCASIFT(Bitmap image)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Performs fingerprinting on the supplied image using the SIFT algorithm.
        /// </summary>
        /// <param name="image">The image to fingerprint.</param>
        /// <returns>The fingerprint of the supplied image.</returns>
        public string FingerprintSIFT(Bitmap image)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Performs fingerprinting on the supplied image using the SURF algorithm.
        /// </summary>
        /// <param name="image">The image to fingerprint.</param>
        /// <returns>The fingerprint of the supplied image.</returns>
        public string FingerprintSURF(Bitmap image)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Performs fingerprinting on the supplied image using the PHASH algorithm.
        /// </summary>
        /// <param name="image">The image to fingerprint.</param>
        /// <returns>The fingerprint of the supplied image.</returns>
        public string FingerprintPHASH(Bitmap image)
        {
            throw new NotImplementedException();
        }
    }
}
