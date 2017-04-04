using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageBird
{
    /// <summary>
    /// Represents a square 2-dimensional kernel used for kernel convolution.
    /// </summary>
    public class Kernel : ICloneable
    {
        private static Lazy<Kernel> horizontalSobel = new Lazy<Kernel>(() =>
            new Kernel(new double[,] { { 1D, 0D, -1D }, { 2D, 0D, -2D }, { 1D, 0D, -1D } }) / 4D);

        private static Lazy<Kernel> verticalSobel = new Lazy<Kernel>(() =>
            new Kernel(new double[,] { { 1D, 2D, 1D }, { 0D, 0D, 0D }, { -1D, -2D, -1D } }) / 4D);

        /// <summary>
        /// Instantiates a new Kernel with the specified contents.
        /// </summary>
        /// <param name="contents">
        /// The kernel's values.
        /// </param>
        protected Kernel(double[,] contents)
        {
            this.Contents = contents;
            this.Dimension = Util.Sqrt(contents.Length);
            this.Center = this.Dimension / 2;
        }

        /// <summary>
        /// The horizontal Sobel kernel.
        /// </summary>
        public static Kernel HorizontalSobel
        {
            get
            {
                return Kernel.horizontalSobel.Value;
            }
        }

        /// <summary>
        /// The vertical Sobel kernel.
        /// </summary>
        public static Kernel VerticalSobel
        {
            get
            {
                return Kernel.verticalSobel.Value;
            }
        }

        /// <summary>
        /// The contents of the kernel.
        /// </summary>
        public double[,] Contents { get; protected set; }

        /// <summary>
        /// The dimension of the kernel.
        /// </summary>
        public int Dimension { get; protected set; }

        /// <summary>
        /// The index representing the center of the kernel.
        /// </summary>
        public int Center { get; protected set; }

        /// <summary>
        /// Returns the associated value of the <see cref="Kernel"/>.
        /// </summary>
        /// <param name="xPos">
        /// The zero-indexed X coordinate, starting from the left.
        /// </param>
        /// <param name="yPos">
        /// The zero-indexed Y coordinate, starting from the top.
        /// </param>
        /// <returns>
        /// The associated value.
        /// </returns>
        public double this[int xPos, int yPos]
        {
            get
            {
                return this.Contents[yPos, xPos];
            }
        }

        /// <summary>
        /// Computes the Gaussian kernel for the supplied sigma and weight.
        /// </summary>
        /// <param name="sigma">
        /// The factor by which to blur. Larger sigmas produce greater blurring.
        /// </param>
        /// <param name="weight">
        /// The size of the kernel. Larger weights produce greater blurring.
        /// </param>
        public static Kernel Gaussian(double sigma, int weight)
        {
            if (sigma <= 0)
            {
                throw new ArgumentException(nameof(sigma));
            }

            if (weight < 1 || weight % 2 == 0)
            {
                throw new ArgumentException(nameof(weight));
            }

            double[,] kernel = new double[weight, weight];

            double sigmaSquared = sigma * sigma;
            double euler = 1D / (2D * Math.PI * sigmaSquared);
            double sum = 0;
            int center = weight / 2;

            for (int yPos = -center; yPos <= center; yPos++)
            {
                for (int xPos = -center; xPos <= center; xPos++)
                {
                    int actualY = yPos + center;
                    int actualX = xPos + center;

                    double value = euler * Math.Exp(-((xPos * xPos) + (yPos * yPos)) / (2 * sigmaSquared));

                    kernel[actualY, actualX] = value;
                    sum += value;
                }
            }

            return new Kernel(kernel) / sum;
        }

        /// <summary>
        /// Divides each value in the <paramref name="kernel"/> by the <paramref name="divisor"/>.
        /// </summary>
        /// <param name="kernel">
        /// The <see cref="Kernel"/> for which each value should be divided by <paramref name="divisor"/>.
        /// </param>
        /// <param name="divisor">
        /// The divisor.
        /// </param>
        /// <returns>
        /// A <see cref="Kernel"/> where each value is equal to <paramref name="kernel"/> at that value's indices, divided by the divsor.
        /// </returns>
        public static Kernel operator /(Kernel kernel, double divisor)
        {
            Kernel output = (Kernel)kernel.Clone();

            for (int yPos = 0; yPos < kernel.Dimension; yPos++)
            {
                for (int xPos = 0; xPos < kernel.Dimension; xPos++)
                {
                    kernel.Contents[yPos, xPos] /= divisor;
                }
            }

            return output;
        }

        /// <summary>
        /// Creates a shallow copy of the Kernel.
        /// </summary>
        public object Clone()
        {
            return new Kernel((double[,])this.Contents.Clone());
        }

        /// <summary>
        /// Returns a string that represents the Kernel.
        /// </summary>
        public override string ToString()
        {
            IEnumerable<double> asEnumerable = this.Contents.Cast<double>();
            return string.Format(
                "{0}:{1}\n{2}", 
                this.Dimension, 
                this.Center, 
                string.Join(
                    ",\n", 
                    Enumerable.Range(0, this.Dimension)
                        .Select(x => 
                            string.Join(
                                ", ", 
                                asEnumerable
                                    .Skip(this.Dimension * x)
                                    .Take(this.Dimension)
                                    .Select(y => y.ToString())))));
        }
    }
}
