using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using SUT = ImageBird;

namespace ImageBird.Tests
{
    /// <summary>
    /// Tests for the ImageBird.Kernel class.
    /// </summary>
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Underscores are used to make names human-readable.")]
    [SuppressMessage("ReSharper", "StyleCop.SA1310", Justification = "Underscores are used to make names human-readable.")]
    [SuppressMessage("ReSharper", "StyleCop.SA1600", Justification = "Test names should adequately describe the SUT, supplied data, and outcome.")]
    public class Kernel
    {
        [Theory]
        [InlineData(-1f, 3)]
        [InlineData(0f, 3)]
        [InlineData(1f, 0)]
        [InlineData(1f, 2)]
        public void GaussianKernel_InvalidParameters_ThrowsArgument(double sigma, int weight)
        {
            Assert.Throws<ArgumentException>(() =>
                SUT.Kernel.Gaussian(sigma, weight));
        }

        [Fact(Skip = "Haven't determined an adequate way of testing for correctness (rounding errors between various methods result in different kernels).")]
        public void GaussianKernel_ValidParameters_ShouldSucceed()
        {
            throw new NotImplementedException();
        }
    }
}
