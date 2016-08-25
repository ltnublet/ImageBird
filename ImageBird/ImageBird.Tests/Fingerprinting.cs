using System;
using System.Drawing;
using Xunit;
using SUT = ImageBird;

namespace ImageBird.Tests 
{
    /// <summary>
    /// This class contains all functionality related to testing the ImageBird.Fingerprinting class.
    /// </summary>
    public class Fingerprinting 
    {
        /// <summary>
        /// Test the Fingerprint method, using a null Bitmap.
        /// </summary>
        [Fact]
        public void Fingerprint_NullBitmap_ShouldThrowArgumentNull()
        {
            Bitmap testData = null;
            // PCASIFT chosen arbitrarily, as it should not impact results of test.
            SUT.Fingerprinting.FingerprintMode testMode = 
                ImageBird.Fingerprinting.FingerprintMode.PCASIFT;

            Assert.Throws<ArgumentNullException>(
                () => SUT.Fingerprinting.Fingerprint(testData, testMode));
        }

        /// <summary>
        /// Test the fingerprint method, using an invalid enum argument.
        /// </summary>
        [Fact]
        public void Fingerprint_InvalidEnum_ShouldThrowArgument()
        {
            Bitmap testData = new Bitmap(1, 1);
            SUT.Fingerprinting.FingerprintMode testMode =
                (SUT.Fingerprinting.FingerprintMode)(-1);

            Assert.Throws<ArgumentException>(
                () => SUT.Fingerprinting.Fingerprint(testData, testMode));
        }

        /// <summary>
        /// Test the Fingerprint method, using an invalid (malformed) Bitmap and the PCASIFT mode.
        /// </summary>
        [Fact]
        public void Fingerprint_InvalidBitmap_PCASIFT_ShouldThrowArgument()
        {
            // TODO: Implement test once related method minimally matches behaviour.
            Assert.True(false, "Test not implemented.");
        }

        /// <summary>
        /// Test the Fingerprint method, using an invalid (malformed) Bitmap and the SIFT mode.
        /// </summary>
        [Fact]
        public void Fingerprint_InvalidBitmap_SIFT_ShouldThrowArgument()
        {
            // TODO: Implement test once related method minimally matches behaviour.
            Assert.True(false, "Test not implemented.");
        }

        /// <summary>
        /// Test the Fingerprint method, using an invalid (malformed) Bitmap and the SURF mode.
        /// </summary>
        [Fact]
        public void Fingerprint_InvalidBitmap_SURF_ShouldThrowArgument()
        {
            // TODO: Implement test once related method minimally matches behaviour.
            Assert.True(false, "Test not implemented.");
        }

        /// <summary>
        /// Test the Fingerprint method, using an invalid (malformed) Bitmap and the PHASH mode.
        /// </summary>
        [Fact]
        public void Fingerprint_InvalidBitmap_PHASH_ShouldThrowArgument()
        {
            // TODO: Implement test once related method minimally matches behaviour.
            Assert.True(false, "Test not implemented.");
        }

        /// <summary>
        /// Test the Fingerprint method, using a valid Bitmap and the PCASIFT mode.
        /// </summary>
        [Fact]
        public void Fingerprint_ValidBitmap_PCASIFT_ShouldSucceed()
        {
            // TODO: Implement test once related method minimally matches behaviour.
            Assert.True(false, "Test not implemented.");
        }

        /// <summary>
        /// Test the Fingerprint method, using a valid Bitmap and the SIFT mode.
        /// </summary>
        [Fact]
        public void Fingerprint_ValidBitmap_SIFT_ShouldSucceed()
        {
            // TODO: Implement test once related method minimally matches behaviour.
            Assert.True(false, "Test not implemented.");
        }

        /// <summary>
        /// Test the Fingerprint method, using a valid Bitmap and the SURF mode.
        /// </summary>
        [Fact]
        public void Fingerprint_ValidBitmap_SURF_ShouldSucceed()
        {
            // TODO: Implement test once related method minimally matches behaviour.
            Assert.True(false, "Test not implemented.");
        }

        /// <summary>
        /// Test the Fingerprint method, using a valid Bitmap and the PHASH mode.
        /// </summary>
        [Fact]
        public void Fingerprint_ValidBitmap_PHASH_ShouldSucceed()
        {
            // TODO: Implement test once related method minimally matches behaviour.
            Assert.True(false, "Test not implemented.");
        }
    }
}
