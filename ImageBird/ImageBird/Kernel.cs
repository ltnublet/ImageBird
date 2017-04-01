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
        /// <summary>
        /// Instantiates a new Kernel with the specified contents.
        /// </summary>
        /// <param name="contents">The kernel's values.</param>
        protected Kernel(double[,] contents)
        {
            this.Contents = contents;
            this.Dimension = (int)Math.Sqrt(contents.Length);
            this.Center = this.Dimension / 2;
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

            for (int yPos = 0; yPos < weight; yPos++)
            {
                for (int xPos = 0; xPos < weight; xPos++)
                {
                    kernel[yPos, xPos] /= sum;
                }
            }

            return new Kernel(kernel);
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
