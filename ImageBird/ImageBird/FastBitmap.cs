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
    using global::System.Diagnostics.CodeAnalysis;

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
        public void Blur(double sigma, int weight)
        {
            this.KernelOperation(Kernel.Gaussian(sigma, weight));
        }

        /// <summary>
        /// Disposes the FastBitmap, freeing it's resources.
        /// </summary>
        public void Dispose()
        {
            this.Original.Dispose();
            this.Buffer.Dispose();
        }

        /// <summary>
        /// Iterates over the Buffer, scaling the magnitude of each pixel by the supplied factor. For example, 255 is
        /// no scaling, 128 doubles all channels, 64 quadruples all channels, etc.
        /// </summary>
        /// <param name="factor">
        /// The factor by which to scale the magnitude of each pixel.
        /// </param>
        [SuppressMessage("StyleCop.CSharp.SpacingRules", "SA1003:SymbolsMustBeSpacedCorrectly",
            Justification = "Spacing is valid - the asterisk isn't multiplying, but rather dereferencing.")]
        [SuppressMessage("StyleCop.CSharp.SpacingRules", "SA1009:ClosingParenthesisMustBeSpacedCorrectly",
            Justification = "Spacing is valid - parenthesis due to dereferencing and casting.")]
        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1115:ParameterMustFollowComma",
             Justification = "Unecessary newlines reduce readability due to the high nesting of scopes.")]
        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1116:SplitParametersMustStartOnLineAfterDeclaration",
             Justification = "Unecessary newlines reduce readability due to the high nesting of scopes.")]
        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1117:ParametersMustBeOnSameLineOrSeparateLines",
             Justification = "Unecessary newlines reduce readability due to the high nesting of scopes.")]
        [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1119:StatementMustNotUseUnnecessaryParenthesis", 
            Justification = "Parenthesis are necessary due to casting of incremented pointer.")]
        public unsafe void ScaleBy(ushort factor)
        {
            double actualFactor = (255D / (double)factor);

            int bufferHeight = this.Buffer.Height;
            int bufferWidth = this.Buffer.Width;
            
            FastBitmap.Operation(this.Buffer, (data, scan0) =>
            {
                Parallel.For(0, bufferHeight, yPos =>
                {
                    int localWidth = bufferWidth;

                    Parallel.For(0, localWidth, xPos =>
                    {
                        byte* valR =
                            scan0 +
                            (yPos * data.Stride) +
                            ((xPos * this.bitsPerPixel) / 8);
                        byte* valG = (valR + 1);
                        byte* valB = (valR + 2);

                        int newR = (int)((double)*valR * actualFactor);
                        int newG = (int)((double)*valG * actualFactor);
                        int newB = (int)((double)*valB * actualFactor);

                        *valR = (byte)(newR > 255 ? 255 : newR);
                        *valG = (byte)(newG > 255 ? 255 : newG);
                        *valB = (byte)(newB > 255 ? 255 : newB);
                    });
                });
            });
        }

        /// <summary>
        /// Converts the Buffer to grayscale.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1117:ParametersMustBeOnSameLineOrSeparateLines", Justification = "Reviewed. Suppression is OK here.")]
        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1115:ParameterMustFollowComma", 
            Justification = "Unnecessary newlines reduce readability due to high nesting of scopes.")]
        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1116:SplitParametersMustStartOnLineAfterDeclaration", 
            Justification = "Unnecessary newlines reduce readability due to high nesting of scopes.")]
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

                        byte avg = (byte)((double)(valR + valG + valB) / 3D);

                        *scan0 = avg;
                        *(scan0 + 1) = avg;
                        *(scan0 + 2) = avg;
                    };
                    break;
                default:
                    throw new NotImplementedException(Resources.UnsupportedImageColorDepth);
            }

            int bufferHeight = this.Buffer.Height;
            int bufferWidth = this.Buffer.Width;

            FastBitmap.Operation(this.Buffer, (data, scan0) =>
            {
                Parallel.For(0, bufferHeight, yPos =>
                {
                    int localWidth = bufferWidth;

                    Parallel.For(0, localWidth, xPos =>
                    {
                        byte* pixel = scan0 + (yPos * data.Stride) + ((xPos * this.bitsPerPixel) / 8);

                        grayscale(pixel);
                    });
                });
            });
        }

        /// <summary>
        /// Performs the supplied operation on the supplied bitmap.
        /// </summary>
        /// <param name="bitmap">
        /// The bitmap to perform the operation on.
        /// </param>
        /// <param name="operation">
        /// The operation to perform on the contents of the supplied Bitmap.
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
        /// Applies the supplied kernel to the buffer.
        /// </summary>
        /// <param name="kernel">
        /// The kernel to apply. Assumed to be square and of odd dimensions.
        /// </param>
        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1115:ParameterMustFollowComma",
             Justification = "Unecessary newlines reduce readability due to the high nesting of scopes.")]
        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1116:SplitParametersMustStartOnLineAfterDeclaration",
             Justification = "Unecessary newlines reduce readability due to the high nesting of scopes.")]
        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1117:ParametersMustBeOnSameLineOrSeparateLines",
             Justification = "Unecessary newlines reduce readability due to the high nesting of scopes.")]
        [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1409:RemoveUnnecessaryCode",
            Justification = "Empty lock is intentional to ensure all threads exit before attempting disposal.")]
        protected unsafe void KernelOperation(Kernel kernel)
        {
            Rectangle cropRect = new Rectangle(
                kernel.Center,
                kernel.Center,
                this.Buffer.Width - (kernel.Center * 2),
                this.Buffer.Height - (kernel.Center * 2));

            Bitmap subBuffer = new Bitmap(cropRect.Width, cropRect.Height);

            using (Graphics graphics = Graphics.FromImage(subBuffer))
            {
                graphics.DrawImage(
                    this.Buffer,
                    new Rectangle(0, 0, subBuffer.Width, subBuffer.Height),
                    cropRect,
                    GraphicsUnit.Pixel);
            }

            FastBitmap.Operation(subBuffer, (subData, subScan0) =>
            {
                // ReSharper disable AccessToDisposedClosure
                FastBitmap.Operation(this.Buffer, (data, scan0) =>
                // ReSharper restore AccessToDisposedClosure
                {
                    int localWidth = subBuffer.Width;

                    Parallel.For(0, subBuffer.Height, yPos =>
                    {
                        Parallel.For(0, localWidth, xPos =>
                        {
                            byte* current =
                                subScan0 +
                                (yPos * subData.Stride) +
                                ((xPos * this.bitsPerPixel) / 8);

                            double newR = 0D;
                            double newG = 0D;
                            double newB = 0D;

                            for (int localY = -kernel.Center; localY <= kernel.Center; localY++)
                            {
                                for (int localX = -kernel.Center; localX <= kernel.Center; localX++)
                                {
                                    double scaling = kernel.Contents[localY + kernel.Center, localX + kernel.Center];

                                    byte* local =
                                        scan0 +
                                        ((yPos + kernel.Center + localY) * data.Stride) +
                                        (((xPos + kernel.Center + localX) * this.bitsPerPixel) / 8);

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

            this.Buffer.Dispose();
            this.Buffer = subBuffer;
        }
    }
}
