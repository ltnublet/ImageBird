using System;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using ImageBird.Properties;

namespace ImageBird
{
    /// <summary>
    /// A Bitmap implementation with improved performance.
    /// </summary>
    [SuppressMessage(
        "Microsoft.StyleCop.CSharp.SpacingRules", 
        "SA1003:SymbolsMustBeSpacedCorrectly", 
        Justification = "Disabled due to unsafe code incorrectly triggering this rule. StyleCop thinks the pointer dereferencing asterisks are multiplication.")]
    [SuppressMessage(
        "Microsoft.StyleCop.CSharp.SpacingRules", 
        "SA1009:ClosingParenthesisMustBeSpacedCorrectly", 
        Justification = "Disabled due to unsafe code incorrectly triggering this rule. StyleCop thinks the pointer dereferencing asterisks are multiplication.")]
    [SuppressMessage(
        "Microsoft.StyleCop.CSharp.MaintainabilityRules", 
        "SA1119:StatementMustNotUseUnnecessaryParenthesis", 
        Justification = "Disabled due to heavy use of math in this file. Parenthesis are to clearly indicate to the reader the order of operations.")]
    public class FastBitmap : IDisposable
    {
        /// <summary>
        /// We cache the bits per pixel on instantiation - it's inordinately expensive to compute, and won't change for the lifetime of the FastBitmap instance.
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
            
            this.Content = (Bitmap)bitmap.Clone();
            this.bitsPerPixel = Image.GetPixelFormatSize(this.Content.PixelFormat);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FastBitmap"/> class. Does not check supplied parameters for validity.
        /// </summary>
        /// <param name="bitmap">
        /// The <see cref="Bitmap"/> to convert to a <see cref="FastBitmap"/>.
        /// </param>
        /// <param name="bitsPerPixel">
        /// The bits per pixel of the supplied bitmap.
        /// </param>
        protected unsafe FastBitmap(Bitmap bitmap, int bitsPerPixel)
        {
            this.Content = (Bitmap)bitmap.Clone();
            this.bitsPerPixel = bitsPerPixel;
        }

        /// <summary>
        /// Performs a locking data operation on the supplied <see cref="BitmapData"/> object.
        /// </summary>
        /// <param name="data">
        /// The BitmapData.
        /// </param>
        /// <param name="scan0">
        /// The base address.
        /// </param>
        protected unsafe delegate void LockingDataOperation(BitmapData data, byte* scan0);

        private unsafe delegate void ToGrayscaleOperation(byte* scan0, int yPos, int stride, int width);

        /// <summary>
        /// The contents of the current <see cref="FastBitmap"/>.
        /// </summary>
        public Bitmap Content { get; protected set; }

        /// <summary>
        /// Creates a <see cref="FastBitmap"/> from the specified file.
        /// </summary>
        /// <param name="path">
        /// The path to the file.
        /// </param>
        /// <returns>
        /// The <see cref="FastBitmap"/> created from the file.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Occurs when the supplied path is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Occurs when the supplied path does not point to a valid <see cref="Bitmap"/>.
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
        /// Performs a gaussian blur on the image using the specified <paramref name="sigma"/> and <paramref name="weight"/>.
        /// </summary>
        /// <param name="sigma">
        /// The factor by which to blur. Larger sigmas produce greater blurring.
        /// </param>
        /// <param name="weight">
        /// The size of the kernel. Larger weights produce greater blurring.
        /// </param>
        public FastBitmap Blur(double sigma, int weight)
        {
            return this.KernelOperation(Kernel.Gaussian(sigma, weight));
        }

        /// <summary>
        /// Disposes the <see cref="FastBitmap"/>, freeing it's resources.
        /// </summary>
        public void Dispose()
        {
            this.Content.Dispose();
        }

        /// <summary>
        /// Returns the largest magnitude in the FastBitmap.
        /// </summary>
        /// <returns>
        /// The largest magnitude in the FastBitmap.
        /// </returns>
        public unsafe ushort Max()
        {
            object theLock = new object();
            ushort returnValue = 0;

            using (FastBitmap asGrayscale = this.ToGrayscale())
            {
                int bpp = asGrayscale.bitsPerPixel;

                FastBitmap.Operation(asGrayscale.Content, (data, scan0) =>
                {
                    Parallel.For(0, data.Height, yPos =>
                    {
                        byte localMax = 0;

                        for (int xPos = 0; xPos < data.Width; xPos++)
                        {
                            byte current = *FastBitmap.PixelPointer(scan0, xPos, yPos, data.Stride, bpp);

                            if (current > localMax)
                            {
                                localMax = current;
                            }
                        }

                        lock (theLock)
                        {
                            if (localMax > returnValue)
                            {
                                returnValue = localMax;
                            }
                        }
                    });
                });
            }

            return returnValue;
        }

        /// <summary>
        /// Raises the value of each pixel in the <see cref="FastBitmap"/>'s channels to the supplied <paramref name="power"/>. Channel values are capped at 255.
        /// </summary>
        /// <param name="power">
        /// The power to raise each pixel's channel to.
        /// </param>
        /// <returns>
        /// A <see cref="FastBitmap"/> whose pixels are equal to the original's to the supplied <paramref name="power"/>.
        /// </returns>
        public unsafe FastBitmap Pow(ushort power)
        {
            FastBitmap buffer = new FastBitmap(this.Content, this.bitsPerPixel);

            int bufferHeight = buffer.Content.Height;
            int bufferWidth = buffer.Content.Width;

            FastBitmap.Operation(buffer.Content, (data, scan0) =>
            {
                Parallel.For(0, bufferHeight, yPos =>
                {
                    int localWidth = bufferWidth;

                    Parallel.For(
                        0, 
                        localWidth, 
                        xPos =>
                        {
                            byte* valR = FastBitmap.PixelPointer(scan0, xPos, yPos, data.Stride, buffer.bitsPerPixel);
                            byte* valG = valR + 1;
                            byte* valB = valR + 2;

                            int newR = (int)Math.Pow(*valR, power);
                            int newG = (int)Math.Pow(*valG, power);
                            int newB = (int)Math.Pow(*valB, power);

                            *valR = (byte)(newR > 255 ? 255 : newR);
                            *valG = (byte)(newG > 255 ? 255 : newG);
                            *valB = (byte)(newB > 255 ? 255 : newB);
                        });
                });
            });

            return buffer;
        }

        /// <summary>
        /// Performs a Radon transformation on the <see cref="FastBitmap"/>.
        /// </summary>
        /// <param name="numberOfAngles">
        /// The number of angles the resulting <see cref="ProjectionResult"/> should contain.
        /// </param>
        /// <returns>
        /// A <see cref="ProjectionResult"/> representing the produced transform.
        /// </returns>
        public unsafe ProjectionResult RadonTransform(int numberOfAngles)
        {
            if (numberOfAngles <= 0)
            {
                throw new ArgumentException(Resources.InvalidNumberOfAnglesSpecified, nameof(numberOfAngles));
            }

            FastBitmap buffer = new FastBitmap(new Bitmap(this.Content.Width, this.Content.Height));

            using (Graphics graphics = Graphics.FromImage(buffer.Content))
            {
                graphics.FillRectangle(Brushes.Black, 0, 0, buffer.Content.Width, buffer.Content.Height);
            }

            int[] pixelsPerLine = new int[numberOfAngles];

            int height = buffer.Content.Height;
            int width = buffer.Content.Width;

            int diff = Math.Max(height, width);

            double xCenter = (double)width / 2f;
            double yCenter = (double)height / 2f;
            int xOffset = (int)(xCenter + FastBitmap.RoundingFactor(xCenter));
            int yOffset = (int)(yCenter + FastBitmap.RoundingFactor(yCenter));

            FastBitmap.Operation(this.Content, (data, scan0) =>
            {
                FastBitmap.Operation(buffer.Content, (subData, subScan0) =>
                {
                    for (int k = 0; k < (numberOfAngles / 4) + 1; k++)
                    {
                        double theta = k * Math.PI / numberOfAngles;
                        double alpha = Math.Tan(theta);

                        for (int x = 0; x < diff; x++)
                        {
                            double y = alpha * (x - xOffset);
                            int yd = (int)(y + FastBitmap.RoundingFactor(y));
                            if ((yd + yOffset >= 0)
                                && (yd + yOffset < height)
                                && (x < width))
                            {
                                // Originally: *ptr_radon_map->data(k, x) = img(x, yd + yOffset);
                                *FastBitmap.PixelPointer(subScan0, k, x, subData.Stride, buffer.bitsPerPixel) 
                                    = *FastBitmap.PixelPointer(scan0, x, yd + yOffset, data.Stride, this.bitsPerPixel);
                                
                                pixelsPerLine[k]++;
                            }

                            if ((yd + xOffset >= 0)
                                && (yd + xOffset < width)
                                && (k != numberOfAngles / 4)
                                && (x < height))
                            {
                                // Originally: *ptr_radon_map->data(numberOfAngles / 2 - k, x) = img(yd + xOffset, x);
                                *FastBitmap.PixelPointer(
                                    subScan0, 
                                    (numberOfAngles / 2) - k, 
                                    x, 
                                    subData.Stride, 
                                    buffer.bitsPerPixel)
                                    = *FastBitmap.PixelPointer(scan0, yd + xOffset, x, data.Stride, this.bitsPerPixel);

                                pixelsPerLine[(numberOfAngles / 2) - k]++;
                            }
                        }
                    }

                    int j = 0;
                    for (int k = 3 * numberOfAngles / 4; k < numberOfAngles; k++)
                    {
                        double theta = k * Math.PI / numberOfAngles;
                        double alpha = Math.Tan(theta);
                        for (int x = 0; x < diff; x++)
                        {
                            double y = alpha * (x - xOffset);
                            int yd = (int)(y + FastBitmap.RoundingFactor(y));
                            if ((yd + yOffset >= 0) && (yd + yOffset < height) && (x < width))
                            {
                                // Originally: *ptr_radon_map->data(k, x) = img(x, yd + yOffset);
                                *FastBitmap.PixelPointer(subScan0, k, x, subData.Stride, buffer.bitsPerPixel) =
                                    *FastBitmap.PixelPointer(scan0, x, yd + yOffset, data.Stride, this.bitsPerPixel);

                                pixelsPerLine[k]++;
                            }

                            if ((yOffset - yd >= 0) 
                                && (yOffset - yd < width) 
                                && ((2 * yOffset) - x >= 0) 
                                && ((2 * yOffset) - x < height) 
                                && (k != (3 * numberOfAngles) / 4))
                            {
                                // Originally: *ptr_radon_map->data(k - j, x) = img(-yd + yOffset, -(x - yOffset) + yOffset);
                                *FastBitmap.PixelPointer(subScan0, k - j, x, subData.Stride, buffer.bitsPerPixel) =
                                    *FastBitmap.PixelPointer(
                                        scan0,
                                        -yd + yOffset,
                                        -(x - yOffset) + yOffset,
                                        data.Stride,
                                        this.bitsPerPixel);

                                pixelsPerLine[k - j]++;
                            }
                        }

                        j += 2;
                    }
                });
            });

            return new ProjectionResult(buffer, pixelsPerLine);
        }

        /// <summary>
        /// Iterates over the <see cref="FastBitmap"/>, scaling the magnitude of each pixel by the supplied <paramref name="factor"/>. Channel values are capped at 255.
        /// </summary>
        /// <param name="factor">
        /// The factor by which to scale the magnitude of each pixel.
        /// </param>
        /// <returns>
        /// A <see cref="FastBitmap"/> which has been scaled by the supplied <paramref name="factor"/>.
        /// </returns>
        /// <example>
        /// ExampleFastBitmap.ScaleBy(255) results in no scaling.
        /// ExampleFastBitmap.ScaleBy(128) doubles all channels.
        /// ExampleFastBitmap.ScaleBy(64) quadruples all channels.
        /// </example>
        public unsafe FastBitmap ScaleBy(ushort factor)
        {
            FastBitmap buffer = new FastBitmap(this.Content, this.bitsPerPixel);

            double actualFactor = (255D / (double)factor);

            int bufferHeight = buffer.Content.Height;
            int bufferWidth = buffer.Content.Width;
            
            FastBitmap.Operation(buffer.Content, (data, scan0) =>
            {
                Parallel.For(0, bufferHeight, yPos =>
                {
                    int localWidth = bufferWidth;

                    Parallel.For(0, localWidth, xPos =>
                    {
                        byte* valR = FastBitmap.PixelPointer(scan0, xPos, yPos, data.Stride, buffer.bitsPerPixel);
                        byte* valG = valR + 1;
                        byte* valB = valR + 2;

                        int newR = (int)((double)*valR * actualFactor);
                        int newG = (int)((double)*valG * actualFactor);
                        int newB = (int)((double)*valB * actualFactor);

                        *valR = (byte)(newR > 255 ? 255 : newR);
                        *valG = (byte)(newG > 255 ? 255 : newG);
                        *valB = (byte)(newB > 255 ? 255 : newB);
                    });
                });
            });

            return buffer;
        }

        /// <summary>
        /// Converts the <see cref="FastBitmap"/> to grayscale.
        /// </summary>
        /// <returns>
        /// A <see cref="FastBitmap"/> whose contents are the same as the current <see cref="FastBitmap"/>, but in grayscale.
        /// </returns>
        public unsafe FastBitmap ToGrayscale()
        {
            FastBitmap buffer = new FastBitmap(this.Content, this.bitsPerPixel);

            // TODO: Resolve whether checking the bpp has any impact - it looks like the Bitmap class
            // will read in bitmaps of arbitrary color depth, but expose them as having 32bpp. Since
            // we don't save the outputs, the behaviour is equivalent.
            ToGrayscaleOperation grayscale = null;
            switch (buffer.bitsPerPixel)
            {
                case 32:
                    grayscale = (scan0, yPos, stride, width) =>
                    {
                        for (int xPos = 0; xPos < width; xPos++)
                        {
                            byte* pixel = FastBitmap.PixelPointer(scan0, xPos, yPos, stride, buffer.bitsPerPixel);

                            int valR = *pixel;
                            int valG = *(pixel + 1);
                            int valB = *(pixel + 2);

                            byte avg = (byte)((double)(valR + valG + valB) / 3D);

                            *pixel = avg;
                            *(pixel + 1) = avg;
                            *(pixel + 2) = avg;
                        }
                    };
                    break;
                default:
                    throw new NotImplementedException(Resources.UnsupportedImageColorDepth);
            }

            int bufferHeight = buffer.Content.Height;
            int bufferWidth = buffer.Content.Width;

            FastBitmap.Operation(buffer.Content, (data, scan0) =>
            {
                Parallel.For(0, bufferHeight, yPos =>
                {
                    int localWidth = bufferWidth;
                    grayscale(scan0, yPos, data.Stride, localWidth);
                });
            });

            return buffer;
        }

        /// <summary>
        /// Performs the supplied <paramref name="operation"/> on the supplied <see cref="Bitmap"/> <paramref name="bitmap"/>.
        /// </summary>
        /// <param name="bitmap">
        /// The <see cref="Bitmap"/> to perform the operation on.
        /// </param>
        /// <param name="operation">
        /// The operation to perform on the contents of the supplied <see cref="Bitmap"/>.
        /// </param>
        protected static unsafe void Operation(Bitmap bitmap, LockingDataOperation operation)
        {
            BitmapData contents = bitmap.LockBits(
                new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                ImageLockMode.ReadOnly,
                bitmap.PixelFormat);

            try
            {
                operation(contents, (byte*)contents.Scan0.ToPointer());
            }
            finally
            {
                bitmap.UnlockBits(contents);
            }
        }

        /// <summary>
        /// Applies the supplied <paramref name="kernel"/> to the <see cref="FastBitmap"/>.
        /// </summary>
        /// <param name="kernel">
        /// The <see cref="Kernel"/> to apply. Assumed to be of odd dimensions.
        /// </param>
        /// <returns>
        /// A <see cref="FastBitmap"/> whose contents are the current <see cref="FastBitmap"/>'s contents convolved with the <paramref name="kernel"/>.
        /// </returns>
        protected unsafe FastBitmap KernelOperation(Kernel kernel)
        {
            FastBitmap buffer = new FastBitmap(this.Content, this.bitsPerPixel);

            Rectangle cropRect = new Rectangle(
                kernel.Center,
                kernel.Center,
                buffer.Content.Width - (kernel.Center * 2),
                buffer.Content.Height - (kernel.Center * 2));

            Bitmap subBuffer = new Bitmap(cropRect.Width, cropRect.Height);

            using (Graphics graphics = Graphics.FromImage(subBuffer))
            {
                graphics.DrawImage(
                    buffer.Content,
                    new Rectangle(0, 0, subBuffer.Width, subBuffer.Height),
                    cropRect,
                    GraphicsUnit.Pixel);
            }

            FastBitmap.Operation(subBuffer, (subData, subScan0) =>
            {
                FastBitmap.Operation(buffer.Content, (data, scan0) =>
                {
                    int localWidth = subBuffer.Width;

                    Parallel.For(0, subBuffer.Height, yPos =>
                    {
                        Parallel.For(0, localWidth, xPos =>
                        {
                            byte* current = FastBitmap.PixelPointer(
                                subScan0,
                                xPos,
                                yPos,
                                subData.Stride,
                                buffer.bitsPerPixel);

                            double newR = 0D;
                            double newG = 0D;
                            double newB = 0D;

                            for (int localY = -kernel.Center; localY <= kernel.Center; localY++)
                            {
                                for (int localX = -kernel.Center; localX <= kernel.Center; localX++)
                                {
                                    double scaling = kernel.Contents[localY + kernel.Center, localX + kernel.Center];

                                    byte* local = FastBitmap.PixelPointer(
                                        scan0,
                                        xPos + kernel.Center + localX,
                                        yPos + kernel.Center + localY,
                                        data.Stride,
                                        buffer.bitsPerPixel);

                                    newR += ((double)(*local)) * scaling;
                                    newG += ((double)(*(local + 1))) * scaling;
                                    newB += ((double)(*(local + 2))) * scaling;
                                }
                            }

                            *current = (byte)(int)newR;
                            *(current + 1) = (byte)(int)newG;
                            *(current + 2) = (byte)(int)newB;
                        });
                    });
                });
            });

            buffer.Content.Dispose();
            buffer.Content = subBuffer;

            return buffer;
        }

        /// <summary>
        /// Computes the rounding factor for the supplied value. Aggressively inlined for performance.
        /// </summary>
        /// <param name="value">
        /// The value to compute the rounding factor for.
        /// </param>
        /// <returns>
        /// The rounding factor associated with the <paramref name="value"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static double RoundingFactor(double value)
        {
            return value >= 0 ? 0.5D : -0.5D;
        }

        /// <summary>
        /// Computes the pointer to a pixel given by the supplied parameters. Aggressively inlined for performance. Performs no validation - make sure the values supplied are accurate, as there's no bounds checking.
        /// </summary>
        /// <param name="scan0">
        /// The base pointer for the Bitmap data.
        /// </param>
        /// <param name="xPos">
        /// The desired x coordinate.
        /// </param>
        /// <param name="yPos">
        /// The desired y coordinate.
        /// </param>
        /// <param name="stride">
        /// The <see cref="Bitmap"/> stride (size of one horizontal line of the image).
        /// </param>
        /// <param name="bpp">
        /// The bits per pixel of the Bitmap.
        /// </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static unsafe byte* PixelPointer(byte* scan0, int xPos, int yPos, int stride, int bpp)
        {
            return scan0 + (yPos * stride) + ((xPos * bpp) / 8);
        }

        /// <summary>
        /// Holds the results of a Radon/Hough transformation. Essentially a tuple with named fields.
        /// </summary>
        public class ProjectionResult
        {
            /// <summary>
            /// Simple constructor. No validation is performed on the supplied arguments.
            /// </summary>
            /// <param name="transform">
            /// The FastBitmap resulting from the transformation.
            /// </param>
            /// <param name="pixelsPerLine">
            /// The number of pixels per line.
            /// </param>
            public ProjectionResult(FastBitmap transform, int[] pixelsPerLine)
            {
                this.Transform = transform;
                this.PixelsPerLine = pixelsPerLine;
            }

            /// <summary>
            /// The resultant Radon/Hough transformation.
            /// </summary>
            public FastBitmap Transform { get; }

            /// <summary>
            /// The number of pixels per line.
            /// </summary>
            public int[] PixelsPerLine { get; }
        }
    }
}
