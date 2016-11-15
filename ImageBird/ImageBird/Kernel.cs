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
    public class Kernel
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
            double avg = weight / 2f;
            double sum = 0f;

            for (int x = 0; x < weight; x++)
            {
                for (int y = 0; y < weight; y++)
                {
                    kernel[x, y] =
                        (double)
                        Math.Exp(
                            -0.5f
                            * (Math.Pow(((double)(x)-avg) / sigma, 2D) + Math.Pow(((double)(y)-avg) / sigma, 2D)))
                        / (double)(2f * Math.PI * sigma * sigma);

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

            return new Kernel(kernel);
        }
    }
}
