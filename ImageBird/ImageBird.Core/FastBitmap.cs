﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using ImageBird.Core.Properties;
using System.Security.Cryptography;

namespace ImageBird.Core
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
        public FastBitmap(Bitmap bitmap)
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
        protected FastBitmap(Bitmap bitmap, int bitsPerPixel)
        {
            this.Content = (Bitmap)bitmap.Clone();
            this.bitsPerPixel = bitsPerPixel;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FastBitmap"/> class with a new <see cref="Bitmap"/> whose pixels have a value 0 for all channels. Does not check supplied parameters for validity.
        /// </summary>
        /// <param name="width">
        /// The width of the <see cref="FastBitmap"/>.
        /// </param>
        /// <param name="height">
        /// The height of the <see cref="FastBitmap"/>.
        /// </param>
        protected FastBitmap(int width, int height)
        {
            this.Content = new Bitmap(width, height);
            this.bitsPerPixel = Image.GetPixelFormatSize(this.Content.PixelFormat);
        }

        /// <summary>
        /// Performs a locking data operation on the supplied <see cref="BitmapData"/> object.
        /// </summary>
        /// <param name="data">
        /// The <see cref="BitmapData"/>.
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
        /// DEBUG METHOD.
        /// </summary>
        /// <returns>
        /// DEBUGGERY.
        /// </returns>
        public unsafe FastBitmap Debug()
        {
            return this.KernelOperation(Kernel.VerticalSobel);
        }

        /// <summary>
        /// Disposes the <see cref="FastBitmap"/>, freeing it's resources.
        /// </summary>
        public void Dispose()
        {
            this.Content.Dispose();
        }

        /// <summary>
        /// Performs a Canny Edge Detect on the <see cref="FastBitmap"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="FastBitmap"/> whose contents represent the edges of the current <see cref="FastBitmap"/>.
        /// </returns>
        public unsafe FastBitmap EdgeDetect()
        {
            (FastBitmap magnitude, double[,] thetas) edgeDetect = this.Sobel(computeThetas: true);
            FastBitmap buffer = edgeDetect.magnitude;
            double[,] angles = edgeDetect.thetas;
            
            FastBitmap output = new FastBitmap(new Bitmap(buffer.Content.Width - 2, buffer.Content.Height - 2));

            FastBitmap.Operation(output.Content, (outputData, outputScan0) =>
            {
                FastBitmap.Operation(buffer.Content, (bufferData, bufferScan0) =>
                {
                    int localWidth = output.Content.Width + 1;
                    int localBpp = buffer.bitsPerPixel;

                    for (int yPos = 1; yPos < output.Content.Height + 1; yPos++)
                    {
                        for (int xPos = 1; xPos < localWidth; xPos++)
                        {
                            int prevX = 0;
                            int prevY = 0;
                            int nextX = 0;
                            int nextY = 0;
                            switch (angles[xPos - 1, yPos - 1])
                            {
                                case 0:
                                    prevX = -1;
                                    nextX = 1;
                                    break;
                                case 1:
                                    prevX = -1;
                                    nextX = 1;
                                    prevY = 1;
                                    nextY = -1;
                                    break;
                                case 2:
                                    prevY = -1;
                                    nextY = 1;
                                    break;
                                case 3:
                                    prevX = -1;
                                    nextX = 1;
                                    prevY = -1;
                                    nextY = 1;
                                    break;
                            }

                            byte* outputA = FastBitmap.PixelPointer(outputScan0, xPos - 1, yPos - 1, outputData.Stride, output.bitsPerPixel) + 3;
                            *outputA = 255;

                            byte* currentR = FastBitmap.PixelPointer(bufferScan0, xPos, yPos, bufferData.Stride, localBpp);
                            byte* previousR = FastBitmap.PixelPointer(bufferScan0, xPos + prevX, yPos + prevY, bufferData.Stride, buffer.bitsPerPixel);
                            byte* nextR = FastBitmap.PixelPointer(bufferScan0, xPos + nextX, yPos + nextY, bufferData.Stride, buffer.bitsPerPixel);

                            if (*currentR > *previousR && *currentR > *nextR)
                            {
                                *(outputA - 3) = 255;
                                *(outputA - 2) = 255;
                                *(outputA - 1) = 255;
                            }
                            else
                            {
                                *currentR = 0;
                                *(currentR + 1) = 0;
                                *(currentR + 2) = 0;
                            }
                        }
                    }
                });
            });

            return output;
        }

        /// <summary>
        /// Placeholder.
        /// </summary>
        /// <returns>
        /// Placeholder.
        /// </returns>
        public string GetPerceptualHash()
        {
            // Placeholder for working on <see cref="ImageBird.Frontend.Shared.Index"/>.
            return Guid.NewGuid().ToString();
        }

        /// <summary>
        /// Returns the largest magnitude in the <see cref="FastBitmap"/>.
        /// </summary>
        /// <returns>
        /// The largest magnitude in the <see cref="FastBitmap"/>.
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
        /// The number of angles to consider. Should be a power of 2.
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

            if ((numberOfAngles & (numberOfAngles - 1)) != 0)
            {
                throw new ArgumentException(Resources.NumberOfAnglesShouldBePowerOfTwo, nameof(numberOfAngles));
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
        /// Performs Sobel edge detection on the <see cref="FastBitmap"/>.
        /// </summary>
        /// <param name="computeThetas">
        /// True if the thetas should be computed in addition to the resulting <see cref="FastBitmap"/>, and false otherwise. Defaults to false.
        /// </param>
        /// <returns>
        /// A named-field tuple containing the resulting Sobel-filtered <see cref="FastBitmap"/>, and the calculated thetas. If <paramref name="computeThetas"/> was false, then thetas will be null.
        /// </returns>
        public unsafe (FastBitmap magnitude, double[,] thetas) Sobel(bool computeThetas = false)
        {
            FastBitmap horizontal = this.KernelOperation(Kernel.HorizontalSobel);
            FastBitmap vertical = this.KernelOperation(Kernel.VerticalSobel);

            FastBitmap result = new FastBitmap(horizontal.Content.Width, horizontal.Content.Height);

            FastBitmap.Operation(result.Content, (resultData, resultScan0) =>
            {
                FastBitmap.Operation(horizontal.Content, (horizontalData, horizontalScan0) =>
                {
                    FastBitmap.Operation(vertical.Content, (verticalData, verticalScan0) =>
                    {
                        for (int yPos = 0; yPos < horizontal.Content.Height; yPos++)
                        {
                            for (int xPos = 0; xPos < horizontal.Content.Width; xPos++)
                            {
                                byte* horizontalR = FastBitmap.PixelPointer(
                                    horizontalScan0, 
                                    xPos, 
                                    yPos, 
                                    horizontalData.Stride, 
                                    horizontal.bitsPerPixel);
                                byte* horizontalG = horizontalR + 1;
                                byte* horizontalB = horizontalR + 2;

                                byte* verticalR = FastBitmap.PixelPointer(
                                    verticalScan0, 
                                    xPos, 
                                    yPos, 
                                    verticalData.Stride, 
                                    vertical.bitsPerPixel);
                                byte* verticalG = verticalR + 1;
                                byte* verticalB = verticalR + 2;

                                byte* resultR = FastBitmap.PixelPointer(
                                    resultScan0, 
                                    xPos, 
                                    yPos, 
                                    resultData.Stride, 
                                    result.bitsPerPixel);
                                byte* resultG = resultR + 1;
                                byte* resultB = resultR + 2;

                                *resultR = (byte)(255 * Util.Sqrt(*horizontalR * *horizontalR + *verticalR * *verticalR) / 360);
                                *resultG = (byte)(255 * Util.Sqrt(*horizontalG * *horizontalG + *verticalG * *verticalG) / 360);
                                *resultB = (byte)(255 * Util.Sqrt(*horizontalB * *horizontalB + *verticalB * *verticalB) / 360);
                                *(resultR + 3) = 255;
                            }
                        }
                    });
                });
            }, ImageLockMode.WriteOnly);

            double[,] thetas = null;
            if (computeThetas)
            {
                FastBitmap.Operation(horizontal.Content, (horizontalData, horizontalScan0) =>
                {
                    FastBitmap.Operation(vertical.Content, (verticalData, verticalScan0) =>
                    {
                        for (int yPos = 0; yPos < horizontal.Content.Height; yPos++)
                        {
                            for (int xPos = 0; xPos <horizontal.Content.Width; xPos++)
                            {
                                thetas[xPos, yPos] = Math.Atan2(
                                    *FastBitmap.PixelPointer(
                                        verticalScan0,
                                        xPos,
                                        yPos,
                                        verticalData.Stride,
                                        vertical.bitsPerPixel),
                                    *FastBitmap.PixelPointer(
                                        horizontalScan0,
                                        xPos,
                                        yPos,
                                        horizontalData.Stride,
                                        horizontal.bitsPerPixel));
                            }
                        }
                    });
                });
            }

            return (result, thetas);
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
        /// <param name="lockMode">
        /// The locking mode for the <see cref="Bitmap"/>. Defaults to <see cref="ImageLockMode.ReadOnly"/>.
        /// </param>
        protected static unsafe void Operation(
            Bitmap bitmap, 
            LockingDataOperation operation, 
            ImageLockMode lockMode = ImageLockMode.ReadOnly)
        {
            BitmapData contents = bitmap.LockBits(
                new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                lockMode,
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
            FastBitmap result = new FastBitmap(
                this.Content.Width - kernel.Dimension + 1, 
                this.Content.Height - kernel.Dimension + 1);

            (double r, double g, double b)[,] buffer = 
                new (double r, double g, double b)[result.Content.Width, result.Content.Height];

            FastBitmap.Operation(this.Content, (actualData, actualScan0) =>
            {
                int localWidth = result.Content.Width;

                Parallel.For(0, result.Content.Height, yPos =>
                {
                    Parallel.For(0, localWidth, xPos =>
                    {
                        double sumR = 0D;
                        double sumG = 0D;
                        double sumB = 0D;

                        for (int kernelY = 0; kernelY < kernel.Dimension; kernelY++)
                        {
                            for (int kernelX = 0; kernelX < kernel.Dimension; kernelX++)
                            {
                                double scalar = kernel[kernelX, kernelY];

                                byte* actualR = FastBitmap.PixelPointer(
                                    actualScan0, 
                                    xPos + kernelX, 
                                    yPos + kernelY, 
                                    actualData.Stride, 
                                    this.bitsPerPixel);

                                sumR += (*actualR * scalar);
                                sumG += (*(actualR + 1) * scalar);
                                sumB += (*(actualR + 2) * scalar);
                            }
                        }

                        if (sumR < 0D)
                        {
                            sumR = -sumR;
                        }

                        if (sumG < 0D)
                        {
                            sumG = -sumG;
                        }

                        if (sumB < 0D)
                        {
                            sumB = -sumB;
                        }

                        buffer[xPos, yPos] = (sumR, sumG, sumB);
                    });
                });
            });

            double max = double.MinValue;

            foreach ((double r, double g, double b) tuple in buffer)
            {
                if (tuple.r > max)
                {
                    max = tuple.r;
                }

                if (tuple.g > max)
                {
                    max = tuple.g;
                }

                if (tuple.b > max)
                {
                    max = tuple.b;
                }
            }

            FastBitmap.Operation(result.Content, (data, scan0) =>
            {
                // HACK: Code is duplicated for performance - don't want to scale the channels unless necessary, and don't want to repeat the check many times.
                //if (max > 255)
                //{
                //    Parallel.For(0, data.Height, yPos =>
                //    {
                //        for (int xPos = 0; xPos < data.Width; xPos++)
                //        {
                //            byte* resultR = FastBitmap.PixelPointer(
                //                scan0,
                //                xPos,
                //                yPos,
                //                data.Stride,
                //                result.bitsPerPixel);

                //            *resultR = (byte)(Math.Round((buffer[xPos, yPos].r / max)) * 255);
                //            *(resultR + 1) = (byte)(Math.Round((buffer[xPos, yPos].g / max)) * 255);
                //            *(resultR + 2) = (byte)(Math.Round((buffer[xPos, yPos].b / max)) * 255);
                //            *(resultR + 3) = 255;
                //        }
                //    });
                //}
                //else
                //{
                    Parallel.For(0, data.Height, yPos =>
                    {
                        for (int xPos = 0; xPos < data.Width; xPos++)
                        {
                            byte* resultR = FastBitmap.PixelPointer(
                                scan0,
                                xPos,
                                yPos,
                                data.Stride,
                                result.bitsPerPixel);

                            *resultR = (byte)(buffer[xPos, yPos].r);
                            *(resultR + 1) = (byte)(buffer[xPos, yPos].g);
                            *(resultR + 2) = (byte)(buffer[xPos, yPos].b);

                            *(resultR + 3) = 255;
                        }
                    });
                //}
            }, ImageLockMode.WriteOnly);

            return result;
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
