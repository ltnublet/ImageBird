using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageBird.Frontend.Shared
{
    interface IIndex
    {
        IReadOnlyCollection<string> MonitoredDirectories { get; }

        ConcurrentTwoWayDictionary<string, PerceptualHash> Files { get; }

        void Save(string path);
    }
}
