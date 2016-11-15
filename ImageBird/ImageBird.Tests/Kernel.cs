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

        [Fact]
        public void GaussianKernel_ValidParameters_ShouldSucceed()
        {
            // The expected kernels were precomputed from an external resource.
            double[,] expected = new double[,]
            {
                { 0.003765D, 0.015019D, 0.023792D, 0.015019D, 0.003765D },
                { 0.015019D, 0.059912D, 0.094907D, 0.059912D, 0.015019D },
                { 0.023792D, 0.094907D, 0.150342D, 0.094907D, 0.023792D },
                { 0.015019D, 0.059912D, 0.094907D, 0.059912D, 0.015019D },
                { 0.003765D, 0.015019D, 0.023792D, 0.015019D, 0.003765D }
            };
            SUT.Kernel actual = SUT.Kernel.Gaussian(1D, 5);
            Assert.True(TestUtil.TruncatedContentsEqual(expected, actual, 6));

            expected = new double[,]
            {
                { 0.001122D, 0.003633D, 0.007349D, 0.009293D, 0.007349D, 0.003633D, 0.001122D },
                { 0.003633D, 0.011761D, 0.023789D, 0.030083D, 0.023789D, 0.011761D, 0.003633D },
                { 0.007349D, 0.023789D, 0.048117D, 0.060847D, 0.048117D, 0.023789D, 0.007349D },
                { 0.009293D, 0.030083D, 0.060847D, 0.076945D, 0.060847D, 0.030083D, 0.009293D },
                { 0.007349D, 0.023789D, 0.048117D, 0.060847D, 0.048117D, 0.023789D, 0.007349D },
                { 0.003633D, 0.011761D, 0.023789D, 0.030083D, 0.023789D, 0.011761D, 0.003633D },
                { 0.001122D, 0.003633D, 0.007349D, 0.009293D, 0.007349D, 0.003633D, 0.001122D }
            };
            actual = SUT.Kernel.Gaussian(1.43D, 7);
            Assert.True(TestUtil.TruncatedContentsEqual(expected, actual, 6));
        }
    }
}
