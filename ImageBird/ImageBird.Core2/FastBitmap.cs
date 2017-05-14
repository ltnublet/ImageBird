using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ImageBird.Core2.Properties;

namespace ImageBird.Core2
{
    public class FastBitmap
    {
        /// <summary>
        /// The <see cref="Bitmap"/> class defaults to 32 bits per pixel when instantiated with <see cref="Bitmap.Bitmap(int, int)"/>, and exposes images as 32 bits per pixel regardless of the source file when instantiated with <see cref="Bitmap.Bitmap(Image)"/>.
        /// </summary>
        private const int bpp = 32;

        public FastBitmap(Bitmap content)
        {
            if (content == null)
            {
                throw new ArgumentNullException(nameof(content), Resources.SpecifiedFileDidNotExist);
            }

            if (content.Height == 0 || content.Width == 0)
            {
                throw new ArgumentException(Resources.SuppliedBitmapHasInvalidDimensions, nameof(content));
            }

            this.Content = (Bitmap)content.Clone();
        }

        protected FastBitmap(int width, int height)
        {
            this.Content = new Bitmap(width, height);
        }

        internal unsafe delegate void LockingDataOperation(BitmapData data, byte* scan0);

        public Bitmap Content { get; private set; }

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

        public unsafe FastBitmap Blur(double sigma, int weight)
        {
            Kernel kernel = Kernel.Gaussian(sigma, weight);

            FastBitmap result = new FastBitmap(
                this.Content.Width - kernel.Dimension + 1,
                this.Content.Height - kernel.Dimension + 1);

            int height = result.Content.Height;
            int width = result.Content.Width;

            this.PerformLockingOperation((readData, readScan0) =>
            {
                result.PerformLockingOperation((writeData, writeScan0) =>
                {
                    for (int yPos = 0; yPos < height; yPos++)
                    {
                        for (int xPos = 0; xPos < width; xPos++)
                        {
                            double sumR = 0D;
                            double sumG = 0D;
                            double sumB = 0D;

                            byte* writingTo = PixelPointer(
                                writeScan0, 
                                xPos, 
                                yPos, 
                                writeData.Stride, 
                                bpp);

                            for (int yKernel = 0; yKernel < kernel.Dimension; yKernel++)
                            {
                                for (int xKernel = 0; xKernel < kernel.Dimension; xKernel++)
                                {
                                    double scalar = kernel[xKernel, yKernel];

                                    byte* readingFrom = PixelPointer(
                                        readScan0,
                                        xPos + xKernel,
                                        yPos + yKernel,
                                        readData.Stride,
                                        bpp);

                                    sumR += (*readingFrom * scalar);
                                    sumG += (*(readingFrom + 1) * scalar);
                                    sumB += (*(readingFrom + 2) * scalar);
                                }
                            }
                            
                            *(writingTo + 0) = (byte)(sumR);
                            *(writingTo + 1) = (byte)(sumG);
                            *(writingTo + 2) = (byte)(sumB);
                            *(writingTo + 3) = *PixelPointer(readScan0, xPos, yPos, readData.Stride, bpp);
                        }
                    }
                });
            });
            
            return result;
        }

        public unsafe FastBitmap Grayscale()
        {
            int height = this.Content.Height;
            int width = this.Content.Width;

            FastBitmap result = new FastBitmap(width, height);

            this.PerformLockingOperation((readData, readScan0) => 
            {
                result.PerformLockingOperation((writeData, writeScan0) =>
                {
                    for (int yPos = 0; yPos < height; yPos++)
                    {
                        for (int xPos = 0; xPos < width; xPos++)
                        {
                            byte* readingFrom = PixelPointer(
                                readScan0, 
                                xPos, 
                                yPos, 
                                readData.Stride, 
                                bpp);
                            byte* writingTo = PixelPointer(
                                writeScan0, 
                                xPos, 
                                yPos, 
                                writeData.Stride, 
                                bpp);

                            byte average = (byte)((*(readingFrom) + *(readingFrom + 1) + *(readingFrom + 2)) / 3);
                            *writingTo = average;
                            *(writingTo + 1) = average;
                            *(writingTo + 2) = average;
                            *(writingTo + 3) = 255;
                        }
                    }
                });
            });

            return result;
        }

        internal static unsafe void PerformLockingOperation(Bitmap bitmap, LockingDataOperation operation)
        {
            lock (bitmap)
            {
                BitmapData data = bitmap.LockBits(
                    new Rectangle(
                        0,
                        0,
                        bitmap.Width,
                        bitmap.Height), 
                    ImageLockMode.ReadWrite, 
                    bitmap.PixelFormat);

                try
                {
                    operation(data, (byte*)data.Scan0.ToPointer());
                }
                finally
                {
                    bitmap.UnlockBits(data);
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static unsafe byte* PixelPointer(byte* scan0, int xPos, int yPos, int stride, int bpp)
        {
            return scan0 + (yPos * stride) + ((xPos * bpp) / 8);
        }
    }
}
