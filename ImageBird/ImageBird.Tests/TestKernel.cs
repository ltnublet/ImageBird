using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SUT = ImageBird;

namespace ImageBird.Tests
{
    /// <summary>
    /// Internal class for Kernel tests which allows for the instantiation of arbitrary kernels.
    /// </summary>
    internal class TestKernel : SUT.Kernel
    {
        /// <summary>
        /// Instantiates the TestKernel using the supplied array.
        /// </summary>
        protected TestKernel(double[,] contents)
            : base(contents)
        {
        }

        /// <summary>
        /// Instantiates an arbitrary Kernel using the supplied array. The resultant kernel is not guaranteed to be
        /// valid. This method is only intended for use in tests.
        /// </summary>
        /// <param name="contents">The array with which to instantiate the Kernel.</param>
        /// <returns>The Kernel.</returns>
        internal static SUT.Kernel InstantiateArbitraryKernel(double[,] contents)
        {
            return new TestKernel(contents);
        }
    }
}
