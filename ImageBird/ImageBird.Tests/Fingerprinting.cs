using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using SUT = ImageBird;

namespace ImageBird.Tests
{
    /// <summary>
    /// This class contains all tests for the ImageBird.Fingerprinting class.
    /// </summary>
    public class Fingerprinting
    {
        /// <summary>
        /// Test the Fingerprint method with an invalid (null) image for each algorithm.
        /// </summary>
        /// <param name="mode">The mode to attempt the fingerprinting with.</param>
        [Theory]
        [InlineData(SUT.Fingerprinting.FingerprintMode.PCASIFT)]
        [InlineData(SUT.Fingerprinting.FingerprintMode.PHASH)]
        [InlineData(SUT.Fingerprinting.FingerprintMode.SIFT)]
        [InlineData(SUT.Fingerprinting.FingerprintMode.SURF)]
        public void Fingerprint_NullImage_AllModes_ThrowsArgumentNull(
            SUT.Fingerprinting.FingerprintMode mode)
        {
            Bitmap testImage = null;

            Assert.Throws<ArgumentNullException>(
                () => SUT.Fingerprinting.Fingerprint(testImage, mode));
        }

        /// <summary>
        /// Test the Fingerprint method with an invalid (not in enumerable) mode.
        /// </summary>
        [Fact]
        public void Fingerprint_InvalidMode_ThrowsArgument()
        {
            Bitmap testImage = new Bitmap(1, 1);
            SUT.Fingerprinting.FingerprintMode testMode = 
                (SUT.Fingerprinting.FingerprintMode)(-1);

            // Check that the mode we're assuming is invalid is actually invalid
            Assert.DoesNotContain(
                testMode, 
                Enum.GetValues(typeof(SUT.Fingerprinting.FingerprintMode))
                    .Cast<SUT.Fingerprinting.FingerprintMode>());

            Assert.Throws<ArgumentException>(
                () => SUT.Fingerprinting.Fingerprint(testImage, testMode));
        }

        /// <summary>
        /// Test the Fingerprint method with an invalid (malformed, non-null) image for each algorithm.
        /// </summary>
        /// <param name="mode">The mode to attempt the fingerprinting with.</param>
        [Theory]
        [InlineData(SUT.Fingerprinting.FingerprintMode.PCASIFT)]
        [InlineData(SUT.Fingerprinting.FingerprintMode.PHASH)]
        [InlineData(SUT.Fingerprinting.FingerprintMode.SIFT)]
        [InlineData(SUT.Fingerprinting.FingerprintMode.SURF)]
        public void Fingerprint_InvalidImage_AllModes_ThrowsArgument(
            SUT.Fingerprinting.FingerprintMode mode)
        {
            Bitmap testImage = new Bitmap(1, 1);

            Assert.Throws<ArgumentException>(() => SUT.Fingerprinting.Fingerprint(testImage, mode));
        }

        /// <summary>
        /// Test the Fingerprint method with a valid image for each algorithm.
        /// </summary>
        /// <param name="expected">The expected output for the associated mode.</param>
        /// <param name="mode">The mode to attempt the fingerprinting with.</param>
        [Theory]
        [InlineData("Placeholder", SUT.Fingerprinting.FingerprintMode.PCASIFT)]
        [InlineData("Placeholder", SUT.Fingerprinting.FingerprintMode.PHASH)]
        [InlineData("Placeholder", SUT.Fingerprinting.FingerprintMode.SIFT)]
        [InlineData("Placeholder", SUT.Fingerprinting.FingerprintMode.SURF)]
        public void Fingerprint_ValidImage_AllModes_ShouldSucceed(
            string expected,
            SUT.Fingerprinting.FingerprintMode mode)
        {
            Bitmap testImage = new Bitmap(1, 1);

            Assert.Equal(expected, SUT.Fingerprinting.Fingerprint(testImage, mode));
        }
    }
}
