using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cudafy;
using Cudafy.Host;
using Cudafy.Translator;

namespace ImageBird
{
    /// <summary>
    /// Class containing system variables (such as GPU information).
    /// </summary>
    public static class System
    {
        /// <summary>
        /// The current GPU used for acceleration.
        /// </summary>
        public static GPGPU Gpu { get; private set; } = GetFirstAvailableGpu();
        
        private static GPGPU GetFirstAvailableGpu()
        {
            if (CudafyHost.GetDeviceCount(CudafyModes.Target) < 1)
            {
                throw new CudafyHostException("No GPU was available.");
            }

            return CudafyHost.GetDevice(eGPUType.OpenCL, CudafyModes.DeviceId = 0);
        }
    }
}
