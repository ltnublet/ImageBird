using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageBird
{
    /// <summary>
    /// A Bitmap implementation with improved performance.
    /// </summary>
    public class FastBitmap : IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FastBitmap"/> class.
        /// </summary>
        /// <param name="bitmap">
        /// The bitmap to convert to a FastBitmap.
        /// </param>
        public unsafe FastBitmap(Bitmap bitmap)
        {
            if (bitmap == null)
            {
                throw new ArgumentNullException(nameof(bitmap));
            }

            if (bitmap.Height == 0 || bitmap.Width == 0)
            {
                throw new ArgumentException("Supplied bitmap has invalid dimensions.", nameof(bitmap));
            }

            this.Buffer = (Bitmap)bitmap.Clone();
        }

        /// <summary>
        /// Used to perform an action on a pixel.
        /// </summary>
        /// <param name="pointer">
        /// A pointer to the first address of the pixel's data.
        /// </param>
        /// <param name="bitsPerPixel">
        /// The number of bits per pixel.
        /// </param>
        protected unsafe delegate void PixelAction(byte* pointer, int bitsPerPixel);

        /// <summary>
        /// Bitmap to which specified changes will be applied.
        /// </summary>
        public Bitmap Buffer { get; protected set; }

        /// <summary>
        /// Disposes the FastBitmap, freeing it's resources.
        /// </summary>
        public void Dispose()
        {
            this.Buffer.Dispose();
        }

        /// <summary>
        /// Performs the supplied operation on each pixel in the buffer.
        /// </summary>
        /// <param name="operation">
        /// The operation to perform on each pixel.
        /// </param>
        protected unsafe void Operation(PixelAction operation)
        {
            BitmapData contents = this.Buffer.LockBits(
                new Rectangle(0, 0, this.Buffer.Width, this.Buffer.Height),
                ImageLockMode.ReadOnly,
                this.Buffer.PixelFormat);

            int bitsPerPixel = Image.GetPixelFormatSize(this.Buffer.PixelFormat);

            byte* scan0 = (byte*)contents.Scan0.ToPointer();

            for (int yPos = 0; yPos < contents.Height; yPos++)
            {
                for (int xPos = 0; xPos < contents.Width; xPos++)
                {
                    byte* data = scan0 + (yPos * contents.Stride) + ((xPos * bitsPerPixel) / 8);

                    operation(data, bitsPerPixel);
                }
            }

            this.Buffer.UnlockBits(contents);
        }
    }
}
