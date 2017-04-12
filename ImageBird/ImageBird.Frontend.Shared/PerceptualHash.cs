using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageBird.Core;

namespace ImageBird.Frontend.Shared
{
    /// <summary>
    /// Pairs a <see cref="FastBitmap"/> with a perceptual hash generated from it.
    /// </summary>
    public class PerceptualHash
    {
        /// <summary>
        /// The <see cref="Bitmap"/> this <see cref="PerceptualHash"/> references.
        /// </summary>
        public FastBitmap Content { get; }
    }
}
