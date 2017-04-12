using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using SUT = ImageBird.Core;

namespace ImageBird.Core.Tests
{
    /// <summary>
    /// Tests for the ImageBird.Kernel class.
    /// </summary>
    public class Kernel
    {
        private static readonly double[,] TestData1_KnownGood = { { 1D, 2D, 3D }, { 4D, 5D, 6D }, { 7D, 8D, 9D } };

        [Fact]
        public void Clone_ValidKernel_ShouldSucceed()
        {
            SUT.Kernel actual = (SUT.Kernel)TestKernel.InstantiateArbitraryKernel(TestData1_KnownGood).Clone();

            Assert.Equal(1, actual.Center);
            Assert.Equal(3, actual.Dimension);
            Assert.Equal(TestData1_KnownGood, actual.Contents);
        }

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

        [Fact]
        public void ToString_ValidKernel_ShouldSucceed()
        {
            Assert.Equal(
                "3:1\n1, 2, 3,\n4, 5, 6,\n7, 8, 9", 
                TestKernel.InstantiateArbitraryKernel(TestData1_KnownGood).ToString());
        }
    }
}
