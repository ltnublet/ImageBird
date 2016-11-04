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
        /// We cache the bits per pixel on instantiation - it's inordinately expensive to compute, and won't
        /// change for the lifetime of the FastBitmap instance.
        /// </summary>
        private readonly int bitsPerPixel;

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
                throw new ArgumentException(Properties.Resources.SuppliedBitmapHasInvalidDimensions, nameof(bitmap));
            }

            this.Buffer = (Bitmap)bitmap.Clone();
            this.Original = (Bitmap)bitmap.Clone();
            this.bitsPerPixel = Image.GetPixelFormatSize(this.Buffer.PixelFormat);
        }

        /// <summary>
        /// Performs a locking data operation on the supplied BitmapData object.
        /// </summary>
        /// <param name="data">
        /// The BitmapData.
        /// </param>
        /// <param name="bitsPerPixel">
        /// The bits per pixel of the bitmap.
        /// </param>
        /// <param name="scan0">
        /// The base address.
        /// </param>
        protected unsafe delegate void LockingDataOperation(BitmapData data, byte* scan0);
        
        private unsafe delegate void ToGrayscaleOperation(byte* scan0);

        /// <summary>
        /// Bitmap to which specified changes will be applied.
        /// </summary>
        public Bitmap Buffer { get; protected set; }

        /// <summary>
        /// A clone of the original bitmap supplied at instantiation.
        /// </summary>
        public Bitmap Original { get; protected set; }
        
        /// <summary>
        /// Disposes the FastBitmap, freeing it's resources.
        /// </summary>
        public void Dispose()
        {
            this.Buffer.Dispose();
        }

        /// <summary>
        /// Converts the Buffer to grayscale.
        /// </summary>
        public unsafe void ToGrayscale()
        {
            throw new NotImplementedException("Need ToGrayscaleOperations for common color depths.");

            ToGrayscaleOperation grayscale = scan0 =>
                {
                    int valR = *scan0;
                    int valG = *(scan0 + 1);
                    int valB = *(scan0 + 2);

                    byte avg = (byte)((float)(valR + valG + valB) / 3f);

                    *scan0 = avg;
                    *(scan0 + 1) = avg;
                    *(scan0 + 2) = avg;
                };

            this.Operation(delegate(BitmapData data, byte* scan0)
            {
                for (int yPos = 0; yPos < data.Height; ++yPos)
                {
                    for (int xPos = 0; xPos < data.Width; ++xPos)
                    {
                        byte* pixel = scan0 + (yPos * data.Stride) + ((xPos * this.bitsPerPixel) / 8);

                        grayscale(pixel);
                    }
                }
            });
        }

        /// <summary>
        /// Performs the supplied operation on each pixel in the buffer.
        /// </summary>
        /// <param name="operation">
        /// The operation to perform on each pixel.
        /// </param>
        protected unsafe void Operation(LockingDataOperation operation)
        {
            BitmapData contents = this.Buffer.LockBits(
                new Rectangle(0, 0, this.Buffer.Width, this.Buffer.Height),
                ImageLockMode.ReadOnly,
                this.Buffer.PixelFormat);

            byte* scan0 = (byte*)contents.Scan0.ToPointer();

            operation(contents, scan0);

            this.Buffer.UnlockBits(contents);
        }
    }
}
