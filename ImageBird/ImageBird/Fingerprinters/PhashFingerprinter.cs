using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageBird.Fingerprinters
{
    /// <summary>
    /// This class contains all functionality related to phash image fingerprinting.
    /// </summary>
    internal class PhashFingerprinter : IFingerprinter
    {
        /// <summary>
        /// Performs the fingerprinting using the supplied image.
        /// </summary>
        /// <param name="image">The image to fingerprint. Assumed non-null.</param>
        /// <returns>The fingerprint of the supplied image.</returns>
        public string Fingerprint(Bitmap image)
        {
            throw new NotImplementedException();
        }
    }
}
