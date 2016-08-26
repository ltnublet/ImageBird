using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageBird.Images
{
    /// <summary>
    /// System.Drawing.Bitmap wrapper containing additional state information.
    /// </summary>
    public class Picture
    {
        private Bitmap cachedGrayscale;
        
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="image">The Bitmap to wrap.</param>
        public Picture(Bitmap image)
        {
            this.Image = image;
            this.cachedGrayscale = this.Image.ToGrayscale();
            this.IsGrayscale = this.Image.ContentsEqual(this.cachedGrayscale);
        }

        /// <summary>
        /// The wrapped Bitmap.
        /// </summary>
        public Bitmap Image { get; protected set; }

        /// <summary>
        /// True if the wrapped Bitmap is grayscale, and false otherwise.
        /// </summary>
        public bool IsGrayscale { get; protected set; }
    }
}
