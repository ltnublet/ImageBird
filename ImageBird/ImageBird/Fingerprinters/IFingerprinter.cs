using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageBird.Fingerprinters
{
    /// <summary>
    /// This interface describes the minimum publicly accessible methods for Fingerprinters.
    /// </summary>
    public interface IFingerprinter
    {
        /// <summary>
        /// This method should perform the fingerprinting, throwing any encountered exceptions.
        /// </summary>
        /// <param name="image">The image to fingerprint. Assumed non-null.</param>
        /// <returns>The fingerprint of the supplied image.</returns>
        string Fingerprint(Bitmap image);
    }
}
