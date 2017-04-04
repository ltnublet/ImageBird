using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageBird
{
    /// <summary>
    /// Bag of utility methods.
    /// </summary>
    internal class Util
    {
        /// <summary>
        /// Integer square root function "borrowed" from @"http://stackoverflow.com/a/5345819".
        /// </summary>
        /// <param name="num">
        /// The number to estimate the square root of.
        /// </param>
        /// <returns>
        /// An integer approximation of the square root of <paramref name="num"/>.
        /// </returns>
        public static int Sqrt(int num)
        {
            if (num == 0)
            {
                return 0;
            }

            int n = (num / 2) + 1;
            int n1 = (n + (num / n)) / 2;

            while (n1 < n)
            {
                n = n1;
                n1 = (n + (num / n)) / 2;
            }

            return n;
        }
    }
}
