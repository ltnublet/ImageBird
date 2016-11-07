using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageBird.Properties;

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
        /// Creates a FastBitmap from the specified file.
        /// </summary>
        /// <param name="path">
        /// The path to the file.
        /// </param>
        /// <returns>
        /// The FastBitmap created from the file.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Occurs when the supplied path is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Occurs when the supplied path does not point to a valid Bitmap.
        /// </exception>
        public static FastBitmap FromFile(string path)
        {
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            if (!File.Exists(path))
            {
                throw new ArgumentException(Resources.SpecifiedFileDidNotExist, nameof(path));
            }

            return new FastBitmap(new Bitmap(Image.FromFile(path)));
        }

        /// <summary>
        /// Performs a gaussian blur on the buffer using the specified sigma and weight.
        /// </summary>
        /// <param name="sigma">
        /// The factor by which to blur. Larger sigmas produce greater blurring.
        /// </param>
        /// <param name="weight">
        /// The size of the kernel. Larger weights produce greater blurring.
        /// </param>
        public void Blur(float sigma, int weight)
        {
            if (weight < 1 || weight % 2 == 0)
            {
                throw new ArgumentException(nameof(weight));
            }

            float[,] kernel = new float[weight, weight];
            float avg = weight / 2f;
            float sum = 0f;

            for (int x = 0; x < weight; x++)
            {
                for (int y = 0; y < weight; y++)
                {
                    kernel[x, y] =
                        (float)
                        Math.Exp(
                            -0.5f
                            * (Math.Pow(((float)(x)-avg) / sigma, 2f) + Math.Pow(((float)(y)-avg) / sigma, 2f)))
                        / (float)(2f * Math.PI * sigma * sigma);

                    sum += kernel[x, y];
                }
            }

            for (int x = 0; x < weight; x++)
            {
                for (int y = 0; y < weight; y++)
                {
                    kernel[x, y] /= sum;
                }
            }

            this.KernelOperation(kernel);
        }

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
            // TODO: Resolve whether checking the bpp has any impact - it looks like the Bitmap class
            // will read in bitmaps of arbitrary color depth, but expose them as having 32bpp. Since
            // we don't save the outputs, the behaviour is equivalent.
            ToGrayscaleOperation grayscale = null;
            switch (this.bitsPerPixel)
            {
                case 32:
                    grayscale = scan0 =>
                    {
                        int valR = *scan0;
                        int valG = *(scan0 + 1);
                        int valB = *(scan0 + 2);

                        byte avg = (byte)((float)(valR + valG + valB) / 3f);

                        *scan0 = avg;
                        *(scan0 + 1) = avg;
                        *(scan0 + 2) = avg;
                    };
                    break;
                default:
                    throw new NotImplementedException(Resources.UnsupportedImageColorDepth);
            }

            this.Operation((data, scan0) =>
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
        /// Applies the supplied kernel to the buffer.
        /// </summary>
        /// <param name="kernel">
        /// The kernel to apply. Assumed to be square and of odd dimensions.
        /// </param>
        protected unsafe void KernelOperation(float[,] kernel)
        {
            int dimension = (int)Math.Sqrt(kernel.Length);

            this.Operation((data, scan0) =>
            {
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

            try
            {
                operation(contents, scan0);
            }
            finally
            {
                this.Buffer.UnlockBits(contents);
            }
        }
    }
}
