using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageBird.Fingerprinters
{
    /// <summary>
    /// This class handles hiding the underlaying fingerprinting algorithm selection mechanism from callers.
    /// </summary>
    public class FingerprinterFactory
    {
        private Dictionary<Fingerprinting.FingerprintMode, IFingerprinter> algorithms;

        /// <summary>
        /// Instantiates a FingerprinterFactory using the supplied allowed algorithm dictionary. If
        /// no supported algorithm dictionary is supplied, the default supported algorithms are used.
        /// </summary>
        /// <param name="supportedAlgorithms">
        /// A dictionary containing the allowed algorithms. If null, the default algorithms 
        /// { PCASIFT, SIFT, SURF, PHASH } are used.
        /// </param>
        public FingerprinterFactory(
            Dictionary<Fingerprinting.FingerprintMode, IFingerprinter> allowedAlgorithms = null)
        {
            this.algorithms = allowedAlgorithms ?? DefaultAlgorithms;
        }

        private static Dictionary<Fingerprinting.FingerprintMode, IFingerprinter> 
            DefaultAlgorithms { get; } = new Dictionary<Fingerprinting.FingerprintMode, IFingerprinter>
            {
                { Fingerprinting.FingerprintMode.PCASIFT, new PcaSiftFingerprinter() },
                { Fingerprinting.FingerprintMode.SIFT, new SiftFingerprinter() },
                { Fingerprinting.FingerprintMode.SURF, new SurfFingerprinter() },
                { Fingerprinting.FingerprintMode.PHASH, new PhashFingerprinter() }
            };
        
        /// <summary>
        /// Performs fingerprinting for the supplied image and mode.
        /// </summary>
        /// <param name="image">The image to fingerprint.</param>
        /// <param name="mode">The algorithm with which to perform the fingerprinting.</param>
        /// <returns>The fingerprint of the image.</returns>
        public string Fingerprint(Bitmap image, Fingerprinting.FingerprintMode mode)
        {
            if (image == null)
            {
                throw new ArgumentNullException("image");
            }

            if (!this.algorithms.ContainsKey(mode))
            {
                throw new ArgumentException(
                    string.Format(
                        "The specified mode {0} was not found in the supported algorithm dictionary.", 
                        mode),
                    "mode");
            }
            
            return this.algorithms[mode].Fingerprint(image);
        }
    }
}
